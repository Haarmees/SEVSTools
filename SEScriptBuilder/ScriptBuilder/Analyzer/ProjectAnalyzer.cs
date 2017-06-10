using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SEScriptBuilder.ScriptBuilder.TaggedSyntax;

namespace SEScriptBuilder.ScriptBuilder.Analyzer
{
	class ProjectAnalyzer
	{
		const string DefaultProgramBaseType = "MyGridProgram";
		const string MainFunctionName = "Main";
		const string SaveFunctionName = "Save";

		private List<SyntaxTree> dependencies;
		public List<SyntaxTree> Dependencies { get { return this.dependencies; } }
		private SyntaxTree mainTree;
		public SyntaxTree MainTree { get { return this.mainTree; } }
		private Project project;

		public ProjectAnalyzer(Project project)
		{
			this.project = project;
		}

		public TaggedSyntaxLibrary analyze(Project project = null)
		{

			TaggedSyntaxLibrary lib = new TaggedSyntaxLibrary();
			if (project == null)
			{
				project = this.project;
			}
			if (project != null)
			{
				SyntaxTree program = this.findMainTree(project);
				if (program != null)
				{
					List<SyntaxNode> entryNodes = this.getEntryNodes(program);
					ScriptAnalyzer analyzer = this.createScriptAnalyzer(project);
					if (entryNodes != null && entryNodes.Count > 0)
					{

						foreach (SyntaxNode entryNode in entryNodes)
						{
							analyzer.AnalyzeNode(entryNode, lib);
						}

						if (lib.TaggedSyntaxTrees.Count() > 0)
						{
							
							TaggedSyntaxTree mainDTree = lib.GetTreeFromNode(entryNodes.First());
							IEnumerable<TaggedSyntaxTree> dTrees = lib.TaggedSyntaxTrees.Where(t=> t != mainDTree);
							if (mainDTree != null) {
								this.mainTree = mainDTree.GetSyntaxTree();
							}
							if (dTrees != null && dTrees.Count()>0) {
								this.dependencies = this.parseTaggedSyntaxTrees(dTrees);
							}
							
						}
					}
				}
			}

			

			// analyze three methods
			return lib;
		}

		

		private SyntaxTree findMainTree(Project project) {

			SyntaxTree result = null;
			if (project != null && project.Documents != null && project.Documents.Count() > 0)
			{
				foreach (Document doc in project.Documents)
				{
					SyntaxTree tree = doc.GetSyntaxTreeAsync().Result;
					if (tree != null)
					{
						ClassDeclarationSyntax mainClass = ProjectAnalyzerHelper.GetMainClass(tree);
						if (mainClass != null)
						{
							BaseListSyntax baseList = mainClass.BaseList;
							if (baseList != null)
							{
								SeparatedSyntaxList<BaseTypeSyntax> types = baseList.Types;
								if (types != null && types.Count() > 0)
								{
									BaseTypeSyntax baseType = types.First();
									if (baseType != null)
									{
										if (baseType.ToString() == ProjectAnalyzer.DefaultProgramBaseType)
										{
											result = doc.GetSyntaxTreeAsync().Result;
											Console.Out.WriteLine("Found program file: " + doc.FilePath);
											break;
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		private List<SyntaxNode> getEntryNodes(SyntaxTree mainTree)
		{
			List<SyntaxNode> nodes = new List<SyntaxNode>();
			if (mainTree != null)
			{
				ClassDeclarationSyntax mainClass = ProjectAnalyzerHelper.GetMainClass(mainTree);
				if (mainClass != null)
				{
					ConstructorDeclarationSyntax cons = ProjectAnalyzerHelper.GetConstructor(mainClass);
					MethodDeclarationSyntax main = ProjectAnalyzerHelper.GetMethodDeclaration(mainClass, ProjectAnalyzer.MainFunctionName);
					MethodDeclarationSyntax save = ProjectAnalyzerHelper.GetMethodDeclaration(mainClass, ProjectAnalyzer.SaveFunctionName);
					if (cons != null)
					{
						nodes.Add(cons);
					}
					if (main != null)
					{
						nodes.Add(main);
					}
					if (save != null)
					{
						nodes.Add(save);
					}
				}
			}
			return nodes;
		}

		private List<SyntaxTree> parseTaggedSyntaxTrees(IEnumerable<TaggedSyntaxTree> taggedTrees)
		{
			List<SyntaxTree> trees = new List<SyntaxTree>();
			if (taggedTrees != null && taggedTrees.Count() > 0)
			{
				foreach (TaggedSyntaxTree ttree in taggedTrees)
				{
					trees.Add(ttree.GetSyntaxTree());
				}
			}

			return trees;
		}

		private ScriptAnalyzer createScriptAnalyzer(Project project)
		{
			ScriptAnalyzer result = null;
			List<Compilation> comps = this.getCompilations(project);
			if (comps != null && comps.Count() > 0)
			{
				ScriptAnalyzerSymbolHelper sHelper = new ScriptAnalyzerSymbolHelper(comps.ToArray());
				ScriptAnalyzerResourceHelper rHelper = new ScriptAnalyzerResourceHelper(sHelper);
				result = new ScriptAnalyzer(rHelper);
			}

			return result;
		}

		private List<Compilation> getCompilations(Project project)
		{
			List<Compilation> comps = new List<Compilation>();
			Compilation mainComp = project.GetCompilationAsync().Result;
			if (mainComp != null)
			{
				comps.Add(mainComp);
			}
			if (project.ProjectReferences != null && project.ProjectReferences.Count() > 0)
			{
				foreach (ProjectReference projRef in project.ProjectReferences)
				{
					Project refProject = project.Solution.GetProject(projRef.ProjectId);
					if (refProject != null)
					{
						Compilation comp = refProject.GetCompilationAsync().Result;
						if (comp != null)
						{
							comps.Add(comp);
						}
					}
				}
			}

			return comps;
		}
	}
}
