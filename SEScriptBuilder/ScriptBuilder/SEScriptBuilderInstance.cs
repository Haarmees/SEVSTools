using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using SEScriptBuilder.ScriptBuilder.Analyzer;
using SEScriptBuilder.ScriptBuilder.TaggedSyntax;

namespace SEScriptBuilder.ScriptBuilder
{
	public class SESCriptBuilderInstance
	{
		public const string EntryName = "Program.cs";
		public const string MainFnName = "Main";
		public const string ProgramName = "Program";
		public const string SaveFnName = "Save";
		public const string DebugName = "DEBUG";
		
		public SESCriptBuilderInstance() {
			
		}

		public SyntaxTree BuildProject(Project project, bool prod = false)
		{
			Console.Out.WriteLine("Analyzing source code");
			ProjectAnalyzer analyzer = new ProjectAnalyzer(project);
			analyzer.analyze();

			SyntaxTree result = null;
			if (analyzer.MainTree != null)
			{
				Console.Out.WriteLine("Renaming main program class");
				SyntaxTree renamedTree = this.renameProgramClass(analyzer.MainTree);
				if (renamedTree != null) {
					result = renamedTree;
					if (analyzer.Dependencies != null && analyzer.Dependencies.Count() > 0)
						{
							Console.Out.WriteLine("Merging dependencies");
							SyntaxTree lib = this.mergeClasses(analyzer.Dependencies);
							if (lib != null)
							{
								Console.Out.WriteLine("Injecting dependencies");
								result = this.injectClasses(result, lib);
							}
						}
					}
				
			}

			if (result != null)
			{
				if (prod)
				{
					result = this.getProductionCode(result);
				}
				else
				{
					result = this.getDebugCode(result);
				}
				
			}

			return result;
		}


		private SyntaxTree getDebugCode(SyntaxTree tree)
		{
			SyntaxTree newTree = null;
			SyntaxTriviaList spaceTrivias = new SyntaxTriviaList();
			SyntaxTrivia space = SyntaxFactory.Space;
			spaceTrivias = spaceTrivias.Add(space);

			SyntaxTriviaList endLineTrivias = new SyntaxTriviaList();
			SyntaxTrivia endline = SyntaxFactory.CarriageReturnLineFeed;
			endLineTrivias = endLineTrivias.Add(endline);

			IdentifierNameSyntax debugIdentifier = SyntaxFactory.IdentifierName(SESCriptBuilderInstance.DebugName).WithLeadingTrivia(spaceTrivias);

			IfDirectiveTriviaSyntax ifOpen = SyntaxFactory.IfDirectiveTrivia(debugIdentifier, false, false, false).WithTrailingTrivia(endLineTrivias);
			SyntaxTrivia ifOpenTrivia = SyntaxFactory.Trivia(ifOpen);
			SyntaxTriviaList ifOpenList = new SyntaxTriviaList();
			ifOpenList = ifOpenList.Add(ifOpenTrivia);

			EndIfDirectiveTriviaSyntax ifClose = SyntaxFactory.EndIfDirectiveTrivia(false).WithTrailingTrivia(endLineTrivias).WithLeadingTrivia(endLineTrivias);
			SyntaxTrivia ifCloseTrivia = SyntaxFactory.Trivia(ifClose);
			SyntaxTriviaList ifCloseList = new SyntaxTriviaList();
			ifCloseList = ifCloseList.Add(ifCloseTrivia);

			CompilationUnitSyntax root = tree.GetCompilationUnitRoot();

			// add begin and end ifs
			root = root.WithLeadingTrivia(ifOpenList);
			root = root.WithTrailingTrivia(ifCloseList);

			ClassDeclarationSyntax mainClass = this.getTopLevelClasses(root.SyntaxTree).First();

			SyntaxToken token = mainClass.OpenBraceToken;
			SyntaxToken newToken = token.WithTrailingTrivia(ifCloseList);

			root = root.ReplaceToken(token, newToken);

			mainClass = this.getTopLevelClasses(root.SyntaxTree).First();

			token = mainClass.CloseBraceToken;
			newToken = token.WithLeadingTrivia(ifOpenList);

			root = root.ReplaceToken(token, newToken);

			newTree = root.SyntaxTree;
			return newTree;
		}

		private SyntaxTree getProductionCode(SyntaxTree tree)
		{
			SyntaxTree newTree = null;
			CompilationUnitSyntax oldComp = tree.GetCompilationUnitRoot();
			ClassDeclarationSyntax mainClass = this.getTopLevelClasses(tree).First();
			if (mainClass != null)
			{
				CompilationUnitSyntax newComp = SyntaxFactory.CompilationUnit().WithMembers(mainClass.Members);
				newTree = newComp.SyntaxTree;
			}
			return newTree;
		}

