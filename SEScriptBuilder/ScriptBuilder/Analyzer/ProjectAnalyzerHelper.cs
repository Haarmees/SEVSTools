using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SEScriptBuilder.ScriptBuilder.Analyzer
{
	public static class ProjectAnalyzerHelper
	{
		public static ClassDeclarationSyntax GetMainClass(SyntaxTree tree)
		{
			ClassDeclarationSyntax node = null;
			if (tree != null)
			{
				IEnumerable<ClassDeclarationSyntax> classes = tree.GetRootAsync().Result.DescendantNodes().OfType<ClassDeclarationSyntax>();
				if (classes != null && classes.Count() > 0)
				{
					node = classes.First();
				}
			}

			return node;
		}

		public static ConstructorDeclarationSyntax GetConstructor(ClassDeclarationSyntax node)
		{
			ConstructorDeclarationSyntax constructor = null;
			if (node != null)
			{
				IEnumerable<ConstructorDeclarationSyntax> cons = node.DescendantNodes().OfType<ConstructorDeclarationSyntax>();
				if (cons != null && cons.Count() > 0)
				{
					constructor = cons.First();
				}
			}
			
			return constructor;
		}

		public static MethodDeclarationSyntax GetMethodDeclaration(ClassDeclarationSyntax node, string name)
		{
			MethodDeclarationSyntax method = null;
			if (node != null && name != null)
			{
				IEnumerable<MethodDeclarationSyntax> methods = node.DescendantNodes().OfType<MethodDeclarationSyntax>();
				if (methods != null && methods.Count() > 0)
				{
					foreach (MethodDeclarationSyntax m in methods)
					{
						if (m.Identifier != null && m.Identifier.Text != null && m.Identifier.Text == name)
						{
							method = m;
						}
					}
				}
			}
			return method;
		}
	}
}
