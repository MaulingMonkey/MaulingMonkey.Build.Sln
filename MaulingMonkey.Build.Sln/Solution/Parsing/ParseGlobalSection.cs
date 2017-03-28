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
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MaulingMonkey.Build.Sln {
	partial class Solution {
		static readonly Regex reGlobalSection					= new Regex(@"^  \s*  GlobalSection  \s*\(\s*  (?<name>[^)]+?)  \s*\)\s*=\s*  (?<stage>.+?)  \s*  $", CommonRegexOptions | RegexOptions.IgnorePatternWhitespace);
		static readonly Regex reEndGlobalSection				= new Regex(@"^  \s*  EndGlobalSection  \s*  $", CommonRegexOptions | RegexOptions.IgnorePatternWhitespace);

		//static readonly Regex reProjectConfiguration			= new Regex(@"^  \s*  (?<projectGuid>\{[-a-fA-F0-9]+\})  \.  (?<slnConfigPlatform>  (?<slnConfig>[^.]+)  )  \.  (?<option>[^=]+?)  \s* = \s*  "
		//														+ @"(?<projectConfigPlatform>  (?<projectConfig>[^|]+)  \|  (?<projectPlatform>.+?)  )  \s*  $", CommonRegexOptions | RegexOptions.IgnorePatternWhitespace);
		//
		//static readonly Regex reProjectConfigurationPlatforms	= new Regex(@"^  \s*  (?<projectGuid>\{[-a-fA-F0-9]+\})  \.  (?<slnConfigPlatform>  (?<slnConfig>[^|]+)  \|  (?<slnPlatform>[^.]+)  )  \.  (?<option>[^=]+?)  \s* = \s*  "
		//														+ @"(?<projectConfigPlatform>  (?<projectConfig>[^|]+)  \|  (?<projectPlatform>.+?)  )  \s*  $", CommonRegexOptions | RegexOptions.IgnorePatternWhitespace);

		static readonly Regex reProjectGuidSlnConfigPlatformOption = new Regex(@"^  \s*  (?<projectGuid>\{[-a-fA-F0-9]+\})  \.  (?<slnConfigPlatform>.*?)  \.  (?<option>[a-zA-Z][a-zA-Z0-9]*(\.\d+)?)  \s*  $", CommonRegexOptions | RegexOptions.IgnorePatternWhitespace);

		/// <summary>
		/// Parse a GlobalSection(???)...EndGlobalSection region.
		/// </summary>
		/// <param name="globalSection">A reGlobalSection match</param>
		void ParseGlobalSection(SolutionStreamReader reader, Match globalSection) {
			var name	= globalSection.Groups["name"].Value;
			var stage	= globalSection.Groups["stage"].Value; // preSolution, postSolution, etc.

			switch (name) {
			case "SolutionNotes":					ParseGlobalSection_Unknown							(reader); break; // VS 7.1 / 2003- ?
			case "SolutionConfiguration":			ParseGlobalSection_SolutionConfiguration			(reader); break; // VS 7.1 / 2003- version of "SolutionConfigurationPlatforms" - no solution platforms at this time?
			case "ProjectConfiguration":			ParseGlobalSection_ProjectConfiguration				(reader); break; // VS 7.1 / 2003- version of "ProjectConfigurationPlatforms" - no solution platforms at this time?
			case "ProjectDependencies":				ParseGlobalSection_ProjectDependencies				(reader); break; // VS 7.1 / 2003- version of "ProjectSection(ProjectDependencies)" - key.0/value guid pairs.
			case "ExtensibilityGlobals":			ParseGlobalSection_Unknown							(reader); break; // Seen (empty) in DCSS's visualc71\libpng.sln
			case "ExtensibilityAddIns":				ParseGlobalSection_Unknown							(reader); break; // Seen (empty) in DCSS's visualc71\libpng.sln
			case "SolutionConfigurationPlatforms":	ParseGlobalSection_SolutionConfigurationPlatforms	(reader); break; // VS2005+
			case "ProjectConfigurationPlatforms":	ParseGlobalSection_ProjectConfigurationPlatforms	(reader); break; // VS2005+
			case "NestedProjects":					ParseGlobalSection_NestedProjects					(reader); break;
			case "Performance":						ParseGlobalSection_KeyValues						(reader, Performance,						false); break;
			case "TestCaseManagementSettings":		ParseGlobalSection_KeyValues						(reader, TestCaseManagementSettings,		false); break;
			case "SolutionProperties":				ParseGlobalSection_KeyValues						(reader, SolutionProperties,				false); break;
			case "MonoDevelopProperties":			ParseGlobalSection_KeyValues						(reader, MonoDevelopProperties,				true); break;
			case "TeamFoundationVersionControl":	ParseGlobalSection_KeyValues						(reader, TeamFoundationVersionControl,		false); break;
			case "DPCodeReviewSolutionGUID":		ParseGlobalSection_KeyValues						(reader, new Dictionary<string, string>(),	false); break; // Contains a DPCodeReviewSolutionGUID = {guid}

			default:
				Warning(reader, "Unrecognized GlobalSection");
				ParseGlobalSection_Unknown(reader);
				break;
			}
		}

		static IEnumerable<string> GlobalSection_Lines(SolutionStreamReader reader) {
			for(;;) {
				Match m;
				var line = reader.ReadLine();
				if		(line == null)									throw new SolutionParseException(reader, "Unexpected EOF while parsing GlobalSection");
				else if	(string.IsNullOrWhiteSpace(line))				continue;
				else if	((m=reEndGlobalSection	.Match(line)).Success)	break;
				else													yield return line;
			}
		}

		static IEnumerable<KeyValuePair<string,string>> GlobalSection_KeyValues(SolutionStreamReader reader) {
			foreach (var line in GlobalSection_Lines(reader)) {
				var m = reKeyValue.Match(line);
				if (!m.Success) throw new SolutionParseException(reader, "Expected key/value pair in GlobalSection");

				yield return new KeyValuePair<string, string>(m.Groups["key"].Value, m.Groups["value"].Value);
			}
		}

		/// <summary>
		/// Parse a GlobalSection(SolutionConfiguration)...EndGlobalSection region.
		/// Visual Studio 2002/2003- enumeration of solution "Config"s.  Note that solution "Platform"s did not exist at this time.
		/// </summary>
		void ParseGlobalSection_SolutionConfiguration(SolutionStreamReader reader) {
			// FormatVersion 7.00 (VS2002):  "ConfigName.0 = Debug"
			// FormatVersion 8.00 (VS2003):  "Debug = Debug"
			foreach (var line in GlobalSection_KeyValues(reader)) {
				if (FormatVersion == "7.00" && !line.Key.StartsWith("ConfigName."))	throw new SolutionParseException(reader, "Expected \"ConfigName.# = Config\" key/value pairs within 7.00 GlobalSection(SolutionConfiguration)");
				if (FormatVersion == "8.00" && line.Key != line.Value)				throw new SolutionParseException(reader, "Expected \"Config = Config\" key/value pairs within 8.00 GlobalSection(SolutionConfiguration)");

				var slnConfigPlatform = new ConfigurationPlatform(line.Value, null);
				if (_ConfigurationPlatforms.Any(scp => scp == slnConfigPlatform)) return; // Duplicate

				if (_Configurations.Find(e => e == line.Value) == null) _Configurations.Add(line.Value);

				_ConfigurationPlatforms.Add(slnConfigPlatform);
			}
		}

		/// <summary>
		/// Parse a GlobalSection(ProjectConfiguration)...EndGlobalSection region.
		/// Visual Studio 2002/2003- mapping of solution "Config" to project "Config|Platform" and build/deploy state.
		/// </summary>
		void ParseGlobalSection_ProjectConfiguration(SolutionStreamReader reader) {
			BuildConfiguration.Initialize(ConfigurationPlatforms, AllProjects);

			// {ProjectGuid}.{SlnConfig}.ActiveCfg = {ProjectConfig}|{ProjectPlatform}
			// {ProjectGuid}.{SlnConfig}.Build.0   = {ProjectConfig}|{ProjectPlatform}
			// {ProjectGuid}.{SlnConfig}.Deploy.0  = {ProjectConfig}|{ProjectPlatform}
			foreach (var line in GlobalSection_KeyValues(reader)) {
				Match m = reProjectGuidSlnConfigPlatformOption.Match(line.Key);
				if (!m.Success) throw new SolutionParseException(reader, "Expected {ProjectGuid}.{SolutionConfig}.{Option} = {ProjectConfig}|{ProjectPlatform} within GlobalSection(ProjectConfiguration)");

				var projectGuid				= new Guid(m.Groups["projectGuid"].Value);
				var slnConfigPlatform		= new ConfigurationPlatform(m.Groups["slnConfigPlatform"].Value);
				var option					= m.Groups["option"].Value;
				var projectConfigPlatform	= new ConfigurationPlatform(line.Value);

				var project = FindProject(projectGuid);
				if (project == null) {
					// MonoDevelop doesn't seem to cull removed projects from GlobalSection(ProjectConfigurationPlatforms), but this tends to only be generated by VS2002/2003?
					Warning(reader, "ProjectGuid specifies missing project within GlobalSection(ProjectConfiguration)");
					continue;
				}

				var bc = BuildConfiguration[slnConfigPlatform, FindProject(projectGuid)];

				switch (option) {
				case "ActiveCfg":	bc.ActiveCfg	= projectConfigPlatform.Name; break;
				case "Build.0":		bc.Build		= projectConfigPlatform.Name == bc.ActiveCfg; break;
				case "Deploy.0":	bc.Deploy		= projectConfigPlatform.Name == bc.ActiveCfg; break;
				default:			Warning(reader, "Unrecognized option in GlobalSection(ProjectConfiguration)"); break;
				}

				BuildConfiguration[slnConfigPlatform, FindProject(projectGuid)] = bc;
			}
		}

		/// <summary>
		/// Parse a GlobalSection(ProjectDependencies)...EndGlobalSection region.
		/// </summary>
		void ParseGlobalSection_ProjectDependencies(SolutionStreamReader reader) {
			// {ParentGuid}.0 = {ChildGuid}
			// {ParentGuid}.1 = {ChildGuid}
			foreach (var line in GlobalSection_KeyValues(reader)) {
				var parent = line.Key.Split(".".ToCharArray(), 2);
				if (parent.Length != 2) throw new SolutionParseException(reader, "Expected {ParentGuid}.# = {ChildGuid} pairs within GlobalSection(ProjectDependencies), but key lacks '.' character");
				Guid parentGuid;
				Guid childGuid;
				if (!Guid.TryParse(parent[0], out parentGuid))	throw new SolutionParseException(reader, "Failed to parse ParentGuid within GlobalSection(ProjectDependencies)");
				if (!Guid.TryParse(line.Value, out childGuid))	throw new SolutionParseException(reader, "Failed to parse ChildGuid within GlobalSection(ProjectDependencies)");
				reader.DependenciesFor(parentGuid).Add(childGuid);
			}
		}

		/// <summary>
		/// Parse a GlobalSection(SolutionConfigurationPlatforms)...EndGlobalSection region.
		/// Visual Studio 2005+ enumeration of solution "Config|Platform"s.
		/// </summary>
		void ParseGlobalSection_SolutionConfigurationPlatforms(SolutionStreamReader reader) {
			// Debug|Any CPU   = Debug|Any CPU
			// Release|Any CPU = Release|Any CPU
			foreach (var line in GlobalSection_KeyValues(reader)) {
				if (line.Key != line.Value) throw new SolutionParseException(reader, "Expected \"Config|Platform = Config|Platform\" key/value pairs within GlobalSection(ParseGlobalSection_SolutionConfigurationPlatforms), but key/value mismatched");

				var split = line.Value.Split("|".ToCharArray(), 2);
				if (split.Length != 2) throw new SolutionParseException(reader, "Expected \"Config|Platform = Config|Platform\" key/value pairs within GlobalSection(ParseGlobalSection_SolutionConfigurationPlatforms), but seperation pipe is missing");

				var slnConfigPlatform = new ConfigurationPlatform(split[0], split[1]);
				if (_ConfigurationPlatforms.Any(scp => scp == slnConfigPlatform)) return; // Duplicate

				if (_Configurations	.Find(e => e == slnConfigPlatform.Configuration)	== null) _Configurations.Add(slnConfigPlatform.Configuration);
				if (_Platforms		.Find(e => e == slnConfigPlatform.Platform)			== null) _Platforms		.Add(slnConfigPlatform.Platform);
				_ConfigurationPlatforms	.Add(slnConfigPlatform);
			}
		}

		/// <summary>
		/// Parse a GlobalSection(ProjectConfigurationPlatforms)...EndGlobalSection region.
		/// Visual Studio 2005+ mapping of solution "Config|Platform" to project "Config|Platform" and build/deploy state.
		/// </summary>
		void ParseGlobalSection_ProjectConfigurationPlatforms(SolutionStreamReader reader) {
			BuildConfiguration.Initialize(ConfigurationPlatforms, AllProjects);

			// {ProjectGuid}.{SlnConfig}|{SlnPlatform}.ActiveCfg = {ProjectConfig}|{ProjectPlatform}
			// {ProjectGuid}.{SlnConfig}|{SlnPlatform}.Build.0   = {ProjectConfig}|{ProjectPlatform}
			// {ProjectGuid}.{SlnConfig}|{SlnPlatform}.Deploy.0  = {ProjectConfig}|{ProjectPlatform}
			foreach (var line in GlobalSection_KeyValues(reader)) {
				Match m = reProjectGuidSlnConfigPlatformOption.Match(line.Key);
				if (!m.Success) throw new SolutionParseException(reader, "Expected {ProjectGuid}.{SolutionConfig}|{SolutionPlatform}.{Option} = {ProjectConfig}|{ProjectPlatform} within GlobalSection(ProjectConfigurationPlatforms)");

				var projectGuid				= new Guid(m.Groups["projectGuid"].Value);
				var slnConfigPlatform		= new ConfigurationPlatform(m.Groups["slnConfigPlatform"].Value);
				var option					= m.Groups["option"].Value;
				var projectConfigPlatform	= new ConfigurationPlatform(line.Value);

				var project = FindProject(projectGuid);
				if (project == null) {
					// MonoDevelop doesn't seem to cull removed projects from GlobalSection(ProjectConfigurationPlatforms)
					// Warning(reader, "ProjectGuid specifies missing project within GlobalSection(ProjectConfigurationPlatforms)");
					continue;
				}

				var bc = BuildConfiguration[slnConfigPlatform, FindProject(projectGuid)];

				switch (option) {
				case "ActiveCfg":	bc.ActiveCfg	= projectConfigPlatform.Name; break;
				case "Build.0":		bc.Build		= projectConfigPlatform.Name == bc.ActiveCfg; break;
				case "Deploy.0":	bc.Deploy		= projectConfigPlatform.Name == bc.ActiveCfg; break;
				default:			Warning(reader, "Unrecognized option in GlobalSection(ProjectConfigurationPlatforms)"); break;
				}

				BuildConfiguration[slnConfigPlatform, FindProject(projectGuid)] = bc;
			}
		}

		/// <summary>
		/// Parse a GlobalSection(NestedProjects)...EndGlobalSection region.
		/// Maps projects and folders to their parent folders, if any.
		/// </summary>
		void ParseGlobalSection_NestedProjects(SolutionStreamReader reader) {
			// Note: Whitespace (or more accurately, tabs) is extremely important here: https://twitter.com/aras_p/status/846643378481713153

			// {ChildProjectGuid} = {ParentFolderGuid}
			// {ChildFolderGuid}  = {ParentFolderGuid}
			foreach (var line in GlobalSection_KeyValues(reader)) {
				var childStr	= line.Key;
				var parentStr	= line.Value;

				Guid childGuid, parentGuid;
				if (!Guid.TryParse(childStr,  out childGuid))  throw new SolutionParseException(reader, "Couldn't parse child GUID within GlobalSection(NestedProjects)");
				if (!Guid.TryParse(parentStr, out parentGuid)) throw new SolutionParseException(reader, "Couldn't parse parent GUID within GlobalSection(NestedProjects)");

				// Parent should always be a SolutionFolder
				var parentFolder = _AllFolders.Find(sf => sf.Guid == parentGuid);
				if (parentFolder == null) throw new SolutionParseException(reader, "GlobalSection(NestedProjects) specifies nonexistant Solution Folder with GUID "+parentGuid);

				// Child may be a SolutionFolder, or a Project.
				var childFolder = _AllFolders.Find(sf => sf.Guid == childGuid);
				var childProject = _AllProjects.Find(pr => pr.Guid == childGuid);

				if		(childFolder != null)	{ parentFolder._Folders.Add(childFolder);   childFolder .ParentFolder = parentFolder; }
				else if	(childProject != null)	{ parentFolder._Projects.Add(childProject); childProject.ParentFolder = parentFolder; }
				else							throw new SolutionParseException(reader, "GlobalSection(NestedProjects) specifies nonexistant child Solution Folder or Project with GUID "+childGuid);
			}

			// TODO: Consider sanity checking for loops
		}

		/// <summary>
		/// Parse a GlobalSection(???)...EndGlobalSection region of key/value pairs.
		/// </summary>
		void ParseGlobalSection_KeyValues(SolutionStreamReader reader, Dictionary<string,string> targetDictionary, bool mergeDuplicates) {
			foreach (var line in GlobalSection_KeyValues(reader)) {
				WarningIf(!mergeDuplicates && targetDictionary.ContainsKey(line.Key), reader, "Duplicate key in GlobalSection");

				if (mergeDuplicates && targetDictionary.ContainsKey(line.Key))	targetDictionary[line.Key] += "\n" + line.Value;
				else															targetDictionary[line.Key] = line.Value;
			}
		}

		/// <summary>
		/// Parse a GlobalSection(???)...EndGlobalSection region of unknown content.
		/// </summary>
		void ParseGlobalSection_Unknown(SolutionStreamReader reader) {
			for (;;) {
				var line = reader.ReadLine();
				if		(line == null) throw new SolutionParseException(reader, "Unexpected EOF while parsing GlobalSection");
				else if	(string.IsNullOrWhiteSpace(line))	continue;
				else if	(reEndGlobalSection.IsMatch(line))	break;
				// else we already warned about an unrecognized GlobalSection - ignore contents
			}
		}
	}
}
