using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SEScriptBuilder.ScriptBuilder.TaggedSyntax
{
	public class TaggedSyntaxTree
	{
		private SyntaxTree originalTree;
		public SyntaxTree OriginalTree { get { return this.originalTree; } }
		private SyntaxNode originalRoot;
		public SyntaxNode OriginalRoot { get { return this.originalRoot; } }
		private List<SyntaxNode> taggedNodes;
		public List<SyntaxNode> TaggedNodes { get { return this.taggedNodes; } }


		public TaggedSyntaxTree(SyntaxTree original)
		{
			this.originalTree = original;
			this.originalRoot = this.originalTree.GetRootAsync().Result;
			this.taggedNodes = new List<SyntaxNode>();
		}

		public bool TagNode(SyntaxNode node)
		{
			bool succes = false;
			// check if it is node from original tree
			if (this.originalRoot.DescendantNodesAndSelf().Contains(node) && !this.IsTagged(node))
			{
				// node can be added
				succes = true;
				this.taggedNodes.Add(node);
			}

			return succes;
		}

		public bool IsTagged(SyntaxNode node)
		{
			return this.taggedNodes.Contains(node);
		}

		public void TagAllParents()
		{
			if (this.taggedNodes.Count() > 0)
			{
				List<SyntaxNode> cNodes = new List<SyntaxNode>(this.taggedNodes);
				foreach (SyntaxNode node in cNodes)
				{
					this.tagParents(node);
				}
			}
		}

		public SyntaxTree GetSyntaxTree()
		{
			SyntaxTree tree = null;
			this.TagAllParents();

			TaggedSyntaxBuilder builder = new TaggedSyntaxBuilder();
			SyntaxNode node = builder.BuildNode(this.originalRoot as CompilationUnitSyntax, this.taggedNodes);
			tree = node.SyntaxTree;

			return tree;
		}

		private List<SyntaxNode> filterNodesToRemove(List<SyntaxNode> nodes)
		{
			List<SyntaxNode> fNodes = new List<SyntaxNode>();
			if (nodes != null && nodes.Count()>0)
			{
				foreach (SyntaxNode node in nodes)
				{
					if (node.Parent == null || !nodes.Contains(node.Parent))
					{
						fNodes.Add(node);
					}
				}
			}
			return fNodes;
		}

		private void tagParents(SyntaxNode node)
		{
			if (node.Parent != null && !this.IsTagged(node.Parent))
			{
				this.tagParents(node.Parent);
			}
			this.TagNode(node);
		}
	}
}
