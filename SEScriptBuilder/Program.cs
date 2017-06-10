using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManyConsole;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;


namespace SEScriptBuilder
{
	public class Program
	{
		static readonly string DefaultCommand = "Build";
		static readonly string ProjectOptionStr = "-p=";

		static public int Main(string[] args)
		{
			int exitCode = 1;
			Console.WriteLine("STARTING");

				IEnumerable<ConsoleCommand> commands = GetCommands();
				exitCode = ConsoleCommandDispatcher.DispatchCommand(commands, args, Console.Out);
	
			return exitCode;
		}

		public static IEnumerable<ConsoleCommand> GetCommands()
		{
			return ConsoleCommandDispatcher.FindCommandsInSameAssemblyAs(typeof(Program));
		}

		
	}
}
