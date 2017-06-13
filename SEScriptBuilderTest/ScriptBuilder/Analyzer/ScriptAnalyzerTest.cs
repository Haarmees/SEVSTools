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
		public void TestAnalyzeOverrideMethod()
		{
			string code1 = @"class TestClass { public void MainMethod(){ SubClass test = new SubClass(); test.TestMethod();}}";
			string code2 = @"class SuperClass { public virtual void TestMethod(){}}";
			string code3 = @"class SubClass:SuperClass { public override void TestMethod(){}}";
			SyntaxTree tree1 = CSharpSyntaxTree.ParseText(code1);
			SyntaxTree tree2 = CSharpSyntaxTree.ParseText(code2);
			SyntaxTree tree3 = CSharpSyntaxTree.ParseText(code3);

			MethodDeclarationSyntax mainNode = tree1.GetRootAsync().Result.DescendantNodes().OfType<MethodDeclarationSyntax>().First();
			MethodDeclarationSyntax superNode = tree2.GetRootAsync().Result.DescendantNodes().OfType<MethodDeclarationSyntax>().First();
			MethodDeclarationSyntax subNode = tree3.GetRootAsync().Result.DescendantNodes().OfType<MethodDeclarationSyntax>().First();
			List<SyntaxTree> trees1 = new List<SyntaxTree> { tree1 , tree2, tree3};
			Compilation comp1 = CSharpCompilation.Create("TestCompilation1", trees1);

			ScriptAnalyzer analyzer = this.createAnalyzer(comp1);

			TaggedSyntaxLibrary lib = analyzer.AnalyzeNode(mainNode);

			Assert.IsNotNull(lib, "Library defined");
			Assert.AreEqual(3, lib.TaggedSyntaxTrees.Count(), "Has three trees");

			TaggedSyntaxTree rTree1 = lib.TaggedSyntaxTrees.First();
			TaggedSyntaxTree rTree2 = lib.TaggedSyntaxTrees.ElementAt(1);
			TaggedSyntaxTree rTree3 = lib.TaggedSyntaxTrees.ElementAt(2);

			CollectionAssert.Contains(rTree1.TaggedNodes, mainNode, "Main tree contains main method");
			CollectionAssert.Contains(rTree3.TaggedNodes, superNode, "Super tree contains super method");
			CollectionAssert.Contains(rTree2.TaggedNodes, subNode, "Sub tree contains sub method");
		}

		[TestMethod, TestCategory("ScriptBuilder.Analyzer.ScriptAnalyzerTest")]
		public void TestAnalyzeOverrideMethodFromVirtual()
		{
			string code1 = @"class TestClass { public void MainMethod(){ SubClass test = new SubClass(); test.TestMethod();}}";
			string code2 = @"class SuperClass { public void TestMethod(){this.TestMethod2();} public virtual void TestMethod2(){}}";
			string code3 = @"class SubClass:SuperClass { public override void TestMethod2(){}}";
			SyntaxTree tree1 = CSharpSyntaxTree.ParseText(code1);
			SyntaxTree tree2 = CSharpSyntaxTree.ParseText(code2);
			SyntaxTree tree3 = CSharpSyntaxTree.ParseText(code3);

			MethodDeclarationSyntax mainNode = tree1.GetRootAsync().Result.DescendantNodes().OfType<MethodDeclarationSyntax>().First();
			MethodDeclarationSyntax superNode1 = tree2.GetRootAsync().Result.DescendantNodes().OfType<MethodDeclarationSyntax>().First();
			MethodDeclarationSyntax superNode2 = tree2.GetRootAsync().Result.DescendantNodes().OfType<MethodDeclarationSyntax>().Last();
			MethodDeclarationSyntax subNode = tree3.GetRootAsync().Result.DescendantNodes().OfType<MethodDeclarationSyntax>().First();
			List<SyntaxTree> trees1 = new List<SyntaxTree> { tree1, tree2, tree3 };
			Compilation comp1 = CSharpCompilation.Create("TestCompilation1", trees1);

			ScriptAnalyzer analyzer = this.createAnalyzer(comp1);

			TaggedSyntaxLibrary lib = analyzer.AnalyzeNode(mainNode);

			Assert.IsNotNull(lib, "Library defined");
			Assert.AreEqual(3, lib.TaggedSyntaxTrees.Count(), "Has three trees");

			TaggedSyntaxTree rTree1 = lib.TaggedSyntaxTrees.First();
			TaggedSyntaxTree rTree2 = lib.TaggedSyntaxTrees.ElementAt(1);
			TaggedSyntaxTree rTree3 = lib.TaggedSyntaxTrees.ElementAt(2);

			CollectionAssert.Contains(rTree1.TaggedNodes, mainNode, "Main tree contains main method");
			CollectionAssert.Contains(rTree3.TaggedNodes, superNode1, "Super tree contains first method");
			CollectionAssert.Contains(rTree3.TaggedNodes, superNode2, "Super tree contains first method");
			CollectionAssert.Contains(rTree2.TaggedNodes, subNode, "Sub tree contains sub method");
		}

		[TestMethod, TestCategory("ScriptBuilder.Analyzer.ScriptAnalyzerTest")]
		public void TestAnalyzeOverrideMethodFromClass()
		{
			string code1 = @"class TestClass { public void MainMethod(){ SubClass test = new SubClass(); test.TestMethod(); NextSubClass test2 = new NextSubClass(); test.TestMethod();}}";
			string code2 = @"class SuperClass { public void TestMethod(){this.TestMethod2();} public virtual void TestMethod2(){}}";
			string code3 = @"class SubClass:SuperClass { public override void TestMethod2(){}}";
			string code4 = @"class NextSubClass:SuperClass { public override void TestMethod2(){}}";
			SyntaxTree tree1 = CSharpSyntaxTree.ParseText(code1);
			SyntaxTree tree2 = CSharpSyntaxTree.ParseText(code2);
			SyntaxTree tree3 = CSharpSyntaxTree.ParseText(code3);
			SyntaxTree tree4 = CSharpSyntaxTree.ParseText(code4);

			MethodDeclarationSyntax mainNode = tree1.GetRootAsync().Result.DescendantNodes().OfType<MethodDeclarationSyntax>().First();
			MethodDeclarationSyntax superNode1 = tree2.GetRootAsync().Result.DescendantNodes().OfType<MethodDeclarationSyntax>().First();
			MethodDeclarationSyntax superNode2 = tree2.GetRootAsync().Result.DescendantNodes().OfType<MethodDeclarationSyntax>().Last();
			MethodDeclarationSyntax subNode = tree3.GetRootAsync().Result.DescendantNodes().OfType<MethodDeclarationSyntax>().First();
			MethodDeclarationSyntax nextSubNode = tree4.GetRootAsync().Result.DescendantNodes().OfType<MethodDeclarationSyntax>().First();
			List<SyntaxTree> trees1 = new List<SyntaxTree> { tree1, tree2, tree3, tree4 };
			Compilation comp1 = CSharpCompilation.Create("TestCompilation1", trees1);

			ScriptAnalyzer analyzer = this.createAnalyzer(comp1);

			TaggedSyntaxLibrary lib = analyzer.AnalyzeNode(mainNode);

			Assert.IsNotNull(lib, "Library defined");
			Assert.AreEqual(4, lib.TaggedSyntaxTrees.Count(), "Has three trees");

			TaggedSyntaxTree rTree1 = lib.TaggedSyntaxTrees.First();
			TaggedSyntaxTree rTree2 = lib.TaggedSyntaxTrees.ElementAt(1);
			TaggedSyntaxTree rTree3 = lib.TaggedSyntaxTrees.ElementAt(2);
			TaggedSyntaxTree rTree4 = lib.TaggedSyntaxTrees.ElementAt(3);

			CollectionAssert.Contains(rTree1.TaggedNodes, mainNode, "Main tree contains main method");
			CollectionAssert.Contains(rTree3.TaggedNodes, superNode1, "Super tree contains first method");
			CollectionAssert.Contains(rTree3.TaggedNodes, superNode2, "Super tree contains first method");
			CollectionAssert.Contains(rTree2.TaggedNodes, subNode, "Sub tree contains sub method");
			CollectionAssert.Contains(rTree4.TaggedNodes, nextSubNode, "Next Sub tree contains sub method");
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
