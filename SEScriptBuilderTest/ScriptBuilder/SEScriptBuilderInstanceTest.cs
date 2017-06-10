using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SEScriptBuilder.ScriptBuilder;

namespace SEScriptBuilderTest.ScriptBuilder
{
	[TestClass]
	public class SEScriptBuilderInstanceTest
	{
		[TestMethod, TestCategory("ScriptBuilder.SEScriptBuilderInstance")]
		public void TestMethod1()
		{
			MSBuildWorkspace ws = MSBuildWorkspace.Create();
			Project project = ws.OpenProjectAsync(@"C:\Users\Jochem\Documents\Visual Studio 2017\Projects\SEVSTools\ExampleProgram\ExampleProgram.csproj").Result;
			SESCriptBuilderInstance builder = new SESCriptBuilderInstance();

			builder.BuildProject(project);
		}
	}
}
