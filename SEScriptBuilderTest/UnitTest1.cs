using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SEScriptBuilder;

namespace SEScriptBuilderTest
{
	[TestClass]
	public class ProgramTest
	{
		[TestMethod]
		public void MainTest()
		{
			string[] args = { "project.csproj" };
			Program.Main(args);
		}
	}
}
