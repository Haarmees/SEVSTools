using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SEScriptBuilder.ScriptBuilder.Analyzer;
using SEScriptBuilder.ScriptBuilder.TaggedSyntax;

namespace SEScriptBuilderTest.ScriptBuilder.Analyzer
{
	[TestClass]
	public class ScriptAnalyzerTest
	{

		[TestMethod, TestCategory("ScriptBuilder.Analyzer.ScriptAnalyzerTest")]
		public void TestConstructor()
		{
			Compilation comp = CSharpCompilation.Create("test");
			ScriptAnalyzerSymbolHelper sHelper = new ScriptAnalyzerSymbolHelper(new Compilation[] { comp });
			ScriptAnalyzerResourceHelper rHelper = new ScriptAnalyzerResourceHelper(sHelper);
			ScriptAnalyzer analyzer = new ScriptAnalyzer(rHelper);

			Assert.AreEqual(rHelper, analyzer.ResourceHelper, "Resource helper gest set");

		}

		[TestMethod, TestCategory("ScriptBuilder.Analyzer.ScriptAnalyzerTest")]
		public void TestDefaultAnalyzeSubNodes()
		{
			string code1 = @"class TestClass { public void TestMethod( int test = 0; test = 1;){}}";
			SyntaxTree tree1 = CSharpSyntaxTree.ParseText(code1);

			MethodDeclarationSyntax method = tree1.GetRootAsync().Result.DescendantNodes().OfType<MethodDeclarationSyntax>().First();

			List<SyntaxTree> trees1 = new List<SyntaxTree> { tree1 };
			Compilation comp1 = CSharpCompilation.Create("TestCompilation1", trees1);

			ScriptAnalyzer analyzer = this.createAnalyzer(comp1);

			TaggedSyntaxLibrary lib = analyzer.AnalyzeNode(method);

			Assert.IsNotNull(lib, "Library defined");
			Assert.AreEqual(1, lib.TaggedSyntaxTrees.Count(), "Has one tree");

			TaggedSyntaxTree tree = lib.TaggedSyntaxTrees.First();

			IEnumerable<SyntaxNode> nodes = method.DescendantNodesAndSelf();
			
			foreach(SyntaxNode node in nodes) {
				CollectionAssert.Contains(tree.TaggedNodes, node, "SubNode is added to tree");
			}
		}

		[TestMethod, TestCategory("ScriptBuilder.Analyzer.ScriptAnalyzerTest")]
		public void TestAnalyzeNameSpace()
		{
			string code1 = @"namespace Test{class TestClass { public void TestMethod( int test = 0; test = 1;){}}}";
			SyntaxTree tree1 = CSharpSyntaxTree.ParseText(code1);

			NamespaceDeclarationSyntax nsNode = tree1.GetRootAsync().Result.DescendantNodes().OfType<NamespaceDeclarationSyntax>().First();

			List<SyntaxTree> trees1 = new List<SyntaxTree> { tree1 };
			Compilation comp1 = CSharpCompilation.Create("TestCompilation1", trees1);

			ScriptAnalyzer analyzer = this.createAnalyzer(comp1);

			TaggedSyntaxLibrary lib = analyzer.AnalyzeNode(nsNode);

			Assert.IsNotNull(lib, "Library defined");
			Assert.AreEqual(1, lib.TaggedSyntaxTrees.Count(), "Has one tree");

			TaggedSyntaxTree tree = lib.TaggedSyntaxTrees.First();

			CollectionAssert.Contains(tree.TaggedNodes, nsNode, "Tree contains class node");

			IEnumerable<SyntaxNode> nodes = nsNode.DescendantNodes();

			foreach (SyntaxNode node in nodes)
			{
				CollectionAssert.DoesNotContain(tree.TaggedNodes, node, "SubNode is not added to tree");
			}
		}

		[TestMethod, TestCategory("ScriptBuilder.Analyzer.ScriptAnalyzerTest")]
		public void TestAnalyzeClass()
		{
			string code1 = @"class TestClass { public void TestMethod( int test = 0; test = 1;){}}";
			SyntaxTree tree1 = CSharpSyntaxTree.ParseText(code1);

			ClassDeclarationSyntax classNode = tree1.GetRootAsync().Result.DescendantNodes().OfType<ClassDeclarationSyntax>().First();

			List<SyntaxTree> trees1 = new List<SyntaxTree> { tree1 };
			Compilation comp1 = CSharpCompilation.Create("TestCompilation1", trees1);

			ScriptAnalyzer analyzer = this.createAnalyzer(comp1);

			TaggedSyntaxLibrary lib = analyzer.AnalyzeNode(classNode);

			Assert.IsNotNull(lib, "Library defined");
			Assert.AreEqual(1, lib.TaggedSyntaxTrees.Count(), "Has one tree");

			TaggedSyntaxTree tree = lib.TaggedSyntaxTrees.First();

			CollectionAssert.Contains(tree.TaggedNodes, classNode, "Tree contains class node");

			IEnumerable<SyntaxNode> nodes = classNode.DescendantNodes();
					
			foreach (SyntaxNode node in nodes)
			{
				CollectionAssert.DoesNotContain(tree.TaggedNodes, node, "SubNode is not added to tree");
			}
		}

