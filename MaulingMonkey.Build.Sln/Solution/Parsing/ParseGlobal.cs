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

using System.Text.RegularExpressions;

namespace MaulingMonkey.Build.Sln {
	partial class Solution {
		static readonly Regex reGlobal		= new Regex(@"^  \s*  Global     \s*  $", CommonRegexOptions | RegexOptions.IgnorePatternWhitespace);
		static readonly Regex reEndGlobal	= new Regex(@"^  \s*  EndGlobal  \s*  $", CommonRegexOptions | RegexOptions.IgnorePatternWhitespace);

		/// <summary>
		/// Parse a Global...EndGlobal region.
		/// </summary>
		/// <param name="global">A reGlobal match.</param>
		void ParseGlobal(SolutionStreamReader reader, Match global) {
			for (;;) {
				Match m;
				var line = reader.ReadLine();

				if		(line == null)								throw new SolutionParseException(reader, "Unexpected EOF within Global");
				else if	(string.IsNullOrWhiteSpace(line))			continue;
				else if	((m=reEndGlobal		.Match(line)).Success)	break;
				else if	((m=reGlobalSection	.Match(line)).Success)	ParseGlobalSection(reader, m);
				else												throw new SolutionParseException(reader, "Expected GlobalSection or EndGlobal within Global");
			}
		}
	}
}
