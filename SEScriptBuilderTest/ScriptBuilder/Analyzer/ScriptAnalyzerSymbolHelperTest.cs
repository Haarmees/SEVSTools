using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SEScriptBuilder.ScriptBuilder.Analyzer;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SEScriptBuilderTest.ScriptBuilder.Analyzer
{

	[TestClass]
	public class ScriptAnalyzerSymbolHelperTest
	{
		[TestCategory("ScriptAnalyzerSymbolHelperTest"), TestMethod]
		public void TestConstructor()
		{
			Compilation comp = CSharpCompilation.Create("TestCompilation");

			Compilation[] arr = new Compilation[] { comp };
			List<Compilation> expected = arr.ToList();
			ScriptAnalyzerSymbolHelper helper = new ScriptAnalyzerSymbolHelper(arr);
			List<Compilation> result = helper.Compilations;

			Assert.IsNotNull(result, "Compilations get set");
			CollectionAssert.AreEqual(expected, result, "Are equal lists");
		}

		[TestCategory("ScriptAnalyzerSymbolHelperTest"), TestMethod]
		public void TestGetCompilation()
		{
			string code = "public Class ClassA{}";
			SyntaxTree tree1 = CSharpSyntaxTree.ParseText(code);
			SyntaxTree tree2 = CSharpSyntaxTree.ParseText(code);
			SyntaxTree tree3 = CSharpSyntaxTree.ParseText(code);
			SyntaxTree tree4 = CSharpSyntaxTree.ParseText(code);

			SyntaxNode node1 = tree1.GetRootAsync().Result.DescendantNodes().First();
			SyntaxNode node2 = tree2.GetRootAsync().Result.DescendantNodes().First();
			SyntaxNode node3 = tree3.GetRootAsync().Result.DescendantNodes().First();
			SyntaxNode node4 = tree4.GetRootAsync().Result.DescendantNodes().First();

			List<SyntaxTree> trees1 = new List<SyntaxTree> { tree1, tree2 };
			List<SyntaxTree> trees2 = new List<SyntaxTree> { tree3 };

			Compilation comp1 = CSharpCompilation.Create("TestCompilation1", trees1);
			Compilation comp2 = CSharpCompilation.Create("TestCompilation2", trees2);

			Compilation[] arr = new Compilation[] { comp1, comp2 };

			ScriptAnalyzerSymbolHelper helper = new ScriptAnalyzerSymbolHelper(arr);

			Assert.AreEqual(comp1, helper.GetCompilation(node1), "Gets right compilation");
			Assert.AreEqual(comp1, helper.GetCompilation(node2), "Gets right compilation");
			Assert.AreEqual(comp2, helper.GetCompilation(node3), "Gets right compilation");
			Assert.IsNull(helper.GetCompilation(node4), "Returns null if nothing found");
			
		}

		[TestCategory("ScriptAnalyzerSymbolHelperTest"), TestMethod]
		public void TestGetModel()
		{
			string code = "public Class ClassA{}";
			SyntaxTree tree1 = CSharpSyntaxTree.ParseText(code);
			SyntaxTree tree2 = CSharpSyntaxTree.ParseText(code);
			SyntaxTree tree3 = CSharpSyntaxTree.ParseText(code);
			SyntaxTree tree4 = CSharpSyntaxTree.ParseText(code);

			SyntaxNode node1 = tree1.GetRootAsync().Result.DescendantNodes().First();
			SyntaxNode node2 = tree2.GetRootAsync().Result.DescendantNodes().First();
			SyntaxNode node3 = tree3.GetRootAsync().Result.DescendantNodes().First();
			SyntaxNode node4 = tree4.GetRootAsync().Result.DescendantNodes().First();

			List<SyntaxTree> trees1 = new List<SyntaxTree> { tree1, tree2 };
			List<SyntaxTree> trees2 = new List<SyntaxTree> { tree3 };

			Compilation comp1 = CSharpCompilation.Create("TestCompilation1", trees1);
			Compilation comp2 = CSharpCompilation.Create("TestCompilation2", trees2);

			Compilation[] arr = new Compilation[] { comp1, comp2 };

			ScriptAnalyzerSymbolHelper helper = new ScriptAnalyzerSymbolHelper(arr);

			SemanticModel eModel1 = comp1.GetSemanticModel(tree1);
			SemanticModel eModel2 = comp1.GetSemanticModel(tree2);
			SemanticModel eModel3 = comp2.GetSemanticModel(tree3);

			SemanticModel rModel1 = helper.GetModel(node1);
			SemanticModel rModel2 = helper.GetModel(node2);
			SemanticModel rModel3 = helper.GetModel(node3);
			SemanticModel rModel4 = helper.GetModel(node4);

			Assert.AreEqual(eModel1.SyntaxTree, rModel1.SyntaxTree, "Gets right model");
			Assert.AreEqual(eModel2.SyntaxTree, rModel2.SyntaxTree, "Gets right model");
			Assert.AreEqual(eModel3.SyntaxTree, rModel3.SyntaxTree, "Gets right model");
			Assert.IsNull(rModel4, "Returns null if nothing found");

			SemanticModel rModel5 = helper.GetModel(node1);

			Assert.AreEqual(rModel1, rModel5, "Only 1 model is created and reused");

		}

		[TestCategory("ScriptAnalyzerSymbolHelperTest"), TestMethod]
		public void TestGetSymbol()
		{
			string code = "public class ClassA{ ClassB test; }";
			SyntaxTree tree1 = CSharpSyntaxTree.ParseText(code);
			SyntaxTree tree2 = CSharpSyntaxTree.ParseText(code);

			ClassDeclarationSyntax node1 = tree1.GetRootAsync().Result.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().First();
			ClassDeclarationSyntax node2 = tree2.GetRootAsync().Result.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().First();
			IdentifierNameSyntax node3 = tree1.GetRootAsync().Result.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>().First();


			List<SyntaxTree> trees1 = new List<SyntaxTree> { tree1 };

			Compilation comp1 = CSharpCompilation.Create("TestCompilation1", trees1);
			

			Compilation[] arr = new Compilation[] { comp1 };

			ScriptAnalyzerSymbolHelper helper = new ScriptAnalyzerSymbolHelper(arr);

			SemanticModel eModel1 = comp1.GetSemanticModel(tree1);
			ISymbol eSymbol1 = eModel1.GetDeclaredSymbol(node1);


			ISymbol rSymbol1 = helper.GetSymbol(node1);
			ISymbol rSymbol2 = helper.GetSymbol(node2);
			ISymbol rSymbol3 = helper.GetSymbol(node3);

			Assert.AreEqual(eSymbol1, rSymbol1, "Gets right symbol");
			Assert.IsNull(rSymbol2, "Returns null if nothing found");
			Assert.IsNull(rSymbol3, "Returns null if nothing found");

		}

		[TestCategory("ScriptAnalyzerSymbolHelperTest"), TestMethod]
		public void TestGetSymbolInfo()
		{
			string code = "public class ClassA{ ClassB test;}";
			SyntaxTree tree1 = CSharpSyntaxTree.ParseText(code);
			SyntaxTree tree2 = CSharpSyntaxTree.ParseText(code);

			IdentifierNameSyntax node1 = tree1.GetRootAsync().Result.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>().First();
			IdentifierNameSyntax node2 = tree2.GetRootAsync().Result.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>().First();
			ClassDeclarationSyntax node3 = tree1.GetRootAsync().Result.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().First();


			List<SyntaxTree> trees1 = new List<SyntaxTree> { tree1 };

			Compilation comp1 = CSharpCompilation.Create("TestCompilation1", trees1);


			Compilation[] arr = new Compilation[] { comp1 };

			ScriptAnalyzerSymbolHelper helper = new ScriptAnalyzerSymbolHelper(arr);

			SemanticModel eModel1 = comp1.GetSemanticModel(tree1);
			SymbolInfo eSymbolInfo1 = eModel1.GetSymbolInfo(node1);


			SymbolInfo rSymbolInfo1 = helper.GetSymbolInfo(node1);
			SymbolInfo rSymbolInfo2 = helper.GetSymbolInfo(node2);
			SymbolInfo rSymbolInfo3 = helper.GetSymbolInfo(node2);


			Assert.AreEqual(eSymbolInfo1, rSymbolInfo1, "Gets right symbolInfo");
			Assert.AreEqual(rSymbolInfo2, new SymbolInfo(), "Returns empty symbol info if nothing found");
			Assert.AreEqual(rSymbolInfo3, new SymbolInfo(), "Returns empty symbol info if nothing found");
		}
	}


}
