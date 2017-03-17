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
using System.IO;

namespace MaulingMonkey.Build.Sln {
	/// <summary>
	/// Just an internal equivalent to StreamReader that adds line number / last line read tracking info for debug display.
	/// </summary>
	class SolutionStreamReader : IDisposable {
		readonly StreamReader	StreamReader;

		// Debug info state
		public string			Path		{ get; internal set; }
		public int				LineNo		{ get; internal set; }
		public string			LastLine	{ get; internal set; }

		// Temporary solution parsing state that doesn't fit well elsewhere.
		readonly Dictionary<Guid,HashSet<Guid>> ProjectDependencies = new Dictionary<Guid,HashSet<Guid>>(); // These cannot immediately be resolved into Project references, so store and defer resolving them.
		internal HashSet<Guid> DependenciesFor(Solution.Project project) { return DependenciesFor(project.Guid); }
		internal HashSet<Guid> DependenciesFor(Guid project) {
			HashSet<Guid> guids;
			if (!ProjectDependencies.TryGetValue(project, out guids)) {
				guids = new HashSet<Guid>();
				ProjectDependencies.Add(project, guids);
			}
			return guids;
		}

		public SolutionStreamReader(string path)	{ StreamReader = new StreamReader(path); Path = path; }
		public SolutionStreamReader(Stream stream)	{ StreamReader = new StreamReader(stream); }
		public void Dispose()						{ using (StreamReader) { } }

		public string ReadLine() {
			var l = StreamReader.ReadLine();
			++LineNo;
			LastLine = l;
			return l;
		}
	}
}