		private SyntaxTree renameProgramClass(SyntaxTree tree)
		{
			ClassDeclarationSyntax mainClass = this.getTopLevelClasses(tree).First();
			SyntaxTree newTree = null;
			if (mainClass != null)
			{
				SyntaxToken newIdentifier = SyntaxFactory.Identifier(SESCriptBuilderInstance.ProgramName);
				SyntaxList<MemberDeclarationSyntax> members = mainClass.Members;
				if (members != null && members.Count() > 0)
				{
					SyntaxList<MemberDeclarationSyntax> newMembers = new SyntaxList<MemberDeclarationSyntax>();
					foreach (MemberDeclarationSyntax member in members)
					{
						MemberDeclarationSyntax newMember = member;
						if (member.GetType() == typeof(ConstructorDeclarationSyntax))
						{
							ConstructorDeclarationSyntax cons = member as ConstructorDeclarationSyntax;
							SyntaxToken identifier = cons.Identifier;
							if (identifier != null)
							{
								
								newMember = SyntaxFactory.ConstructorDeclaration(cons.AttributeLists, cons.Modifiers, newIdentifier, cons.ParameterList, cons.Initializer, cons.Body, cons.ExpressionBody, cons.SemicolonToken);
							}
							
						}
						newMembers = newMembers.Add(newMember);
					}

					members = newMembers;
				}

				ClassDeclarationSyntax newClass = mainClass.WithIdentifier(newIdentifier).WithMembers(members);

				if (newClass != null)
				{
					IEnumerable<NamespaceDeclarationSyntax> namespaces = tree.GetRootAsync().Result.DescendantNodes().OfType<NamespaceDeclarationSyntax>();
					CompilationUnitSyntax newComp = null;
					CompilationUnitSyntax oldComp = tree.GetCompilationUnitRoot();
					SyntaxList<MemberDeclarationSyntax> newClassMembers = new SyntaxList<MemberDeclarationSyntax>();
					newClassMembers = newClassMembers.Add(newClass);
					if (namespaces != null && namespaces.Count() > 0)
					{
						NamespaceDeclarationSyntax ns = namespaces.First();
						if (ns != null)
						{
							NamespaceDeclarationSyntax newNs = ns.WithMembers(newClassMembers);
							SyntaxList<MemberDeclarationSyntax> newNsMembers = new SyntaxList<MemberDeclarationSyntax>();
							newNsMembers = newNsMembers.Add(newNs);
							newComp = oldComp.WithMembers(newNsMembers);
						}
					}
					else
					{

						newComp = oldComp.WithMembers(newClassMembers);
					}

					if (newComp != null) {
						newTree = newComp.SyntaxTree;
					}
				}
			}
			return newTree;
		}

		private SyntaxTree mergeClasses(List<SyntaxTree> trees) {

			SyntaxTree result = null;
			if (trees != null && trees.Count() > 0)
			{
				List<ClassDeclarationSyntax> classes = new List<ClassDeclarationSyntax>();
				foreach (SyntaxTree tree in trees)
				{
					IEnumerable<ClassDeclarationSyntax> cs = this.getTopLevelClasses(tree);
					if (cs != null && cs.Count() > 0)
					{
						classes.AddRange(cs);
					}
				}

				if (classes.Count() > 0)
				{
					SyntaxList<MemberDeclarationSyntax> members = new SyntaxList<MemberDeclarationSyntax>();
					members = members.AddRange(classes);
					SyntaxNode root = SyntaxFactory.CompilationUnit().WithMembers(members);
					if (root != null)
					{
						result = root.SyntaxTree;
					}
				}
			}
			
			return result;
		}

		private SyntaxTree injectClasses(SyntaxTree program, SyntaxTree lib)
		{
			SyntaxTree result = null;
			ClassDeclarationSyntax mainClass = this.getTopLevelClasses(program).First();
			IEnumerable<ClassDeclarationSyntax> libClasses = this.getTopLevelClasses(lib);

			IEnumerable<SyntaxNode> subNodes = mainClass.DescendantNodes().Where(n => n.Parent == mainClass);
			if (subNodes.Count() > 0)
			{
				SyntaxNode lastNode = subNodes.Last();

				SyntaxNode newNode = program.GetRootAsync().Result.InsertNodesAfter(lastNode, libClasses);
				result = newNode.SyntaxTree;
			}
			return result;
		}

		private IEnumerable<ClassDeclarationSyntax> getTopLevelClasses(SyntaxTree tree) {
			IEnumerable<ClassDeclarationSyntax> classes = tree.GetRootAsync().Result.DescendantNodes().OfType<ClassDeclarationSyntax>().Where(c => c.Parent.GetType() == typeof(NamespaceDeclarationSyntax) || c.Parent.GetType() == typeof(CompilationUnitSyntax));
			return classes;
		}
	}
}
