using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SEScriptBuilder.ScriptBuilder.Analyzer;

namespace SEScriptBuilderTest.ScriptBuilder.Analyzer
{
	[TestClass]
	public class ScriptAnalyzerResourceHelperTest
	{
		[TestMethod, TestCategory("ScriptAnalyzerResourceHelperTest")]
		public void TestGetSyntaxNodesFromSymbol()
		{
			string code = "public class ClassA{ public void Test(){}}";
			SyntaxTree tree1 = CSharpSyntaxTree.ParseText(code);

			ClassDeclarationSyntax node1 = tree1.GetRootAsync().Result.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().First();
			MethodDeclarationSyntax node2 = tree1.GetRootAsync().Result.DescendantNodesAndSelf().OfType<MethodDeclarationSyntax>().First();
			
			List<SyntaxTree> trees1 = new List<SyntaxTree> { tree1 };

			Compilation comp1 = CSharpCompilation.Create("TestCompilation1", trees1);

			SemanticModel model1 = comp1.GetSemanticModel(tree1);
			ISymbol symbol1 = model1.GetDeclaredSymbol(node1);
			ISymbol symbol2 = model1.GetDeclaredSymbol(node2);


			List<SyntaxNode> rNodes1 = ScriptAnalyzerResourceHelper.GetSyntaxNodesFromSymbol(symbol1);
			List<SyntaxNode> rNodes2 = ScriptAnalyzerResourceHelper.GetSyntaxNodesFromSymbol(symbol2);

			Assert.IsNotNull(rNodes1, "First result nodes set");
			Assert.AreEqual(1, rNodes1.Count(), "First result contains 1 element");
			Assert.IsNotNull(rNodes2, "Second result nodes set");
			Assert.AreEqual(1, rNodes2.Count(), "Second result contains 1 element");

			SyntaxNode rNode1 = rNodes1.First();
			SyntaxNode rNode2 = rNodes2.First();


			Assert.AreEqual(node1, rNode1, "Gets right node");
			Assert.AreEqual(node2, rNode2, "Gets right node");
		}

		[TestMethod, TestCategory("ScriptAnalyzerResourceHelperTest")]
		public void TestGetSyntaxNodesFromClassNone()
		{
			string code1 = "public class ClassA{}";
			SyntaxTree tree1 = CSharpSyntaxTree.ParseText(code1);
			ClassDeclarationSyntax node1 = tree1.GetRootAsync().Result.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().First();

			List<SyntaxTree> trees1 = new List<SyntaxTree> { tree1 };
			Compilation comp1 = CSharpCompilation.Create("TestCompilation1", trees1);

			ScriptAnalyzerSymbolHelper sHelper = new ScriptAnalyzerSymbolHelper(new Compilation[] { comp1 });
			ScriptAnalyzerResourceHelper rHelper = new ScriptAnalyzerResourceHelper(sHelper);

			List<SyntaxNode> result = rHelper.GetSyntaxNodes(node1);

			Assert.IsNotNull(result, "Returns a list");
			Assert.AreEqual(0, result.Count(), "List is empty");
		}

		[TestMethod, TestCategory("ScriptAnalyzerResourceHelperTest")]
		public void TestGetSyntaxNodesFromClassBase()
		{
			string code1 = "public class ClassA:ClassB{}" +
				"public class ClassB{}";
			SyntaxTree tree1 = CSharpSyntaxTree.ParseText(code1);
			ClassDeclarationSyntax node1 = tree1.GetRootAsync().Result.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().First();
			ClassDeclarationSyntax node2 = tree1.GetRootAsync().Result.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().Last();

			List<SyntaxTree> trees1 = new List<SyntaxTree> { tree1 };
			Compilation comp1 = CSharpCompilation.Create("TestCompilation1", trees1);

			ScriptAnalyzerSymbolHelper sHelper = new ScriptAnalyzerSymbolHelper(new Compilation[] { comp1 });
			ScriptAnalyzerResourceHelper rHelper = new ScriptAnalyzerResourceHelper(sHelper);

			List<SyntaxNode> result = rHelper.GetSyntaxNodes(node1);

			Assert.IsNotNull(result, "Returns a list");
			Assert.AreEqual(1, result.Count(), "List has one node");
			Assert.AreEqual(node2, result.First(), "Node is expected node");
		}

