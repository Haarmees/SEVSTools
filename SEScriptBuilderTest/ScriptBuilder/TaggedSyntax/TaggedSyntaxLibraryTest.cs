using System;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SEScriptBuilder.ScriptBuilder.TaggedSyntax;

namespace SEScriptBuilderTest.ScriptBuilder.TaggedSyntax
{
	[TestClass]
	public class TaggedSyntaxLibraryTest
	{
		[TestMethod, TestCategory("ScriptBuilder.TaggedSyntax.TaggedSyntaxLibrary")]
		public void TestConstructor()
		{
			TaggedSyntaxLibrary lib = new TaggedSyntaxLibrary();

			Assert.IsNotNull(lib.TaggedSyntaxTrees, "Has created list of tagged syntax trees");
			Assert.AreEqual(0, lib.TaggedSyntaxTrees.Count(), "List is empty");
		}

		[TestMethod, TestCategory("ScriptBuilder.TaggedSyntax.TaggedSyntaxLibrary")]
		public void TestTagNode()
		{
			TaggedSyntaxLibrary lib = new TaggedSyntaxLibrary();

			ClassDeclarationSyntax node = SyntaxFactory.ClassDeclaration("test");

			bool result = lib.TagNode(node);

			Assert.IsTrue(result, "Expect result to be true");
			Assert.IsNotNull(lib.TaggedSyntaxTrees, "Has created list of tagged syntax trees");
			Assert.AreEqual(1, lib.TaggedSyntaxTrees.Count(), "List has one tagged tree");

			TaggedSyntaxTree tree = lib.TaggedSyntaxTrees.First();

			Assert.AreEqual(node.SyntaxTree, tree.OriginalTree, "Tagged Syntax tree has the correct syntaxtree");
			Assert.IsTrue(tree.IsTagged(node), "The node is tagged on the tree");
		}

		[TestMethod, TestCategory("ScriptBuilder.TaggedSyntax.TaggedSyntaxLibrary")]
		public void TestIsTagged()
		{
			TaggedSyntaxLibrary lib = new TaggedSyntaxLibrary();

			ClassDeclarationSyntax node = SyntaxFactory.ClassDeclaration("test");
			ClassDeclarationSyntax unknown = SyntaxFactory.ClassDeclaration("unknown");

			bool result = lib.TagNode(node);

			Assert.IsTrue(lib.IsTagged(node), "Expect result to be true");
			Assert.IsFalse(lib.IsTagged(unknown), "Expect result to be false");
		}
	}
}
