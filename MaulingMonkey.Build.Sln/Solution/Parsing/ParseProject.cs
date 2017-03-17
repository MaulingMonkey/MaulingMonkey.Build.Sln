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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MaulingMonkey.Build.Sln {
	partial class Solution {
		static readonly Regex reProject		= new Regex(@"^  \s*  Project  \s*\(\s*  ""(?<typeGuid>\{[-a-fA-F0-9]+\})""  \s*\)\s*=\s*  ""(?<name>[^""]+)""  \s*,\s*  ""(?<path>[^""]+)""  \s*,\s*  ""(?<guid>\{[-a-fA-F0-9]+\})""  \s*$", CommonRegexOptions | RegexOptions.IgnorePatternWhitespace);
		static readonly Regex reEndProject	= new Regex(@"^  \s*  EndProject  \s*  $", CommonRegexOptions | RegexOptions.IgnorePatternWhitespace);

		/// <summary>
		/// Parse a Project("typeGuid") = "name", "path", "guid"...EndProject region.
		/// This may result in an actual Project, or a Folder.
		/// </summary>
		/// <param name="mProject">A reProject match</param>
		void ParseProject(SolutionStreamReader reader, Match mProject) {
			var typeGuidStr	= mProject.Groups["typeGuid"].Value;
			var name		= mProject.Groups["name"].Value;
			var relPath		= mProject.Groups["path"].Value;
			var guidStr		= mProject.Groups["guid"].Value;

			Guid typeGuid, guid;
			if (!Guid.TryParse(typeGuidStr,	out typeGuid))	throw new SolutionParseException(reader, "Project type guid ("+typeGuidStr+") failed to parse");
			if (!Guid.TryParse(guidStr,		out guid))		throw new SolutionParseException(reader, "Project guid ("+guidStr+") failed to parse");

			WarningIf(!WellKnownProjectTypes.All.Contains(typeGuid), reader, "Didn't recognize reference guid type: {0}", typeGuid.ToString());

			if (typeGuid == WellKnownProjectTypes.SolutionFolder) {
				if (name != relPath) throw new SolutionParseException(reader, "Expected solution folder name to match path");

				var folder = new Folder() {
					Name		= name,
					Guid		= guid,
				};

				for(;;) {
					Match m;
					var line = reader.ReadLine();

					if		(line == null)									throw new SolutionParseException(reader, "Unexpected EOF before end of Project section");
					else if (string.IsNullOrWhiteSpace(line))				continue; // Leading blank line
					else if ((m=reProjectSection	.Match(line)).Success)	ParseProjectSection(reader, m, folder);
					else if ((m=reEndProject		.Match(line)).Success)	break;
					else													Warning(reader, "Expected ProjectSection or EndProject within Project");
				}

				_AllFolders.Add(folder);
			} else { // Some kind of project
				var absPath = Path.Combine(Path.GetDirectoryName(OriginalPath), relPath);
				//WarningIf(!File.Exists(absPath), project.Value, "Failed to find project file: {0}", absPath);

				var project = new Project() {
					TypeGuid	= typeGuid,
					Name		= name,
					Path		= relPath,
					Guid		= guid,
				};

				for(;;) {
					Match m;
					var line = reader.ReadLine();

					if		(line == null)									throw new SolutionParseException(reader, "Unexpected EOF before end of Project section");
					else if (string.IsNullOrWhiteSpace(line))				continue; // Leading blank line
					else if ((m=reProjectSection	.Match(line)).Success)	ParseProjectSection(reader, m, project);
					else if ((m=reEndProject		.Match(line)).Success)	break;
					else													Warning(reader, "Expected ProjectSection or EndProject within Project");
				}

				_AllProjects.Add(project);
			}
		}
	}
}
