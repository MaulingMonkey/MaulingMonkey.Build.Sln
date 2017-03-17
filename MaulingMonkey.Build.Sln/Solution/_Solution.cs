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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MaulingMonkey.Build.Sln {
	/// <summary>
	/// Represents a parsed Visual Studio Solution (.sln) file.
	/// </summary>
	public partial class Solution : IEnumerable<Solution.IItem> {
		public string											OriginalPath					{ get; private set; } // Example: "I:\home\projects\new\MaulingMonkey.Build.Sln\MaulingMonkey.Build.Sln.sln"
		public string											FormatVersion					{ get; private set; } // Example: "12.00"
		public string											SkuHint							{ get; private set; } // Example: "Visual Studio 14" - see VisualStudioSkus for likely matches
		public string											VisualStudioVersion				{ get; private set; } // Example: "14.0.24720.0"
		public string											MinimumVisualStudioVersion		{ get; private set; } // Example: "10.0.40219.1"

		// TODO: Decide how to support adding/removing from the solution while maintaining graph invariants.  Add(...)/Remove(...) methods directly on the Solution?
		// For now, just expose these as IEnumerable<...>s.
		readonly List<ConfigurationPlatform>					_ConfigurationPlatforms			= new List<ConfigurationPlatform>();
		readonly List<string>									_Configurations					= new List<string>();
		readonly List<string>									_Platforms						= new List<string>(); // Note: VS2005+ only!  VS2002/2003 have no concept of Solution level platforms, only configurations.  ConfigurationPlatforms will work.
		readonly List<Folder>									_AllFolders						= new List<Folder>();
		readonly List<Folder>									_RootFolders					= new List<Folder>();
		readonly List<Project>									_AllProjects					= new List<Project>();
		readonly List<Project>									_RootProjects					= new List<Project>();

		public readonly BuildMatrix								BuildConfiguration				= new BuildMatrix();



		/// <summary>
		/// Solution Configuration|Platform pairs.
		/// <para>Note: VS2002/VS2003 don't have the concept of solution-level platforms, so this will be a list of Configurations for those solutions.</para>
		/// <para>Example: {"Debug|Any CPU", "Release|Any CPU"}</para>
		/// </summary>
		public IEnumerable<ConfigurationPlatform>				ConfigurationPlatforms			{ get { return _ConfigurationPlatforms	.AsReadOnly(); } }

		/// <summary>
		/// All solution configurations.
		/// <para>Example: {"Debug", "Release"}</para>
		/// </summary>
		public IEnumerable<string>								Configurations					{ get { return _Configurations			.AsReadOnly(); } }

		/// <summary>
		/// All solution platforms.
		/// <para>Note: VS2002/VS2003 don't have the concept of solution-level platforms, so this will be empty for those solutions.  Consider using <seealso cref="ConfigurationPlatforms"/> instead.</para>
		/// <para>Example: {"Any CPU"}</para>
		/// </summary>
		public IEnumerable<string>								Platforms						{ get { return _Platforms				.AsReadOnly(); } }


		/// <summary>
		/// Folders belonging directly to the solution.
		/// <para>To get all folders recursively, see <seealso cref="AllFolders"/> instead.</para>
		/// <para>Example: {"Configuration", "Documentation"}</para>
		/// </summary>
		public IEnumerable<Folder>								RootFolders						{ get { return _RootFolders				.AsReadOnly(); } }

		/// <summary>
		/// Projects belonging directly to the solution.
		/// <para>To get all projects recursively, see <seealso cref="AllProjects"/> instead.</para>
		/// <para>Example: {"MaullingMonkey.Build.Sln", "MaulingMonkey.Build.Sln.Example"}</para>
		/// </summary>
		public IEnumerable<Project>								RootProjects					{ get { return _RootProjects			.AsReadOnly(); } }

		/// <summary>
		/// All folders belonging directly or indirectly to the solution.
		/// <para>To get only folders belonging to the root of the solution, see <seealso cref="RootFolders"/> instead.</para>
		/// <para>Example: {"Configuration", "Documentation"}</para>
		/// </summary>
		public IEnumerable<Folder>								AllFolders						{ get { return _AllFolders				.AsReadOnly(); } }

		/// <summary>
		/// All projects belonging directly or indirectly to the solution.
		/// <para>To get only projects belonging to the root of the solution, see <seealso cref="RootProjects"/> instead.</para>
		/// <para>Example: {"MaullingMonkey.Build.Sln", "MaulingMonkey.Build.Sln.Example"}</para>
		/// </summary>
		public IEnumerable<Project>								AllProjects						{ get { return _AllProjects				.AsReadOnly(); } }

		/// <summary>
		/// Find a project belonging to the solution by it's guid.  Returns null if no such project exists.
		/// </summary>
		public Project											FindProject(Guid guid)			{ return _AllProjects.Find(pr => pr.Guid == guid); }

		// More GlobalSection s - TODO: Expose
		readonly PerformanceDictionary						Performance						= new PerformanceDictionary();
		readonly TestCaseManagementSettingsDictionary		TestCaseManagementSettings		= new TestCaseManagementSettingsDictionary();
		readonly SolutionPropertiesDictionary				SolutionProperties				= new SolutionPropertiesDictionary();
		readonly MonoDevelopPropertiesDictionary			MonoDevelopProperties			= new MonoDevelopPropertiesDictionary();
		readonly TeamFoundationVersionControlDictionary		TeamFoundationVersionControl	= new TeamFoundationVersionControlDictionary();

		public IEnumerator<IItem> GetEnumerator() {
			return _RootFolders.Cast<IItem>()
				.Concat(_RootProjects)
				.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }



		static readonly RegexOptions CommonRegexOptions
			= RegexOptions.Compiled
			| RegexOptions.CultureInvariant
			| RegexOptions.ExplicitCapture
			// | RegexOptions.IgnoreCase
			| RegexOptions.Singleline
			;
	}
}