		[TestMethod, TestCategory("ScriptAnalyzerResourceHelperTest")]
		public void TestGetSyntaxNodesFromClassInterface()
		{
			string code1 = "public class ClassA:IClassB{}" +
				"public interface IClassB{}";

			SyntaxTree tree1 = CSharpSyntaxTree.ParseText(code1);
			ClassDeclarationSyntax node1 = tree1.GetRootAsync().Result.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().First();
			InterfaceDeclarationSyntax node2 = tree1.GetRootAsync().Result.DescendantNodesAndSelf().OfType<InterfaceDeclarationSyntax>().First();

			List<SyntaxTree> trees1 = new List<SyntaxTree> { tree1 };
			Compilation comp1 = CSharpCompilation.Create("TestCompilation1", trees1);

			ScriptAnalyzerSymbolHelper sHelper = new ScriptAnalyzerSymbolHelper(new Compilation[] { comp1 });
			ScriptAnalyzerResourceHelper rHelper = new ScriptAnalyzerResourceHelper(sHelper);

			List<SyntaxNode> result = rHelper.GetSyntaxNodes(node1);

			Assert.IsNotNull(result, "Returns a list");
			Assert.AreEqual(1, result.Count(), "List has one node");
			CollectionAssert.Contains(result, node2, "Node is expected node");
		}

		[TestMethod, TestCategory("ScriptAnalyzerResourceHelperTest")]
		public void TestGetSyntaxNodesFromClassInterfaces()
		{
			string code1 = "public class ClassA:IClassB, IClassC{}" +
				"public interface IClassB{} public interface IClassC { }";

			SyntaxTree tree1 = CSharpSyntaxTree.ParseText(code1);
			ClassDeclarationSyntax node1 = tree1.GetRootAsync().Result.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().First();
			InterfaceDeclarationSyntax node2 = tree1.GetRootAsync().Result.DescendantNodesAndSelf().OfType<InterfaceDeclarationSyntax>().First();
			InterfaceDeclarationSyntax node3 = tree1.GetRootAsync().Result.DescendantNodesAndSelf().OfType<InterfaceDeclarationSyntax>().Last();

			List<SyntaxTree> trees1 = new List<SyntaxTree> { tree1 };
			Compilation comp1 = CSharpCompilation.Create("TestCompilation1", trees1);

			ScriptAnalyzerSymbolHelper sHelper = new ScriptAnalyzerSymbolHelper(new Compilation[] { comp1 });
			ScriptAnalyzerResourceHelper rHelper = new ScriptAnalyzerResourceHelper(sHelper);

			List<SyntaxNode> result = rHelper.GetSyntaxNodes(node1);

			Assert.IsNotNull(result, "Returns a list");
			Assert.AreEqual(2, result.Count(), "List has two nodes");
			CollectionAssert.Contains(result, node2, "Node is expected node");
			CollectionAssert.Contains(result, node3, "Node is expected node");
		}

		[TestMethod, TestCategory("ScriptAnalyzerResourceHelperTest")]
		public void TestGetSyntaxNodesFromClassBaseInterface()
		{
			string code1 = "public class ClassA:ClassB, IClassC{}" +
				"public class ClassB{} public interface IClassC { }";

			SyntaxTree tree1 = CSharpSyntaxTree.ParseText(code1);
			ClassDeclarationSyntax node1 = tree1.GetRootAsync().Result.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().First();
			ClassDeclarationSyntax node2 = tree1.GetRootAsync().Result.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().Last();
			InterfaceDeclarationSyntax node3 = tree1.GetRootAsync().Result.DescendantNodesAndSelf().OfType<InterfaceDeclarationSyntax>().First();

			List<SyntaxTree> trees1 = new List<SyntaxTree> { tree1 };
			Compilation comp1 = CSharpCompilation.Create("TestCompilation1", trees1);

			ScriptAnalyzerSymbolHelper sHelper = new ScriptAnalyzerSymbolHelper(new Compilation[] { comp1 });
			ScriptAnalyzerResourceHelper rHelper = new ScriptAnalyzerResourceHelper(sHelper);

			List<SyntaxNode> result = rHelper.GetSyntaxNodes(node1);

			Assert.IsNotNull(result, "Returns a list");
			Assert.AreEqual(2, result.Count(), "List has two nodes");
			CollectionAssert.Contains(result, node2, "Node is expected node");
			CollectionAssert.Contains(result, node3, "Node is expected node");
		}


