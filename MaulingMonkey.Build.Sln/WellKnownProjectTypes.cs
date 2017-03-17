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
	/// A collection of well known <see cref="Solution.Project.TypeGuid"/> values.
	/// <para>Not comprehensive:  New Microsoft and Third Party SDKs will continue to add new project types, possibly under NDA.
	/// These correspond to Project({TypeGuid}) = ... values.</para>
	/// </summary>
	public static class WellKnownProjectTypes {
		public static readonly Guid SolutionFolder		= new Guid("{2150E333-8FDC-42A3-9474-1A3956D46DE8}"); // Not a real project
		public static readonly Guid CsProj				= new Guid("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"); // C# project
		public static readonly Guid VstForUnityCsProj1	= new Guid("{D83AD1EE-EC33-5AAE-302A-07D1E5669F96}"); // Visual Studio Tools for Unity C# project?
		public static readonly Guid VstForUnityCsProj2	= new Guid("{22253d0B-F117-6BB8-3528-A8009E184337}"); // Visual Studio Tools for Unity C# project?
		public static readonly Guid VstForUnityCsProj3	= new Guid("{26AE7496-4466-3C64-9950-CD240B55236F}"); // Visual Studio Tools for Unity C# project?
		public static readonly Guid VstForUnityCsProj4	= new Guid("{F1BD4581-5464-3EEC-D380-3F4772A7B237}"); // Visual Studio Tools for Unity C# project?
		public static readonly Guid VcxProj				= new Guid("{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}"); // C++ project (VS2002-2005)
		public static readonly Guid VcProj				= new Guid("{C2A7C241-C4D1-0129-082B-00508D642E22}"); // C++ project (VS2008+)
		public static readonly Guid RsProj				= new Guid("{78C9907C-C22D-4F8D-B13A-49F213FF1631}"); // Rust ( https://github.com/PistonDevelopers/VisualRust )
		public static readonly Guid JsProj				= new Guid("{262852C6-CD72-467D-83FE-5EEB1973A190}"); // Javascript App Project ( see e.g. https://github.com/popravich/typescript/blob/master/samples/win8/encyclopedia/Encyclopedia.sln )
		public static readonly Guid CosmosBreakpoints	= new Guid("{471EC4BB-E47E-4229-A789-D1F5F83B52D4}"); // Something to do with https://github.com/CosmosOS/Cosmos 's "Breakpoints" project/folder/???

		public static readonly Guid[] All = new[] {
			SolutionFolder,
			CsProj,
			VstForUnityCsProj1,
			VstForUnityCsProj2,
			VstForUnityCsProj3,
			VstForUnityCsProj4,
			VcxProj,
			VcProj,
			RsProj,
			JsProj,
			CosmosBreakpoints,
		};
	}
}
