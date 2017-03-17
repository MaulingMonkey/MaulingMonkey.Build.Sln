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
using System.IO;
using System.Linq;
using System.Reflection;

namespace MaulingMonkey.Build.Sln.Example {
	static partial class Program {
		static readonly string Documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

		static readonly string[] KnownProjectLocations = new[] {
			// Not 100% certain which of these actually exist.  Consider verifying/culling if you have one of these installed.
			Path.Combine(Documents, "Visual Studio 2002"),
			Path.Combine(Documents, "Visual Studio 2003"),
			Path.Combine(Documents, "Visual Studio 2005"),
			Path.Combine(Documents, "Visual Studio 2008"), // Recalled...?
			Path.Combine(Documents, "Visual Studio 2010"),
			Path.Combine(Documents, "Visual Studio 2012"),
			Path.Combine(Documents, "Visual Studio 2013"),
			Path.Combine(Documents, "Visual Studio 2015"), // Verified
			Path.Combine(Documents, "Visual Studio 2017"),

			// Author's perrogative.
			@"I:\home\projects",

			// Hey, let's look at ourselves.
			Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),@"..\..\.."),
		};

		static void Main(string[] args) {
#if DEBUG
			Solution.OnWarning += Solution_OnWarning;
#endif
			if (args.Length == 0) {
				foreach (var root in KnownProjectLocations) {
					if (!Directory.Exists(root)) continue;
					DumpSlnDirRecursive(root);
				}
			} else if (args.Length == 1) {
				var path = args[0];
				if		(File.Exists(path))			DumpSln(path); // Assume it's a .sln
				else if	(Directory.Exists(path))	DumpSlnDirRecursive(path); // Assume it's a directory with .sln s
				else {
					WriteLine(ConsoleColor.Black, ConsoleColor.Red, "Path does not exist: \"{0}\"", path);
					Environment.Exit(1);
				}
			} else {
				WriteLine(ConsoleColor.Black, ConsoleColor.Red, "Expected at most one argument");
				Environment.Exit(1);
			}
		}

		static void DumpSlnDirRecursive(string root) {
			WriteHeaderLine("==== Enumerating {0}\\... ====", root.TrimEnd('\\'));
			foreach (var slnPath in Directory.EnumerateFiles(root, "*.sln", SearchOption.AllDirectories)) DumpSln(slnPath);
			WriteLine();
			WriteLine();
			WriteLine();
		}

		static void DumpSln(string slnPath) {
			SinkSolutionParseExceptions(()=>{
				WriteHeaderLine("==== {0} ====", slnPath);
				var sln = Solution.ParseFrom(slnPath);

				WriteLine("Configs:           {0}", sln.Configurations.FirstOrDefault() ?? "N/A");
				foreach (var config in sln.Configurations.Skip(1)) WriteLine("                   {0}", config);
				WriteLine();

				WriteLine("Platforms:         {0}", sln.Platforms.FirstOrDefault() ?? "N/A (VS2003 or earlier solution?)");
				foreach (var platform in sln.Platforms.Skip(1)) WriteLine("                   {0}", platform);
				WriteLine();

				WriteLine("Config|Platforms:  {0}", sln.ConfigurationPlatforms.Select(p=>p.Name).FirstOrDefault() ?? "N/A");
				foreach (var configPlatform in sln.ConfigurationPlatforms.Skip(1)) WriteLine("                   {0}", configPlatform.Name);
				WriteLine();

				var maxProjectName = sln.AllProjects.Max(p => p.Name.Length);
				foreach (var project in sln.AllProjects) {
					WriteTable(project.Name.PadRight(maxProjectName, ' '), sln.Configurations.ToArray(), sln.Platforms.Any() ? sln.Platforms.ToArray() : new string[] { null }, (config, platform) => {
						var scp = new Solution.ConfigurationPlatform(config, platform);
						var bc = sln.BuildConfiguration[scp, project];

						return	(bc.Build ? "B" : "_")
								+ (bc.Deploy ? "D" : "_")
								+ " "
								+ bc.ActiveCfg ?? "...";
					});
					WriteLine();
				}


				WriteLine("Items:");
				foreach (var item in sln) WriteItem("  ", "  ", sln, item);
				WriteLine();
				WriteLine();
			});
		}

		[DebuggerHidden] static void SinkSolutionParseExceptions(Action a) {
			try { a(); }
			catch (SolutionParseException e) when (Solution_OnHandleException(e)) { }
		}

		[DebuggerHidden] static void Solution_OnWarning(Solution.WarningEvent we) {
			WriteLine(ConsoleColor.Black, ConsoleColor.Yellow, "Warning: {0}", we.Message);
			if (Debugger.IsAttached) Debugger.Break();
		}

		[DebuggerHidden] static bool Solution_OnHandleException(Exception e) {
			WriteLine(ConsoleColor.Black, ConsoleColor.Red, "{0}: {1}", e.GetType().Name, e.Message);
			if (Debugger.IsAttached) Debugger.Break();
			return true;
		}

		static void WriteItem(string thisIndent, string subsequentIndent, Solution sln, Solution.IItem item) {
			WriteItem(thisIndent, subsequentIndent, sln, (dynamic)item);
		}

		static void WriteItem(string thisIndent, string subsequentIndent, Solution sln, Solution.Folder folder) {
			WriteLine("{0} [+] {1}", thisIndent, folder.Name);
			var lastChild = folder.LastOrDefault();
			foreach (var child in folder) {
				var isLastChild = child == lastChild;
				WriteItem(subsequentIndent+(isLastChild?"  `-":"  |-"), subsequentIndent+(isLastChild?"    ":"  | "), sln, child);
			}
		}

		static void WriteItem(string thisIndent, string subsequentIndent, Solution sln, Solution.Project project) {
			var langTag
				= project.TypeGuid == WellKnownProjectTypes.CsProj	? "[C# ]"
				: project.TypeGuid == WellKnownProjectTypes.VcProj	? "[C++]"
				: project.TypeGuid == WellKnownProjectTypes.VcxProj	? "[C++]"
				: "[ ? ]";

			WriteLine("{0} {1} {2}", thisIndent, langTag, project.Name);
			foreach (var dep in project.Dependencies) {
				WriteLine("{0}   (depends on {1})", subsequentIndent, dep.Name);
			}
		}

		static void WriteItem(string thisIndent, string subsequentIndent, Solution sln, Solution.File file) {
			WriteLine("{0} {1}", thisIndent, file.Name);
		}
	}
}
