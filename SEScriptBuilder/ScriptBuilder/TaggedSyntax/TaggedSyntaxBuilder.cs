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
	class TaggedSyntaxBuilder
	{
		public SyntaxNode BuildNode(CompilationUnitSyntax node, List<SyntaxNode> tagged)
		{
			SyntaxNode member = null;
			member = this.BuildCompilation(node, tagged);
			return member;
		}

		public MemberDeclarationSyntax BuildNode(MemberDeclarationSyntax node, List<SyntaxNode> tagged)
		{
			MemberDeclarationSyntax member = null;
			if (node != null && tagged.Contains(node))
			{
				Type type = node.GetType();

				if (type == typeof(NamespaceDeclarationSyntax))
				{
					member = this.BuildNameSpace(node as NamespaceDeclarationSyntax, tagged);
				}
				if (type == typeof(ClassDeclarationSyntax))
				{
					member = this.BuildClass(node as ClassDeclarationSyntax, tagged);
				}
				if (type == typeof(ConstructorDeclarationSyntax))
				{
					member = this.BuildConstructor(node as ConstructorDeclarationSyntax, tagged);
				}
				if (type == typeof(MethodDeclarationSyntax))
				{
					member = this.BuildMethod(node as MethodDeclarationSyntax, tagged);
				}
				if (type == typeof(FieldDeclarationSyntax))
				{
					member = this.BuildProperty(node as FieldDeclarationSyntax, tagged);
				}
			}

			return member;
		}

		public CompilationUnitSyntax BuildCompilation(CompilationUnitSyntax node, List<SyntaxNode> tagged)
		{
			CompilationUnitSyntax member = null;
			SyntaxList<MemberDeclarationSyntax> newSubMembers = this.BuildMembers(node.Members, tagged);
			member = SyntaxFactory.CompilationUnit(node.Externs, node.Usings, node.AttributeLists, newSubMembers, node.EndOfFileToken);
			return member;
		}

		public MemberDeclarationSyntax BuildNameSpace(NamespaceDeclarationSyntax node, List<SyntaxNode> tagged)
		{
			MemberDeclarationSyntax member = null;
			SyntaxList<MemberDeclarationSyntax> newSubMembers = this.BuildMembers(node.Members, tagged);
			
			member = SyntaxFactory.NamespaceDeclaration(node.NamespaceKeyword, node.Name, node.OpenBraceToken, node.Externs, node.Usings, newSubMembers, node.CloseBraceToken, node.SemicolonToken);
			return member;
		}

		public MemberDeclarationSyntax BuildClass(ClassDeclarationSyntax node, List<SyntaxNode> tagged)
		{
			MemberDeclarationSyntax member = null;
			SyntaxList<MemberDeclarationSyntax> newSubMembers = this.BuildMembers(node.Members, tagged);
			member = SyntaxFactory.ClassDeclaration(node.AttributeLists, node.Modifiers, node.Keyword, node.Identifier, node.TypeParameterList, node.BaseList, node.ConstraintClauses, node.OpenBraceToken, newSubMembers, node.CloseBraceToken, node.SemicolonToken);
			return member;
		}

		public MemberDeclarationSyntax BuildConstructor(ConstructorDeclarationSyntax node, List<SyntaxNode> tagged)
		{
			MemberDeclarationSyntax member = node;

			return member;
		}

		public MemberDeclarationSyntax BuildMethod(MethodDeclarationSyntax node, List<SyntaxNode> tagged)
		{
			MemberDeclarationSyntax member = node;
			
			return member;
		}

		public MemberDeclarationSyntax BuildProperty(FieldDeclarationSyntax node, List<SyntaxNode> tagged)
		{
			MemberDeclarationSyntax member = node;
			
			return member;
		}

		private SyntaxList<MemberDeclarationSyntax> BuildMembers(SyntaxList<MemberDeclarationSyntax> members, List<SyntaxNode> tagged) {
			SyntaxList<MemberDeclarationSyntax> newMembers = new SyntaxList<MemberDeclarationSyntax>();
			if (members != null && members.Count() > 0)
			{
				foreach (MemberDeclarationSyntax member in members)
				{
					MemberDeclarationSyntax newSubMember = this.BuildNode(member, tagged);
					if (newSubMember != null)
					{
						newMembers = newMembers.Add(newSubMember);
					}
				}
			}

			return newMembers;
		}
	}
}
