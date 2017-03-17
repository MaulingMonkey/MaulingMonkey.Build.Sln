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

using System.Linq;
using System.Text.RegularExpressions;

namespace MaulingMonkey.Build.Sln {
	partial class Solution {
		static readonly Regex reFormatVersion	= new Regex(@"^Microsoft Visual Studio Solution File, Format Version (?<version>\d+\.\d+)$", CommonRegexOptions);
		static readonly Regex reSkuHint			= new Regex(@"^# (?<sku>Visual .*)$", CommonRegexOptions);

		/// <summary>
		/// Parse an entire solution.
		/// </summary>
		void ParseRoot(SolutionStreamReader reader) {
			for(;;) {
				var line = reader.ReadLine();
				Match m;

				if		(line == null)								break; // Expected EOF
				else if	(string.IsNullOrWhiteSpace(line))			continue; // Leading blank line
				else if	((m=reFormatVersion	.Match(line)).Success)	FormatVersion		= m.Groups["version"].Value;
				else if	((m=reSkuHint		.Match(line)).Success)	SkuHint				= m.Groups["sku"].Value;
				else if	((m=reProject		.Match(line)).Success)	ParseProject		(reader, m);
				else if	((m=reGlobal		.Match(line)).Success)	ParseGlobal			(reader, m);
				else if	((m=reKeyValue		.Match(line)).Success)	ParseRootKeyValue	(reader, m);
				else												Warning(reader, "Unrecognized root line");
			}

			if (SkuHint == null) {
				switch (FormatVersion) {
				// 6.00 and earlier = Visual Studio 6 .dsw files, vastly different format.
				// Necessary - SkuHint will be missing!
				case "7.00":	SkuHint = WellKnownSkus.VisualStudio2002; break; // Observed in e.g. crawl-ref\source\contrib\zlib\contrib\vstudio\vc7\zlibvc.sln
				case "8.00":	SkuHint = WellKnownSkus.VisualStudio2003; break; // Observed in e.g. crawl-ref\source\contrib\libpng\projects\visualc71\libpng.sln
				// Un-necessary - SkuHint should exist, this is more for documentation purpouses.
				case "9.00":	SkuHint = WellKnownSkus.VisualStudio2005; break;
				case "10.00":	SkuHint = WellKnownSkus.VisualStudio2008; break;
				case "11.00":	SkuHint = WellKnownSkus.VisualStudio2010; break;
				case "12.00":	SkuHint = WellKnownSkus.VisualStudio2012; break; // Note: 2013+ keeps the FormatVersion stable.
				// No further FormatVersion bumps yet?
				default:		Warning(reader, "Couldn't infer SkuHint from FormatVersion"); break;
				}
			}
			WarningIf(!WellKnownSkus.All.Contains(SkuHint), reader, "Unrecognized SkuHint");

			// Final cleanup
			_RootFolders		.AddRange(_AllFolders	.Where(f => f.ParentFolder == null));
			_RootProjects	.AddRange(_AllProjects	.Where(p => p.ParentFolder == null));

			foreach (var project in _AllProjects) {
				var dependencies = reader.DependenciesFor(project);
				project._Dependencies.AddRange(dependencies.Select(d => FindProject(d)));
			}

			//BuildConfiguration.Initialize(_ConfigurationPlatforms, _AllProjects);
		}

		static readonly Regex reKeyValue = new Regex(@"^ \s* (?<key>[^=]+?) \s* = \s* (?<value>.*?) \s* $", CommonRegexOptions | RegexOptions.IgnorePatternWhitespace);
		void ParseRootKeyValue(SolutionStreamReader reader, Match m) {
			var line	= m.Value;
			var key		= m.Groups["key"].Value;
			var value	= m.Groups["value"].Value;

			switch (key) {
			case "VisualStudioVersion":			VisualStudioVersion			= value; break;
			case "MinimumVisualStudioVersion":	MinimumVisualStudioVersion	= value; break;
			default:							Warning(reader, "Unrecognized root key: {0}", key); break;
			}
		}
	}
}
