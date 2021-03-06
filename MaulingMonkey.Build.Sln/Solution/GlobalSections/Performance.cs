﻿// Copyright 2017 MaulingMonkey
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
		/// Corresponds to a GlobalSection(Performance)...EndGlobalSection region.
		/// </summary>
		class PerformanceDictionary : GlobalSectionDictionary {
			public bool HasPerformanceSessions {
				get { string value; return TryGetValue("HasPerformanceSessions", out value) && string.Compare(value, "true", StringComparison.InvariantCultureIgnoreCase) == 0; }
				set { this["HasPerformanceSessions"] = value ? "true" : "false"; }
			}
		}
	}
}
