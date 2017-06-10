using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleProgram.TestNamespace
{
	public class TestClass:SuperClass
	{
		private string message;
		private int unused;
		public TestClass():base()
		{

		}

		public TestClass(string msg):base()
		{
			this.test = msg;
			this.message = msg;
		}

		public string GetMessage()
		{
			return this.message;
		}

		public string GetMessage(bool uppercase)
		{
			if (uppercase)
			{
				return this.message.ToUpper();
			}
			return this.message;
		}
	}
}
