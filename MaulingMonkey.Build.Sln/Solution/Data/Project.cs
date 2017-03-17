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

namespace MaulingMonkey.Build.Sln {
	public partial class Solution {
		/// <summary>
		/// Corresponds to a Project(...) = .../EndProject region (minus Solution Folders.)
		/// </summary>
		public class Project : IItem {
			public Guid						TypeGuid		{ get; internal set; } // See WellKnownProjectTypes for some known types
			public Guid						Guid			{ get; internal set; }
			public string					Name			{ get; internal set; }
			public string					Path			{ get; internal set; }

			internal Folder					ParentFolder	{ get; set; } // Null if belonging directly to the solution.  TODO: Decide if we want to expose e.g. an IEnumerable<IItem> Parent ?  Or expose this as-is?  Or eliminate this in favor of a SolutionStreamReader field?
			internal readonly List<Project>	_Dependencies	= new List<Project>();

			/// <summary>
			/// Solution configured dependencies of this project on other projects.  Corresponds to the "Common Properties > Project Dependencies" tab under the solution properties (reached via "Properties" under the right click menu of the solution).
			/// <para>NOTE WELL:  This does *not* include *project* configured dependencies of the project on other projects (such as the project "References" node.)</para>
			/// </summary>
			public IEnumerable<Project>		Dependencies	{ get { return _Dependencies.AsReadOnly(); } }
			// TODO: Decide how to support adding/removing dependencies from the project while maintaining graph invariants.  Add(...)/Remove(...) methods directly on the Project?  What about dependency cycle checking?
		}
	}
}
