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
		public struct WarningEvent {
			public string SlnLine;
			public string Message;
		}
#if !DEBUG
		[Obsolete("Warnings are not emitted by the Release version of MaulingMonkey.Build.Sln")]
#endif
		public static event Action<WarningEvent> OnWarning = (we) => { };

		[DebuggerHidden] [Conditional("DEBUG")] static void Warning(WarningEvent we)															{ OnWarning(we); }
		[DebuggerHidden] [Conditional("DEBUG")] static void Warning(SolutionStreamReader r, string warningFormat)								{ OnWarning(new WarningEvent() { SlnLine = r.LastLine, Message = warningFormat }); }
		[DebuggerHidden] [Conditional("DEBUG")] static void Warning(SolutionStreamReader r, string warningFormat, params string[] warningArgs)	{ OnWarning(new WarningEvent() { SlnLine = r.LastLine, Message = string.Format(warningFormat, warningArgs) }); }

		[DebuggerHidden] [Conditional("DEBUG")] static void WarningIf(bool condition, WarningEvent we)																{ if (condition) OnWarning(we); }
		[DebuggerHidden] [Conditional("DEBUG")] static void WarningIf(bool condition, SolutionStreamReader r, string warningFormat)									{ if (condition) OnWarning(new WarningEvent() { SlnLine = r.LastLine, Message = warningFormat }); }
		[DebuggerHidden] [Conditional("DEBUG")] static void WarningIf(bool condition, SolutionStreamReader r, string warningFormat, params string[] warningArgs)	{ if (condition) OnWarning(new WarningEvent() { SlnLine = r.LastLine, Message = string.Format(warningFormat, warningArgs) }); }
	}
}
