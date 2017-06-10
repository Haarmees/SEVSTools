using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ManyConsole;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using SEScriptBuilder.ScriptBuilder;

namespace SEScriptBuilder.Commands
{
	class BuildCommand:ConsoleCommand
	{
		public const string DefaultScriptName = "Script.cs";
		public string ProjectPath { get; set; }
		public bool ProductionMode { get; set; }
		public string OutputPath { get; set; }
		private const string ProjectPathPattern = @"^(.*\.(csproj|sln))$";

		public BuildCommand()
		{
			// Register the actual command with a simple (optional) description.
			this.IsCommand("Build", "Build a space engineers script");

			// Add a longer description for the help on that specific command.
			this.HasLongDescription("This can be used to inject all code in a script file, which can be used for the programmable block");

			// Required options/flags, append '=' to obtain the required value.
			this.HasRequiredOption("p|project=", "The full path of either a .csproj or .snl file.", p => this.ProjectPath = p);
			//this.HasOption("prod|production:", "Run in production mode, default is false", prod => this.ProductionMode = prod == null ? true : Convert.ToBoolean(prod));
			this.HasRequiredOption("o|out=", "The outputh path (required).", o => this.OutputPath = o);
			//this.HasOption("prod|production=", "Builds script for production", p => this.ProductionMode = p == null ? true : Convert.ToBoolean(p));
			this.HasOption("prod|production", "Builds script for production", b => this.ProductionMode = true);
		}

		public override int Run(string[] remainingArguments)
		{
			int error = 0;
			Console.Out.WriteLine("Starting build of project: " + this.ProjectPath);
			if (this.ProjectPath != null)
			{
				MSBuildWorkspace ws = MSBuildWorkspace.Create();
				Project project = ws.OpenProjectAsync(this.ProjectPath).Result;
				
				if (project != null)
				{
					Console.Out.WriteLine("Project loaded...");
					SESCriptBuilderInstance builder = new SESCriptBuilderInstance();
					SyntaxTree result = builder.BuildProject(project, this.ProductionMode);
					if (result != null)
					{
						if (!this.ProductionMode)
						{
							string fullpath = System.IO.Path.Combine(this.OutputPath, BuildCommand.DefaultScriptName);

							System.IO.File.WriteAllText(fullpath, result.ToString());
							Console.Out.WriteLine("Finished building: " + fullpath);
							error = 0;
						}
						else
						{
							string foldername = project.Name;
							string fullDirPath = System.IO.Path.Combine(this.OutputPath, foldername);
							System.IO.Directory.CreateDirectory(fullDirPath);
							string fullpath = System.IO.Path.Combine(fullDirPath, BuildCommand.DefaultScriptName);
							System.IO.File.WriteAllText(fullpath, result.ToString());
							Console.Out.WriteLine("Finished building: " + fullpath);
						}
					}
				}
			}



			//Console.Out.WriteLine(this.OutputPath);
			return error;
		}

		public bool IsProjectOrSolutionPath(string arg)
		{
			Regex rgx = new Regex(ProjectPathPattern);
			Match match = rgx.Match(arg);

			return match.Success; 
		}

	}
}
