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

namespace MaulingMonkey.Build.Sln {
	/// <summary>
	/// Some kind of error parsing a .sln (e.g. syntax errors.)
	/// </summary>
	public class SolutionParseException : Exception {
		readonly SolutionStreamReader SolutionStreamReader; // For maximum debug info

		static string FormatMessage(SolutionStreamReader reader, string message) {
			if (reader == null)			return string.Format("unknown(0): {0}", message);
			if (reader.Path == null)	return string.Format("unknown({0}): {1}", reader.LineNo, message);
			return string.Format("{0}({1}): {2}", reader.Path, reader.LineNo, message);
		}

		internal SolutionParseException(SolutionStreamReader reader, string message)
			: base(FormatMessage(reader, message))
		{
			SolutionStreamReader = reader;
		}
	}
}
