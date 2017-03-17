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
		/// Corresponds to a GlobalSection(TeamFoundationVersionControl)...EndGlobalSection region.
		/// </summary>
		class TeamFoundationVersionControlDictionary : GlobalSectionDictionary {
			// SccNumberOfProjects = 36
			public int SccNumberOfProjects {
				get { string s; int i; return TryGetValue("SccNumberOfProjects", out s) && int.TryParse(s, out i) ? i : 0; }
				set { this["SccNumberOfProjects"] = value.ToString(); }
			}

			// SccEnterpriseProvider = {4CA58AB2-18FA-4F8D-95D4-32DDF27D184C}
			public Guid SccEnterpriseProvider {
				get { string s; Guid g; return TryGetValue("SccEnterpriseProvider", out s) && Guid.TryParse(s, out g) ? g : Guid.Empty; }
				set { this["SccEnterpriseProvider"] = value.ToString(); }
			}

			// SccTeamFoundationServer = https://tfs04.codeplex.com/
			public string SccTeamFoundationServer {
				get { string value; return TryGetValue("SccTeamFoundationServer", out value) ? value : null; }
				set { if (value == null) Remove(value); else this["SccTeamFoundationServer"] = value; }
			}

			// SccLocalPath0 = .

			// SccProjectUniqueName1 = Cosmos\\Cosmos.Kernel\\Cosmos.Kernel.csproj
			// SccProjectTopLevelParentUniqueName1 = Cosmos.sln
			// SccProjectName1 = Cosmos/Cosmos.Kernel
			// SccLocalPath1 = Cosmos\\Cosmos.Kernel

			// [...]

			// SccProjectUniqueName35 = ..\\source2\\Debug\\Cosmos.Debug.GDB\\Cosmos.Debug.GDB.csproj
			// SccProjectTopLevelParentUniqueName35 = Cosmos.sln
			// SccProjectName35 = ../source2/Debug/Cosmos.Debug.GDB
			// SccLocalPath35 = ..\\source2\\Debug\\Cosmos.Debug.GDB
		}
	}
}
