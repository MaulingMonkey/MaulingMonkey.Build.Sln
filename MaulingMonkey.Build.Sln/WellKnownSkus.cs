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
	/// A collection of well known <seealso cref="Solution.SkuHint"/> values.
	/// <para>Not comprehensive:  New versions of Visual Studio will continue to be released.
	/// These generally correspond to the leading '# Visual Studio XXXX' comment in .sln files, although VS2002/2003 are inferred from FormatVersion instead.</para>
	/// </summary>
	public static class WellKnownSkus {
		const string sUnverifiedSku = "This SKU identifier has not been verified.  If you have a .sln created with this SKU, please verify if the string is correct - or what it should be - and open an issue to remove this tag.";

		// TODO: VS6 and earlier workspaces?

		// https://en.wikipedia.org/wiki/Microsoft_Visual_Studio
		// https://en.wikipedia.org/wiki/PlayStation_Mobile / PlayStation Suite - Observed pretending to be VS2010
		// https://en.wikipedia.org/wiki/MonoDevelop - Observed pretending to be VS2010

		// I believe these cover all "combined" skus (e.g. Community, Professional, Ultimate, Enterprise, etc.)
		public const string VisualStudio2002 = "Visual Studio 2002"; // NOTE: There's no corresponding SkuHint for these!
		public const string VisualStudio2003 = "Visual Studio 2003"; // NOTE: There's no corresponding SkuHint for these!
		public const string VisualStudio2005 = "Visual Studio 2005";
		public const string VisualStudio2008 = "Visual Studio 2008";
		public const string VisualStudio2010 = "Visual Studio 2010";
		public const string VisualStudio2012 = "Visual Studio 2012";
		public const string VisualStudio2013 = "Visual Studio 2013";
		public const string VisualStudio2015 = "Visual Studio 14";
		public const string VisualStudio2017 = "Visual Studio 15";

		// https://en.wikipedia.org/wiki/Microsoft_Visual_Studio_Express#2005.E2.80.932010_products

		public const string VisualCSharpExpress2005		= "Visual C# Express 2005";
		public const string VisualCSharpExpress2008		= "Visual C# Express 2008";
		public const string VisualCSharpExpress2010		= "Visual C# Express 2010";

		public const string VisualCPlusPlusExpress2005	= "Visual C++ Express 2005";
		public const string VisualCPlusPlusExpress2008	= "Visual C++ Express 2008";
		public const string VisualCPlusPlusExpress2010	= "Visual C++ Express 2010";

		// TODO: 2005-2010 Web Developer Express skus?  Basic?  Windows Phone?

		// https://en.wikipedia.org/wiki/Microsoft_Visual_Studio_Express#2012-2013_products

									public const string VisualStudioExpress2012ForWeb				= "Visual Studio Express 2012 for Web";
		[Obsolete(sUnverifiedSku)]	public const string VisualStudioExpress2012ForWindows8			= "Visual Studio Express 2012 for Windows 8";
									public const string VisualStudioExpress2012ForWindowsDesktop	= "Visual Studio Express 2012 for Windows Desktop";
		[Obsolete(sUnverifiedSku)]	public const string VisualStudioExpress2012ForWindowsPhone		= "Visual Studio Express 2012 for Windows Phone";

		[Obsolete(sUnverifiedSku)]	public const string VisualStudioExpress2013ForWeb				= "Visual Studio Express 2013 for Web";
		[Obsolete(sUnverifiedSku)]	public const string VisualStudioExpress2013ForWindows81			= "Visual Studio Express 2013 for Windows 8.1";
									public const string VisualStudioExpress2013ForWindowsDesktop	= "Visual Studio Express 2013 for Windows Desktop";
		[Obsolete(sUnverifiedSku)]	public const string VisualStudioExpress2013ForWindowsPhone		= "Visual Studio Express 2013 for Windows Phone";

		// https://en.wikipedia.org/wiki/Microsoft_Visual_Studio_Express#2015_products

		[Obsolete(sUnverifiedSku)]	public const string VisualStudioExpress2015ForWeb				= "Visual Studio Express 2015 for Web";
		[Obsolete(sUnverifiedSku)]	public const string VisualStudioExpress2015ForWindows10			= "Visual Studio Express 2015 for Windows 10";
		[Obsolete(sUnverifiedSku)]	public const string VisualStudioExpress2015ForWindowsDesktop	= "Visual Studio Express 2015 for Windows Desktop";

		public static readonly string[] All = new[] {
			VisualStudio2002,
			VisualStudio2003,
			VisualStudio2005,
			VisualStudio2008,
			VisualStudio2010,
			VisualStudio2012,
			VisualStudio2013,
			VisualStudio2015,
			VisualStudio2017,

			VisualCSharpExpress2005,
			VisualCSharpExpress2008,
			VisualCSharpExpress2010,

			VisualCPlusPlusExpress2005,
			VisualCPlusPlusExpress2008,
			VisualCPlusPlusExpress2010,

			VisualStudioExpress2012ForWeb,
			//VisualStudioExpress2012ForWindows8,
			VisualStudioExpress2012ForWindowsDesktop,
			//VisualStudioExpress2012ForWindowsPhone,

			//VisualStudioExpress2013ForWeb,
			//VisualStudioExpress2013ForWindows81,
			VisualStudioExpress2013ForWindowsDesktop,
			//VisualStudioExpress2013ForWindowsPhone,

			//VisualStudioExpress2015ForWeb,
			//VisualStudioExpress2015ForWindows10,
			//VisualStudioExpress2015ForWindowsDesktop,
		};
	}
}
