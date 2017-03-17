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
using System.Diagnostics;

namespace MaulingMonkey.Build.Sln {
	public partial class Solution {
		/// <summary>
		/// Corresponds to the settings configured under the Visual Studio "Configuration Manager..." UI.
		/// </summary>
		public class BuildMatrix {
			/// <summary>
			/// Solution Config+Platform + Project (Key)
			/// </summary>
			struct SCPP {
				public ConfigurationPlatform	SlnConfigPlatform;
				public Project					Project;

				public SCPP(ConfigurationPlatform slnConfigPlatform, Project project) {
					SlnConfigPlatform	= slnConfigPlatform;
					Project				= project;
				}
			}

			public struct Entry {
				public string	ActiveCfg;
				public bool		Build;
				public bool		Deploy;
			}

			readonly List<string>						Configs			= new List<string>();
			readonly List<string>						Platforms		= new List<string>();
			readonly List<ConfigurationPlatform>		ConfigPlatforms	= new List<ConfigurationPlatform>();
			readonly Dictionary<SCPP, Entry>			Entries			= new Dictionary<SCPP, Entry>();

			internal BuildMatrix() { }
			internal BuildMatrix(IEnumerable<ConfigurationPlatform> solutionConfigPlatforms, IEnumerable<Project> projects) { Initialize(solutionConfigPlatforms, projects); }

			internal void Initialize(IEnumerable<ConfigurationPlatform> solutionConfigPlatforms, IEnumerable<Project> projects) {
				Debug.Assert(Configs		.Count == 0);
				Debug.Assert(Platforms		.Count == 0);
				Debug.Assert(ConfigPlatforms.Count == 0);
				Debug.Assert(Entries		.Count == 0);

				var uniqueConfigs			= new HashSet<string>();
				var uniquePlatforms			= new HashSet<string>();
				var uniqueConfigPlatforms	= new HashSet<ConfigurationPlatform>();

				foreach (var scp in solutionConfigPlatforms) {
					if (scp.Configuration	!= null && uniqueConfigs	.Add(scp.Configuration	)) Configs	.Add(scp.Configuration	);
					if (scp.Platform		!= null && uniquePlatforms	.Add(scp.Platform		)) Platforms.Add(scp.Platform		);

					if (!uniqueConfigPlatforms.Add(scp)) continue; // duplicate - maybe throw an error instead?
					ConfigPlatforms.Add(scp);

					foreach (var project in projects) {
						var scpap = new SCPP(scp, project);
						Entries.Add(scpap, new Entry() {
							ActiveCfg	= null,
							Build		= false,
							Deploy		= false,
						});
					}
				}
			}

			public Entry this[ConfigurationPlatform solutionConfigPlatform, Project project] {
				get				{ return Entries[new SCPP(solutionConfigPlatform,project)]; }
				internal set	{ Entries[new SCPP(solutionConfigPlatform,project)] = value; }
			}
		}
	}
}