		[TestMethod, TestCategory("ScriptAnalyzerResourceHelperTest")]
		public void TestGetSyntaxNodesFromMethodNone()
		{
			string code1 = "public void Test(){}";

			SyntaxTree tree1 = CSharpSyntaxTree.ParseText(code1);
			MethodDeclarationSyntax node1 = tree1.GetRootAsync().Result.DescendantNodesAndSelf().OfType<MethodDeclarationSyntax>().First();
			
			List<SyntaxTree> trees1 = new List<SyntaxTree> { tree1 };
			Compilation comp1 = CSharpCompilation.Create("TestCompilation1", trees1);

			ScriptAnalyzerSymbolHelper sHelper = new ScriptAnalyzerSymbolHelper(new Compilation[] { comp1 });
			ScriptAnalyzerResourceHelper rHelper = new ScriptAnalyzerResourceHelper(sHelper);

			List<SyntaxNode> result = rHelper.GetSyntaxNodes(node1);

			Assert.IsNotNull(result, "Returns a list");
			Assert.AreEqual(0, result.Count(), "List has no nodes");
		}

		[TestMethod, TestCategory("ScriptAnalyzerResourceHelperTest")]
		public void TestGetSyntaxNodesFromMethodReturn()
		{
			string code1 = "public ClassA Test(){} class ClassA{}";

			SyntaxTree tree1 = CSharpSyntaxTree.ParseText(code1);
			MethodDeclarationSyntax node1 = tree1.GetRootAsync().Result.DescendantNodesAndSelf().OfType<MethodDeclarationSyntax>().First();
			ClassDeclarationSyntax node2 = tree1.GetRootAsync().Result.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().First();

			List<SyntaxTree> trees1 = new List<SyntaxTree> { tree1 };
			Compilation comp1 = CSharpCompilation.Create("TestCompilation1", trees1);

			ScriptAnalyzerSymbolHelper sHelper = new ScriptAnalyzerSymbolHelper(new Compilation[] { comp1 });
			ScriptAnalyzerResourceHelper rHelper = new ScriptAnalyzerResourceHelper(sHelper);

			List<SyntaxNode> result = rHelper.GetSyntaxNodes(node1);

			Assert.IsNotNull(result, "Returns a list");
			Assert.AreEqual(1, result.Count(), "List has one node");
			CollectionAssert.Contains(result, node2, "Node is expected node");
		}

		[TestMethod, TestCategory("ScriptAnalyzerResourceHelperTest")]
		public void TestGetSyntaxNodesFromMethodParameters()
		{
			string code1 = "public void Test(ClassA first, ClassB second, ClassB third, int fourth){} class ClassA{} class ClassB{}";

			SyntaxTree tree1 = CSharpSyntaxTree.ParseText(code1);
			MethodDeclarationSyntax node1 = tree1.GetRootAsync().Result.DescendantNodesAndSelf().OfType<MethodDeclarationSyntax>().First();
			ClassDeclarationSyntax node2 = tree1.GetRootAsync().Result.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().First();
			ClassDeclarationSyntax node3 = tree1.GetRootAsync().Result.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().Last();

			List<SyntaxTree> trees1 = new List<SyntaxTree> { tree1 };
			Compilation comp1 = CSharpCompilation.Create("TestCompilation1", trees1);

			ScriptAnalyzerSymbolHelper sHelper = new ScriptAnalyzerSymbolHelper(new Compilation[] { comp1 });
			ScriptAnalyzerResourceHelper rHelper = new ScriptAnalyzerResourceHelper(sHelper);

			List<SyntaxNode> result = rHelper.GetSyntaxNodes(node1);

			Assert.IsNotNull(result, "Returns a list");
			Assert.AreEqual(2, result.Count(), "List has two nodes");
			CollectionAssert.Contains(result, node2, "Node is expected node");
			CollectionAssert.Contains(result, node2, "Node is expected node");
		}

		[TestMethod, TestCategory("ScriptAnalyzerResourceHelperTest")]
		public void TestGetSyntaxNodesFromInvocationExpression()
		{
			string code1 = "public class TestCLass{public void Main(){Test(\"test\");} public void Test(){} public void Test(string param){}}";

			SyntaxTree tree1 = CSharpSyntaxTree.ParseText(code1);
			InvocationExpressionSyntax node1 = tree1.GetRootAsync().Result.DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>().First();
			MethodDeclarationSyntax node2 = tree1.GetRootAsync().Result.DescendantNodesAndSelf().OfType<MethodDeclarationSyntax>().ElementAt(1);
			MethodDeclarationSyntax node3 = tree1.GetRootAsync().Result.DescendantNodesAndSelf().OfType<MethodDeclarationSyntax>().Last();

			List<SyntaxTree> trees1 = new List<SyntaxTree> { tree1 };
			Compilation comp1 = CSharpCompilation.Create("TestCompilation1", trees1);

			ScriptAnalyzerSymbolHelper sHelper = new ScriptAnalyzerSymbolHelper(new Compilation[] { comp1 });
			ScriptAnalyzerResourceHelper rHelper = new ScriptAnalyzerResourceHelper(sHelper);

			List<SyntaxNode> result = rHelper.GetSyntaxNodes(node1);

			Assert.IsNotNull(result, "Returns a list");
			Assert.AreEqual(1, result.Count(), "List has one node");
			CollectionAssert.Contains(result, node3, "Node is expected node");
			CollectionAssert.DoesNotContain(result, node2, "Node is not other unused function");
		}

