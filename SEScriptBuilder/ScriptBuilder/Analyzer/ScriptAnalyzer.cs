using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SEScriptBuilder.ScriptBuilder.TaggedSyntax;

namespace SEScriptBuilder.ScriptBuilder.Analyzer
{
	public class ScriptAnalyzer
	{
		private ScriptAnalyzerResourceHelper resourceHelper;
		public ScriptAnalyzerResourceHelper ResourceHelper { get { return this.resourceHelper; } }

		public ScriptAnalyzer(ScriptAnalyzerResourceHelper rHelper)
		{
			this.resourceHelper = rHelper;
		}

		public TaggedSyntaxLibrary AnalyzeNode(SyntaxNode node, TaggedSyntaxLibrary lib = null) {
			if (lib == null)
			{
				lib = new TaggedSyntaxLibrary();
			}
			if (!lib.IsTagged(node))
			{
				Type type = node.GetType();
				if (type == typeof(NamespaceDeclarationSyntax))
				{
					this.AnalyzeNameSpace(node as NamespaceDeclarationSyntax, lib);
				}
				else if (type == typeof(ClassDeclarationSyntax))
				{
					this.AnalyzeClass(node as ClassDeclarationSyntax, lib);
				}
				else if (type == typeof(VariableDeclaratorSyntax))
				{
					this.AnalyzeVariableDeclarator(node as VariableDeclaratorSyntax, lib);
				}
				else
				{
					this.DefaultAnalyze(node, lib);
				}

				// analyze resources if any
				List<SyntaxNode> resources = this.resourceHelper.GetSyntaxNodes(node);
				if (resources.Count() > 0)
				{
					foreach (SyntaxNode resource in resources)
					{
						this.AnalyzeNode(resource, lib);
					}
				}
			}
			return lib;
		}
		public TaggedSyntaxLibrary AnalyzeNameSpace(NamespaceDeclarationSyntax node, TaggedSyntaxLibrary lib)
		{
			lib.TagNode(node);
			return lib;
		}

		public TaggedSyntaxLibrary AnalyzeClass(ClassDeclarationSyntax node, TaggedSyntaxLibrary lib)
		{
			lib.TagNode(node);
			return lib;
		}

		public TaggedSyntaxLibrary AnalyzeVariableDeclarator(VariableDeclaratorSyntax node, TaggedSyntaxLibrary lib)
		{
			lib.TagNode(node);
			if (node.Parent != null)
			{
				this.AnalyzeNode(node.Parent, lib);
			}
			this.DefaultAnalyze(node, lib);
			return lib;
		}

		public TaggedSyntaxLibrary DefaultAnalyze(SyntaxNode node, TaggedSyntaxLibrary lib)
		{
			lib.TagNode(node);
			IEnumerable<SyntaxNode> subnodes = node.DescendantNodes().Where(n => n.Parent == node);
			if (subnodes != null && subnodes.Count()>0)
			{
				foreach (SyntaxNode subnode in subnodes)
				{
					this.AnalyzeNode(subnode, lib);
				}
			}
			
			return lib;
		}


	}
}
