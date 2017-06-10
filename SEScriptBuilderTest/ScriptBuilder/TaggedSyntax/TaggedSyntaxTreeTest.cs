using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SEScriptBuilder.ScriptBuilder.TaggedSyntax;

namespace SEScriptBuilderTest.ScriptBuilder.TaggedSyntax
{
	[TestClass]
	public class TaggedSyntaxTreeTest
	{
		[TestMethod, TestCategory("ScriptBuilder.TaggedSyntaxTree.TaggedSyntaxTree")]
		public void TestConstructor()
		{
			SyntaxTree tree = CSharpSyntaxTree.ParseText(@"namespace Test{public class TestClass{}}");

			TaggedSyntaxTree fTree = new TaggedSyntaxTree(tree);

			Assert.IsNotNull(fTree, "Object created");
			Assert.IsNotNull(fTree.TaggedNodes, "List created");
			Assert.AreEqual(0, fTree.TaggedNodes.Count, "List is empty");
			Assert.IsNotNull(fTree.OriginalTree, "Original tree not null");
			Assert.AreEqual(tree, fTree.OriginalTree, "Original tree is correct syntax tree");
			Assert.IsNotNull(fTree.OriginalRoot, "Original root is set");
			Assert.AreEqual(tree.GetRootAsync().Result, fTree.OriginalRoot, "Original root is correct root");
		}

		[TestMethod, TestCategory("ScriptBuilder.TaggedSyntaxTree.TaggedSyntaxTree")]
		public void TestTagNode()
		{
			SyntaxTree tree = CSharpSyntaxTree.ParseText(@"namespace Test{public class TestClass{}}");

			TaggedSyntaxTree fTree = new TaggedSyntaxTree(tree);

			ClassDeclarationSyntax node = tree.GetRootAsync().Result.DescendantNodes().OfType<ClassDeclarationSyntax>().First();

			bool succes = fTree.TagNode(node);
			Assert.IsTrue(succes, "Tagging was succesfull");
			Assert.IsNotNull(fTree.TaggedNodes, "Tagged notes is set");
			Assert.AreEqual(1, fTree.TaggedNodes.Count, "Only one node is tagged");
			CollectionAssert.Contains(fTree.TaggedNodes, node, "Right node is tagged");

			succes = fTree.TagNode(node);
			Assert.IsFalse(succes, "Tagging should be unsuccessfull (already tagged node)");
			Assert.IsNotNull(fTree.TaggedNodes, "Tagged notes still set");
			Assert.AreEqual(1, fTree.TaggedNodes.Count, "Tagged notes still has only one node");
			CollectionAssert.Contains(fTree.TaggedNodes, node, "The correct node is still tagged");
		}

		[TestMethod, TestCategory("ScriptBuilder.TaggedSyntaxTree.TaggedSyntaxTree")]
		public void TestTagUnknownNode()
		{
			SyntaxTree tree = CSharpSyntaxTree.ParseText(@"namespace Test{public class TestClass{}}");

			TaggedSyntaxTree fTree = new TaggedSyntaxTree(tree);

			SyntaxNode node = SyntaxFactory.ClassDeclaration("test2");

			bool succes = fTree.TagNode(node);
			Assert.IsFalse(succes, "Adding was not succesfull");
			Assert.IsNotNull(fTree.TaggedNodes, "Added notes is set");
			Assert.AreEqual(0, fTree.TaggedNodes.Count, "Zero nodes are added");
		}

		[TestMethod, TestCategory("ScriptBuilder.TaggedSyntaxTree.TaggedSyntaxTree")]
		public void TestIsTagged()
		{
			SyntaxTree tree = CSharpSyntaxTree.ParseText(@"namespace Test{public class TestClass{}}");

			TaggedSyntaxTree fTree = new TaggedSyntaxTree(tree);

			SyntaxNode node = SyntaxFactory.ClassDeclaration("test2");

			SyntaxNode root = tree.GetRootAsync().Result;
			NamespaceDeclarationSyntax testNs = root.DescendantNodes().OfType<NamespaceDeclarationSyntax>().First();
			ClassDeclarationSyntax unknown = SyntaxFactory.ClassDeclaration("test2");
			bool succes = fTree.TagNode(root);
			Assert.IsTrue(fTree.IsTagged(root), "HasNode returns true for added root");
			Assert.IsFalse(fTree.IsTagged(testNs), "HasNode returns false for not added node (although its in the original tree)");
			Assert.IsFalse(fTree.IsTagged(unknown), "HasNode returns false for unknown node");
		}

		[TestMethod, TestCategory("ScriptBuilder.TaggedSyntaxTree.TaggedSyntaxTree")]
		public void TestTagAllParents()
		{
			SyntaxTree tree = CSharpSyntaxTree.ParseText(@"namespace Test{public class TestClass{}}");

			TaggedSyntaxTree fTree = new TaggedSyntaxTree(tree);


			SyntaxNode root = tree.GetRootAsync().Result;
			NamespaceDeclarationSyntax testNs = root.DescendantNodes().OfType<NamespaceDeclarationSyntax>().First();
			ClassDeclarationSyntax testClass = root.DescendantNodes().OfType<ClassDeclarationSyntax>().First();
			bool succes = fTree.TagNode(testClass);
			fTree.TagAllParents();
			Assert.IsTrue(fTree.IsTagged(root), "HasNode returns true for added root");
			Assert.IsTrue(fTree.IsTagged(testNs), "HasNode returns true for namespace");
			Assert.IsTrue(fTree.IsTagged(testClass), "HasNode returns true for class node");
		}
	}



}