		[TestMethod, TestCategory("ScriptAnalyzerResourceHelperTest")]
		public void TestGetSyntaxNodesFromObjectCreation()
		{
			string code1 = "public class TestCLass{public void Main(){ClassA test = new ClassA(0);}} public class ClassA{ public ClassA(){} public ClassA(int param){}}";

			SyntaxTree tree1 = CSharpSyntaxTree.ParseText(code1);
			ObjectCreationExpressionSyntax node1 = tree1.GetRootAsync().Result.DescendantNodesAndSelf().OfType<ObjectCreationExpressionSyntax>().First();
			ClassDeclarationSyntax node2 = tree1.GetRootAsync().Result.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().Last();
			ConstructorDeclarationSyntax node3 = node2.DescendantNodes().OfType<ConstructorDeclarationSyntax>().First();
			ConstructorDeclarationSyntax node4 = node2.DescendantNodes().OfType<ConstructorDeclarationSyntax>().Last();
			IdentifierNameSyntax node5 = node1.DescendantNodes().OfType<IdentifierNameSyntax>().Last();

			List<SyntaxTree> trees1 = new List<SyntaxTree> { tree1 };
			Compilation comp1 = CSharpCompilation.Create("TestCompilation1", trees1);

			ScriptAnalyzerSymbolHelper sHelper = new ScriptAnalyzerSymbolHelper(new Compilation[] { comp1 });
			ScriptAnalyzerResourceHelper rHelper = new ScriptAnalyzerResourceHelper(sHelper);

			List<SyntaxNode> result = rHelper.GetSyntaxNodes(node1);

			Assert.IsNotNull(result, "Returns a list");
			Assert.AreEqual(1, result.Count(), "List has one node");
			CollectionAssert.Contains(result, node4, "Node is right class constructor");
		}

		[TestMethod, TestCategory("ScriptAnalyzerResourceHelperTest")]
		public void TestGetSyntaxNodesFromIdentifierType()
		{
			string code1 = "public class TestCLass{ ClassA prop;} public class ClassA{}";

			SyntaxTree tree1 = CSharpSyntaxTree.ParseText(code1);
			VariableDeclarationSyntax node1 = tree1.GetRootAsync().Result.DescendantNodesAndSelf().OfType<VariableDeclarationSyntax>().First();
			ClassDeclarationSyntax node2 = tree1.GetRootAsync().Result.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().Last();
			IdentifierNameSyntax node3 = node1.DescendantNodes().OfType<IdentifierNameSyntax>().First();

			List<SyntaxTree> trees1 = new List<SyntaxTree> { tree1 };
			Compilation comp1 = CSharpCompilation.Create("TestCompilation1", trees1);

			ScriptAnalyzerSymbolHelper sHelper = new ScriptAnalyzerSymbolHelper(new Compilation[] { comp1 });
			ScriptAnalyzerResourceHelper rHelper = new ScriptAnalyzerResourceHelper(sHelper);

			List<SyntaxNode> result = rHelper.GetSyntaxNodes(node3);

			Assert.IsNotNull(result, "Returns a list");
			Assert.AreEqual(1, result.Count(), "List has one node");
			CollectionAssert.Contains(result, node2, "Node is expected node");
		}

