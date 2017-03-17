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

namespace MaulingMonkey.Build.Sln {
	public partial class Solution {
		/// <summary>
		/// Solution Project, Folder, or File.
		/// </summary>
		public interface IItem {
			// TODO: Expose item type?  Enum?  Hmm.
			string Name { get; }
			// TODO: Properties that aren't really shared by all items such as paths, subitems, etc.?
		}
	}
}
