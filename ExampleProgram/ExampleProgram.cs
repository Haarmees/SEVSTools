
using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using VRageMath;
using VRage.Game;
using VRage.Collections;
using Sandbox.ModAPI.Ingame;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using Sandbox.Game.EntityComponents;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using ExampleProgram.TestNamespace;

namespace SpaceEngineers
{
	public sealed class ExampleProgram : MyGridProgram
	{

		TestClass testclass;
		const string unused = "test";
		//=======================================================================
		//////////////////////////BEGIN//////////////////////////////////////////
		//=======================================================================

		public ExampleProgram()
		{
			this.testclass = new TestClass("Hello world!");
			this.TestMethod();
			this.TestMethod2();
			// The constructor, called only once every session and
			// always before any other method is called. Use it to
			// initialize your script.
		}

		public void Main(string args)
		{
			string msg = this.testclass.GetMessage(true);
			// The main entry point of the script, invoked every time
			// one of the programmable block's Run actions are invoked.

			// The method itself is required, but the argument above
			// can be removed if not needed.
		}

		public void Save()
		{
			// Called when the program needs to save its state. Use
			// this method to save your state to the Storage field
			// or some other means.

			// This method is optional and can be removed if not
			// needed.
		}

		//=======================================================================
		//////////////////////////END////////////////////////////////////////////
		//=======================================================================

		public void TestMethod() {
			int[] items = new int[] { 0, 1, 2, 3 };
			foreach (int item in items)
			{
				int doubled = item * 2;
			}
		}

		public string TestMethod2()
		{
			TestClass[] testClasses = new TestClass[] { new TestClass("hello"), new TestClass("world") };
			string msg = "";
			foreach (TestClass testclass in testClasses)
			{
				msg += testclass.GetMessage();
			}
			return msg;
		}


	}
}
