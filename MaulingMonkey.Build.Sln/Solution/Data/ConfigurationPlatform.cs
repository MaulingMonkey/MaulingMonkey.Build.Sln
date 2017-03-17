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
using System.Diagnostics;

namespace MaulingMonkey.Build.Sln {
	public partial class Solution {
		[DebuggerDisplay("{Name}")]
		public struct ConfigurationPlatform {
			public ConfigurationPlatform(string configuration, string platform) {
				if (configuration == null) throw new ArgumentNullException(nameof(configuration), "ConfigurationPlatform:  Configuration cannot be null");
				// platform may be null for e.g. VS2002/2003 era solutions.

				Name			= platform == null ? configuration : string.Format("{0}|{1}", configuration, platform);
				Configuration	= configuration;
				Platform		= platform;
			}

			public ConfigurationPlatform(string configurationPlatform) {
				if (configurationPlatform == null) throw new ArgumentNullException(nameof(configurationPlatform), "ConfigurationPlatform:  Configuration|Platform combination cannot be null");

				Name = configurationPlatform;
				var s = configurationPlatform.Split("|".ToCharArray(), 2);
				Configuration	= s.Length > 0 ? s[0] : "";
				Platform		= s.Length > 1 ? s[1] : null;
			}

			public string	Name			{ get; internal set; }
			public string	Configuration	{ get; internal set; }
			public string	Platform		{ get; internal set; }

			public static bool operator==(ConfigurationPlatform lhs, ConfigurationPlatform rhs) { return lhs.Name == rhs.Name; }
			public static bool operator!=(ConfigurationPlatform lhs, ConfigurationPlatform rhs) { return lhs.Name != rhs.Name; }

			public override bool Equals(object obj) { return base.Equals(obj); }
			public override int GetHashCode() { return Name.GetHashCode(); }
		}
	}
}