		[TestMethod, TestCategory("ScriptAnalyzerResourceHelperTest")]
		public void TestGetSyntaxNodesFromIdentifierVar()
		{
			string code1 = "public class TestCLass{ int prop; public void Main(){ prop = 1;}}";

			SyntaxTree tree1 = CSharpSyntaxTree.ParseText(code1);
			VariableDeclarationSyntax node1 = tree1.GetRootAsync().Result.DescendantNodesAndSelf().OfType<VariableDeclarationSyntax>().First();
			MethodDeclarationSyntax node2 = tree1.GetRootAsync().Result.DescendantNodesAndSelf().OfType<MethodDeclarationSyntax>().Last();
			IdentifierNameSyntax node3 = node2.DescendantNodes().OfType<IdentifierNameSyntax>().First();
			VariableDeclaratorSyntax eNode = node1.DescendantNodes().OfType<VariableDeclaratorSyntax>().First();
			List<SyntaxTree> trees1 = new List<SyntaxTree> { tree1 };
			Compilation comp1 = CSharpCompilation.Create("TestCompilation1", trees1);

			ScriptAnalyzerSymbolHelper sHelper = new ScriptAnalyzerSymbolHelper(new Compilation[] { comp1 });
			ScriptAnalyzerResourceHelper rHelper = new ScriptAnalyzerResourceHelper(sHelper);

			List<SyntaxNode> result = rHelper.GetSyntaxNodes(node3);

			Assert.IsNotNull(result, "Returns a list");
			Assert.AreEqual(1, result.Count(), "List has one node");
			CollectionAssert.Contains(result, eNode, "Node is expected node");
		}

		[TestMethod, TestCategory("ScriptAnalyzerResourceHelperTest")]
		public void TestGetSyntaxNodesFromIdentifierFunc()
		{
			string code1 = "public class TestCLass{public void Main(){Test(\"test\");} public void Test(){} public void Test(string param){}}";

			SyntaxTree tree1 = CSharpSyntaxTree.ParseText(code1);
			InvocationExpressionSyntax node1 = tree1.GetRootAsync().Result.DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>().First();
			MethodDeclarationSyntax node2 = tree1.GetRootAsync().Result.DescendantNodesAndSelf().OfType<MethodDeclarationSyntax>().ElementAt(1);
			MethodDeclarationSyntax node3 = tree1.GetRootAsync().Result.DescendantNodesAndSelf().OfType<MethodDeclarationSyntax>().Last();
			IdentifierNameSyntax node4 = node1.DescendantNodes().OfType<IdentifierNameSyntax>().Last();

			List<SyntaxTree> trees1 = new List<SyntaxTree> { tree1 };
			Compilation comp1 = CSharpCompilation.Create("TestCompilation1", trees1);

			ScriptAnalyzerSymbolHelper sHelper = new ScriptAnalyzerSymbolHelper(new Compilation[] { comp1 });
			ScriptAnalyzerResourceHelper rHelper = new ScriptAnalyzerResourceHelper(sHelper);

			List<SyntaxNode> result = rHelper.GetSyntaxNodes(node4);

			Assert.IsNotNull(result, "Returns a list");
			Assert.AreEqual(1, result.Count(), "List has one node");
			CollectionAssert.Contains(result, node3, "Node is expected node");
			CollectionAssert.DoesNotContain(result, node2, "Node is not other unused function");
		}

		[TestMethod, TestCategory("ScriptAnalyzerResourceHelperTest")]
		public void TestGetSyntaxNodesFromIdentifierObjectCreation()
		{
			string code1 = "public class TestCLass{public void Main(){ClassA test = new ClassA(0);}} public class ClassA{ public ClassA(){} public ClassA(int param){}}";

			SyntaxTree tree1 = CSharpSyntaxTree.ParseText(code1);
			ObjectCreationExpressionSyntax node1 = tree1.GetRootAsync().Result.DescendantNodesAndSelf().OfType<ObjectCreationExpressionSyntax>().First();
			ClassDeclarationSyntax node2 = tree1.GetRootAsync().Result.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().Last();
			ConstructorDeclarationSyntax node3 = node2.DescendantNodes().OfType<ConstructorDeclarationSyntax>().First();
			ConstructorDeclarationSyntax node4 = node2.DescendantNodes().OfType<ConstructorDeclarationSyntax>().Last();
			IdentifierNameSyntax node5 = node1.DescendantNodes().OfType<IdentifierNameSyntax>().Last();

			List<SyntaxTree> trees1 = new List<SyntaxTree> { tree1 };
			Compilation comp1 = CSharpCompilation.Create("TestCompilation1", trees1);

			ScriptAnalyzerSymbolHelper sHelper = new ScriptAnalyzerSymbolHelper(new Compilation[] { comp1 });
			ScriptAnalyzerResourceHelper rHelper = new ScriptAnalyzerResourceHelper(sHelper);

			List<SyntaxNode> result = rHelper.GetSyntaxNodes(node5);

			Assert.IsNotNull(result, "Returns a list");
			Assert.AreEqual(1, result.Count(), "List has one node");
			CollectionAssert.Contains(result, node2, "Node is class");
		}
	}
}
