// Copyright 2017 MaulingMonkey
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Text.RegularExpressions;

namespace MaulingMonkey.Build.Sln {
	partial class Solution {
		static readonly Regex reProjectSection		= new Regex(@"^  \s*  ProjectSection  \s*\(\s*  (?<name>[^)]+?)  \s*\)\s*=\s*  (?<stage>.+?)  \s*$", CommonRegexOptions | RegexOptions.IgnorePatternWhitespace);
		static readonly Regex reEndSectionProject	= new Regex(@"^  \s*  EndProjectSection  \s*  $", CommonRegexOptions | RegexOptions.IgnorePatternWhitespace);

		/// <summary>
		/// Parse a ProjectSection(???)...EndProjectSection region belonging to a real project.
		/// </summary>
		/// <param name="projectSection">A reProjectSection match</param>
		void ParseProjectSection(SolutionStreamReader reader, Match projectSection, Project project) {
			var name			= projectSection.Groups["name"].Value;
			var stage			= projectSection.Groups["stage"].Value; // preProject, postProject, ...?

			switch (name) {
			case "SolutionItems":			throw new SolutionParseException(reader, "Solution projects are not expected to have ProjectSection(SolutionItems) sections");
			case "ProjectDependencies":		ParseProjectSection_ProjectDependencies(reader, project); break;
			case "SiliconStudioPackage":	ParseProjectSection_Unknown(reader); break; // lists .pdxpkg files - related to http://xenko.com/
			default:
				Warning(reader, "Unrecognized ProjectSection \"{0}\"", name);
				ParseProjectSection_Unknown(reader);
				break;
			}
		}

		/// <summary>
		/// Parse a ProjectSection(???)...EndProjectSection region belonging to a folder.
		/// </summary>
		/// <param name="projectSection">A reProjectSection match</param>
		void ParseProjectSection(SolutionStreamReader reader, Match projectSection, Folder folder) {
			var name			= projectSection.Groups["name"].Value;
			var stage			= projectSection.Groups["stage"].Value; // preProject, postProject, ...?

			switch (name) {
			case "SolutionItems":			ParseProjectSection_SolutionItems(reader, folder); break;
			case "ProjectDependencies":		throw new SolutionParseException(reader, "Solution Folders are not expected to have ProjectSection(ProjectDependencies) sections");
			case "SiliconStudioPackage":	ParseProjectSection_Unknown(reader); break; // lists .pdxpkg files - related to http://xenko.com/
			default:
				Warning(reader, "Unrecognized ProjectSection \"{0}\"", name);
				ParseProjectSection_Unknown(reader);
				break;
			}
		}



		/// <summary>
		/// Parse a ProjectSection(SolutionItems)...EndProjectSection region belonging to a folder.
		/// </summary>
		void ParseProjectSection_SolutionItems(SolutionStreamReader reader, Folder folder) {
			for (;;) {
				Match m;
				var line = reader.ReadLine();
				if		(line == null)									throw new SolutionParseException(reader, "Unexpected EOF while parsing ProjectSection(SolutionItems)");
				else if	(string.IsNullOrWhiteSpace(line))				continue;
				else if	((m=reEndSectionProject	.Match(line)).Success)	break;
				else if	((m=reKeyValue			.Match(line)).Success)	ParseSolutionItem(reader, m, folder);
				else													Warning(reader, "Unexpected non-key/value in ProjectSection(SolutionItems)");
			}
		}

		void ParseSolutionItem(SolutionStreamReader reader, Match solutionItem, Folder folder) {
			var key		= solutionItem.Groups["key"].Value;
			var value	= solutionItem.Groups["value"].Value;
			if (key != value) throw new SolutionParseException(reader, "Unexpected key/value mismatch in ProjectSection(SolutionItems)");

			var file = new File(value);
			folder._Files.Add(file);
		}



		/// <summary>
		/// Parse a ProjectSection(ProjectDependencies)...EndProjectSection region belonging to a project.
		/// </summary>
		void ParseProjectSection_ProjectDependencies(SolutionStreamReader reader, Project project) {
			for (;;) {
				Match m;
				var line = reader.ReadLine();
				if		(line == null)									throw new SolutionParseException(reader, "Unexpected EOF while parsing ProjectSection(ProjectDependencies)");
				else if	(string.IsNullOrWhiteSpace(line))				continue;
				else if	((m=reEndSectionProject	.Match(line)).Success)	break;
				else if	((m=reKeyValue			.Match(line)).Success) {
					var key		= m.Groups["key"].Value;
					var value	= m.Groups["value"].Value;
					if (key != value) throw new SolutionParseException(reader, "Unexpected key/value mismatch in ProjectSection(ProjectDependencies)");
					Guid guid;
					if (!Guid.TryParse(value, out guid)) throw new SolutionParseException(reader, "ProjectSection(ProjectDependencies) key/value failed to parse as a Guid");
					reader.DependenciesFor(project).Add(guid);
				}
				else													Warning(reader, "Unexpected non-key/value in ProjectSection(SolutionItems)");
			}
		}



		/// <summary>
		/// Parse a ProjectSection(???)...EndProjectSection
		/// </summary>
		void ParseProjectSection_Unknown(SolutionStreamReader reader) {
			for (;;) {
				var line = reader.ReadLine();
				if		(line == null)							throw new SolutionParseException(reader, "Unexpected EOF while parsing ProjectSection(???)");
				else if	(string.IsNullOrWhiteSpace(line))		continue;
				else if	(reEndSectionProject.IsMatch(line))		break;
				// else no warning - already warned about unknown ProjectSection type
			}
		}
	}
}
