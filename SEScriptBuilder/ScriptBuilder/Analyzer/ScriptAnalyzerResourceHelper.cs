using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SEScriptBuilder.ScriptBuilder.Analyzer
{
	public class ScriptAnalyzerResourceHelper
	{
		private IScriptAnalyzerSymbolHelper symbolHelper;
		public IScriptAnalyzerSymbolHelper SymbolHelper { get { return this.symbolHelper; } }

		public ScriptAnalyzerResourceHelper(IScriptAnalyzerSymbolHelper sHelper)
		{
			this.symbolHelper = sHelper;
		}

		public List<SyntaxNode> GetSyntaxNodes(SyntaxNode node)
		{
			List<SyntaxNode> nodes = new List<SyntaxNode>();
			Type type = node.GetType();

			if (type == typeof(ClassDeclarationSyntax))
			{
				nodes = this.GetSyntaxNodesFromClass(node as ClassDeclarationSyntax);
			}
			else if (type == typeof(ConstructorInitializerSyntax))
			{
				nodes = this.GetSyntaxNodesFromConstructorInitializer(node as ConstructorInitializerSyntax);
			}
			else if (type == typeof(MethodDeclarationSyntax))
			{
				nodes = this.GetSyntaxNodesFromMethod(node as MethodDeclarationSyntax);
			}
			else if (type == typeof(InvocationExpressionSyntax))
			{
				nodes = this.GetSyntaxNodesFromInvocation(node as InvocationExpressionSyntax);
			}
			else if (type == typeof(ObjectCreationExpressionSyntax))
			{
				nodes = this.GetSyntaxNodesFromObjectCreation(node as ObjectCreationExpressionSyntax);
			}
			else if (type == typeof(IdentifierNameSyntax))
			{
				nodes = this.GetSyntaxNodesFromIdentifier(node as IdentifierNameSyntax);
			}


			return nodes;
		}

		public List<SyntaxNode> GetSyntaxNodesFromClass(ClassDeclarationSyntax node)
		{
			List<SyntaxNode> nodes = new List<SyntaxNode>();
			INamedTypeSymbol classSymbol = this.symbolHelper.GetSymbol(node) as INamedTypeSymbol;
			if (classSymbol != null)
			{
				List<ISymbol> symbols = new List<ISymbol>();
				INamedTypeSymbol baseType = classSymbol.BaseType;
				if (baseType != null)
				{
					symbols.Add(baseType);
				}

				ImmutableArray<INamedTypeSymbol> interfaces = classSymbol.Interfaces;
				if (interfaces != null && interfaces.Count() > 0)
				{
					foreach (INamedTypeSymbol inter in interfaces)
					{
						if (inter != null)
						{
							symbols.Add(inter);

						}
					}
				}
				nodes = ScriptAnalyzerResourceHelper.GetSyntaxNodesFromSymbols(symbols);
			}
			
			return nodes;
		}

		public List<SyntaxNode> GetSyntaxNodesFromConstructorInitializer(ConstructorInitializerSyntax node)
		{
			List<SyntaxNode> nodes = new List<SyntaxNode>();
			ISymbol symbol = this.symbolHelper.GetSymbolInfo(node).Symbol;
			if (symbol != null)
			{
				nodes = ScriptAnalyzerResourceHelper.GetSyntaxNodesFromSymbol(symbol);
			}
			return nodes;
		}

		public List<SyntaxNode> GetSyntaxNodesFromMethod(MethodDeclarationSyntax node)
		{
			List<SyntaxNode> nodes = new List<SyntaxNode>();
			IMethodSymbol symbol = this.symbolHelper.GetSymbol(node) as IMethodSymbol;
			if (symbol != null)
			{
				List<ISymbol> symbols = new List<ISymbol>();

				// return type
				ITypeSymbol returnType = symbol.ReturnType;
				if (returnType != null)
				{
					symbols.Add(returnType);
				}

				// parameters
				ImmutableArray<IParameterSymbol> parameterList = symbol.Parameters;
				if (parameterList != null && parameterList.Count() > 0)
				{
					foreach (IParameterSymbol param in parameterList)
					{
						if (param != null && param.Type != null)
						{
							symbols.Add(param.Type);
						}
					}
				}
				nodes = ScriptAnalyzerResourceHelper.GetSyntaxNodesFromSymbols(symbols);
			}
		
			return nodes;
		}

		public List<SyntaxNode> GetSyntaxNodesFromInvocation(InvocationExpressionSyntax node)
		{
			List<SyntaxNode> nodes = new List<SyntaxNode>();
			List<ISymbol> symbols = new List<ISymbol>();

			// function
			if (node.Expression != null)
			{
				ISymbol symbol = this.symbolHelper.GetSymbolInfo(node.Expression).Symbol;
				if (symbol != null)
				{
					symbols.Add(symbol);
				}
			}

			nodes = ScriptAnalyzerResourceHelper.GetSyntaxNodesFromSymbols(symbols);
			return nodes;
		}

		public List<SyntaxNode> GetSyntaxNodesFromObjectCreation(ObjectCreationExpressionSyntax node)
		{
			List<SyntaxNode> nodes = new List<SyntaxNode>();
			ISymbol symbol = this.SymbolHelper.GetSymbolInfo(node).Symbol;
			if (symbol != null)
			{
				nodes = ScriptAnalyzerResourceHelper.GetSyntaxNodesFromSymbol(symbol);
			}
			return nodes;
		}

		public List<SyntaxNode> GetSyntaxNodesFromIdentifier(IdentifierNameSyntax node)
		{
			List<SyntaxNode> nodes = new List<SyntaxNode>();
			ISymbol symbol = this.SymbolHelper.GetSymbolInfo(node).Symbol;
			if (symbol != null)
			{
				nodes = ScriptAnalyzerResourceHelper.GetSyntaxNodesFromSymbol(symbol);
			}
			return nodes;
		}

		public static List<SyntaxNode> GetSyntaxNodesFromSymbols(List<ISymbol> symbols)
		{
			List<SyntaxNode> nodes = new List<SyntaxNode>();
			if (symbols != null && symbols.Count() > 0)
			{
				// filter duplicate symbols
				symbols = ScriptAnalyzerResourceHelper.filterSymbols(symbols);
				foreach (ISymbol symbol in symbols)
				{
					List<SyntaxNode> newNodes = ScriptAnalyzerResourceHelper.GetSyntaxNodesFromSymbol(symbol);
					if (newNodes.Count() > 0)
					{
						nodes.AddRange(newNodes);
					}
				}
			}
			return nodes;
		}

		public static List<SyntaxNode> GetSyntaxNodesFromSymbol(ISymbol symbol)
		{
			List<SyntaxNode> nodes = new List<SyntaxNode>();
			if (symbol.DeclaringSyntaxReferences.Count() > 0)
			{
				foreach (SyntaxReference synRef in symbol.DeclaringSyntaxReferences)
				{
					SyntaxNode node = synRef.GetSyntaxAsync().Result;
					if (node != null)
					{
						nodes.Add(node);
					}
				}
			}

			return nodes;
		}

		private static List<ISymbol> filterSymbols(List<ISymbol> symbols)
		{
			List<ISymbol> newSymbols = new List<ISymbol>();
			if (symbols != null && symbols.Count() > 0)
			{
				foreach (ISymbol symbol in symbols) {
					if (!newSymbols.Exists(s => s.Name == symbol.Name))
					{
						newSymbols.Add(symbol);
					}
				}
			}

			// remove duplicates
			
			return newSymbols;
		}
	}
}
