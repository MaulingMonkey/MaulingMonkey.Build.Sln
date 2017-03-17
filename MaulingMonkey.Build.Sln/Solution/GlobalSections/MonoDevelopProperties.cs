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

using System.Collections.Generic;

namespace MaulingMonkey.Build.Sln {
	public partial class Solution {
		/// <summary>
		/// Corresponds to a GlobalSection(MonoDevelopProperties)...EndGlobalSection region.
		/// </summary>
		class MonoDevelopPropertiesDictionary : GlobalSectionDictionary {
			public string StartupItem {
				get { string value; return TryGetValue("StartupItem", out value) ? value : null; }
				set { if (value == null) Remove(value); else this["StartupItem"] = value; }
			}

			// NOTE: We have some psuedo-heiarchy for other properties:
			// Policies = $0
			// $0.TextStylePolicy = $1
			// $1.inheritsSet = null
			// $1.scope = text/x-csharp
			// $0.CSharpFormattingPolicy = $2
			// $2.IndentPropertyBody = False
			// $2.inheritsSet = Mono
			// $2.inheritsScope = text/x-csharp
			// $2.scope = text/x-csharp
		}
	}
}
