using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace SEScriptBuilder.ScriptBuilder.TaggedSyntax
{
	public class TaggedSyntaxLibrary
	{
		private List<TaggedSyntaxTree> taggedSyntaxTrees;
		public List<TaggedSyntaxTree> TaggedSyntaxTrees { get { return this.taggedSyntaxTrees; } }

		public TaggedSyntaxLibrary()
		{
			this.taggedSyntaxTrees = new List<TaggedSyntaxTree> ();
		}

		public bool IsTagged(SyntaxNode node)
		{
			bool result = false;
			if (node != null)
			{
				TaggedSyntaxTree tree = this.GetTreeFromNode(node);
				if (tree != null)
				{
					result = tree.IsTagged(node);
				}
			}
			return result;
		}

		public bool TagNode(SyntaxNode node)
		{
			bool result = false;
			if (node != null && node.SyntaxTree != null && !this.IsTagged(node))
			{
				TaggedSyntaxTree tree = this.GetTreeFromNode(node);
				if (tree == null)
				{
					tree = new TaggedSyntaxTree(node.SyntaxTree);
					this.taggedSyntaxTrees.Add(tree);
				}

				result = tree.TagNode(node);
			}
			return result;
		}

		public TaggedSyntaxTree GetTreeFromNode(SyntaxNode node) {
			TaggedSyntaxTree rTree = null;
			if (this.taggedSyntaxTrees.Count > 0 && node != null)
			{
				foreach (TaggedSyntaxTree tree in this.taggedSyntaxTrees)
				{
					if (tree.OriginalTree == node.SyntaxTree)
					{
						rTree = tree;
					}
				}
			}

			return rTree;
		}
	}

	
}
