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
using System.IO;
using System.Security;

namespace MaulingMonkey.Build.Sln.Example {
	static partial class Program {
		static void GetColors(out ConsoleColor fore, out ConsoleColor back) {
			fore = ConsoleColor.White;
			back = ConsoleColor.Black;

			try {
				fore = Console.ForegroundColor;
				back = Console.BackgroundColor;
			}
			catch (NotSupportedException) { } // Piped output?
			catch (IOException			) { } // Piped output?
			catch (SecurityException	) { } // Piped output?
		}

		static void SetColors(ConsoleColor fore, ConsoleColor back) {
			try {
				Console.ForegroundColor = fore;
				Console.BackgroundColor = back;
			}
			catch (NotSupportedException) { } // Piped output?
			catch (IOException			) { } // Piped output?
			catch (SecurityException	) { } // Piped output?
		}



		static void Write(string format, params string[] args) {
			try { Console.Write(format, args); }
			catch (IOException) { } // Broken pipe?
		}

		static void Write(ConsoleColor fore, ConsoleColor back, string format, params string[] args) {
			ConsoleColor ofore, oback;
			GetColors(out ofore, out oback);
			SetColors(fore, back);
			Write(format, args);
			SetColors(ofore, oback);
		}



		static void WriteLine() {
			try { Console.WriteLine(); }
			catch (IOException) { } // Broken pipe?
		}

		static void WriteLine(string format, params string[] args) {
			try { Console.WriteLine(format, args); }
			catch (IOException) { } // Broken pipe?
		}

		static void WriteLine(ConsoleColor fore, ConsoleColor back, string format, params string[] args) {
			Write(fore, back, format, args);
			WriteLine();
		}



		static void WriteHeaderLine(string format, params string[] args) {
			WriteLine(ConsoleColor.Black, ConsoleColor.White, format, args);
		}

		static void WriteTable(string[,] data) {
			var cols = data.GetLength(0);
			var rows = data.GetLength(1);

			var colWidths	= new int[cols];
			//var rowHeights	= new int[rows];

			for (var row=0; row<rows; ++row)
			for (var col=0; col<cols; ++col)
			{
				var w = (data[col,row] ?? "").Length;
				if (w > colWidths[col]) colWidths[col] = w;
			}

			for (var row=0; row<rows; ++row) {
				for (var col=0; col<cols; ++col) {
					var w = colWidths[col];
					if ((col == 0) && (row == 0)) {
						Write(ConsoleColor.Black, ConsoleColor.DarkCyan, " {0} ", (data[col,row] ?? "").PadRight(w, ' '));
					} else if ((col == 0) != (row == 0)) {
						Write(ConsoleColor.Black, ConsoleColor.Gray, " {0} ", (data[col,row] ?? "").PadRight(w, ' '));
					} else {
						Write(" {0} ", (data[col,row] ?? "").PadRight(w, ' '));
					}
				}
				WriteLine();
			}
		}

		delegate string TableCellLookup(string column, string row);
		static void WriteTable(string title, string[] cols, string[] rows, TableCellLookup lookup) {
			var cells = new string[cols.Length+1, rows.Length+1];
			cells[0,0] = title ?? "";
			for (var row=0; row<rows.Length; ++row) cells[0,row+1] = rows[row];
			for (var col=0; col<cols.Length; ++col) cells[col+1,0] = cols[col];
			for (var row=0; row<rows.Length; ++row) for (var col=0; col<cols.Length; ++col) cells[col+1,row+1] = lookup(cols[col], rows[row]) ?? "";
			WriteTable(cells);
		}
	}
}