		[TestMethod, TestCategory("ScriptBuilder.Analyzer.ScriptAnalyzerTest")]
		public void TestAnalyzeVarDeclarator()
		{
			string code1 = @"class TestClass { public void TestMethod(){int test = 0; test = 1;}}";
			SyntaxTree tree1 = CSharpSyntaxTree.ParseText(code1);

			VariableDeclaratorSyntax varDecNode = tree1.GetRootAsync().Result.DescendantNodes().OfType<VariableDeclaratorSyntax>().First();
			VariableDeclarationSyntax varDeclartionNode = tree1.GetRootAsync().Result.DescendantNodes().OfType<VariableDeclarationSyntax>().First();

			List<SyntaxTree> trees1 = new List<SyntaxTree> { tree1 };
			Compilation comp1 = CSharpCompilation.Create("TestCompilation1", trees1);

			ScriptAnalyzer analyzer = this.createAnalyzer(comp1);

			TaggedSyntaxLibrary lib = analyzer.AnalyzeNode(varDecNode);

			Assert.IsNotNull(lib, "Library defined");
			Assert.AreEqual(1, lib.TaggedSyntaxTrees.Count(), "Has one tree");

			TaggedSyntaxTree tree = lib.TaggedSyntaxTrees.First();

			CollectionAssert.Contains(tree.TaggedNodes, varDeclartionNode, "Tree contains variable declaration node");

			IEnumerable<SyntaxNode> nodes = varDeclartionNode.DescendantNodes();

			foreach (SyntaxNode node in nodes)
			{
				CollectionAssert.Contains(tree.TaggedNodes, node, "SubNode is added to tree: " + node.ToString() );
			}
		}

		[TestMethod, TestCategory("ScriptBuilder.Analyzer.ScriptAnalyzerTest")]
		public void TestAnalyzeForEach()
		{
			string code1 = @"class MainClass {public int MainMethod(){ TestClass[] testClasses = new TestClass[] { new TestClass(1), new TestClass(2) };int count = 0; foreach (TestClass testclass in testClasses){count += testclass.GetInt()} return count;}};}";
			string code2 = @"class TestClass{ int counter; public TestClass(int param}{this.counter = param} public int GetInt(){return this.counter}";
			SyntaxTree tree1 = CSharpSyntaxTree.ParseText(code1);
			SyntaxTree tree2 = CSharpSyntaxTree.ParseText(code2);
			ForEachStatementSyntax forEachNode = tree1.GetRootAsync().Result.DescendantNodes().OfType<ForEachStatementSyntax>().First();

			List<SyntaxTree> trees1 = new List<SyntaxTree> { tree1, tree2 };
			Compilation comp1 = CSharpCompilation.Create("TestCompilation1", trees1);

			ScriptAnalyzer analyzer = this.createAnalyzer(comp1);

			TaggedSyntaxLibrary lib = analyzer.AnalyzeNode(forEachNode);

			//Assert.IsNotNull(lib, "Library defined");
			//Assert.AreEqual(1, lib.TaggedSyntaxTrees.Count(), "Has one tree");

			//TaggedSyntaxTree tree = lib.TaggedSyntaxTrees.First();

			//CollectionAssert.Contains(tree.TaggedNodes, varDeclartionNode, "Tree contains variable declaration node");

			//IEnumerable<SyntaxNode> nodes = varDeclartionNode.DescendantNodes();

			//foreach (SyntaxNode node in nodes)
			//{
			//	CollectionAssert.Contains(tree.TaggedNodes, node, "SubNode is added to tree: " + node.ToString());
			//}
		}
		private ScriptAnalyzer createAnalyzer(Compilation[] comps)
		{
			ScriptAnalyzerSymbolHelper sHelper = new ScriptAnalyzerSymbolHelper(comps);
			ScriptAnalyzerResourceHelper rHelper = new ScriptAnalyzerResourceHelper(sHelper);

			ScriptAnalyzer analyzer = new ScriptAnalyzer(rHelper);

			return analyzer;
		}

		private ScriptAnalyzer createAnalyzer(Compilation comp)
		{
			return this.createAnalyzer(new Compilation[] { comp });
		}
	}
}
