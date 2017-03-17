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
using System.Linq;
using System.Collections.Generic;

namespace MaulingMonkey.Build.Sln {
	public partial class Solution {
		/// <summary>
		/// Corresponds to a Project(...) = .../EndProject region that represents a Solution Folder instead of an actual project.
		/// </summary>
		public class Folder : IEnumerable<IItem>, IItem {
			public string					Name			{ get; internal set; }
			public Guid						Guid			{ get; internal set; }

			// TODO: Decide how to support adding/removing from the folder while maintaining graph invariants.  Add(...)/Remove(...) methods directly on the Folder?
			// For now, just expose these as IEnumerable<...>s.
			public IEnumerable<Folder>		Folders			{ get { return _Folders	.AsReadOnly(); } }
			public IEnumerable<Project>		Projects		{ get { return _Projects.AsReadOnly(); } }
			public IEnumerable<File>		Files			{ get { return _Files	.AsReadOnly(); } }

			internal Folder					ParentFolder	{ get; set; } // Null if belonging directly to the solution.  TODO: Decide if we want to expose e.g. an IEnumerable<IItem> Parent ?  Or expose this as-is?  Or eliminate this in favor of a SolutionStreamReader field?
			internal readonly List<Folder>	_Folders		= new List<Folder>();
			internal readonly List<Project>	_Projects		= new List<Project>();
			internal readonly List<File>	_Files			= new List<File>();

			public IEnumerator<IItem> GetEnumerator() {
				return _Folders.Cast<IItem>()
					.Concat(_Projects)
					.Concat(_Files)
					.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
		}
	}
}
