using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SEScriptBuilder.ScriptBuilder.TaggedSyntax;

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

		public List<SyntaxNode> GetSyntaxNodes(SyntaxNode node, TaggedSyntaxLibrary lib = null)
		{
			List<SyntaxNode> nodes = new List<SyntaxNode>();
			Type type = node.GetType();

			if (type == typeof(ClassDeclarationSyntax))
			{
				nodes = this.GetSyntaxNodesFromClass(node as ClassDeclarationSyntax, lib);
			}
			else if (type == typeof(ConstructorInitializerSyntax))
			{
				nodes = this.GetSyntaxNodesFromConstructorInitializer(node as ConstructorInitializerSyntax);
			}
			else if (type == typeof(MethodDeclarationSyntax))
			{
				nodes = this.GetSyntaxNodesFromMethod(node as MethodDeclarationSyntax, lib);
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

		public List<SyntaxNode> GetSyntaxNodesFromClass(ClassDeclarationSyntax node, TaggedSyntaxLibrary lib)
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

				// check for override methods
				List<IMethodSymbol> oMethods = this.getOverrideMethodSymbolsFromClass(node);
				if (oMethods != null && oMethods.Count() > 0)
				{
					foreach (IMethodSymbol oMethod in oMethods)
					{
						IMethodSymbol overridden = oMethod.OverriddenMethod;
						if (overridden != null)
						{
							List<SyntaxNode> oNodes = ScriptAnalyzerResourceHelper.GetSyntaxNodesFromSymbol(overridden);
							if (oNodes != null && oNodes.Count() > 0)
							{
								MethodDeclarationSyntax oNode = oNodes.First() as MethodDeclarationSyntax;
								if (oNode != null)
								{
									TaggedSyntaxTree tTree = lib.GetTreeFromNode(oNode);
									if (tTree != null && tTree.IsTagged(oNode))
									{
										symbols.Add(oMethod);
									}
								}
							}
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

		public List<SyntaxNode> GetSyntaxNodesFromMethod(MethodDeclarationSyntax node, TaggedSyntaxLibrary lib)
		{
			List<SyntaxNode> nodes = new List<SyntaxNode>();
			IMethodSymbol symbol = this.symbolHelper.GetSymbol(node) as IMethodSymbol;
			if (symbol != null)
			{
				List<ISymbol> symbols = new List<ISymbol>();
				if (symbol.IsOverride)
				{
					symbols.Add(symbol.OverriddenMethod);
				}
				if (symbol.IsVirtual)
				{
					List<ISymbol> mSymbols = this.GetSymbolsFromVirtualFunction(symbol, lib).Select(s => s as ISymbol).ToList() ;
					symbols.AddRange(mSymbols);
				}
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

		private List<IMethodSymbol> GetSymbolsFromVirtualFunction(IMethodSymbol symbol, TaggedSyntaxLibrary lib)
		{
			List<IMethodSymbol> symbols = new List<IMethodSymbol>();

			if (lib.TaggedSyntaxTrees != null && lib.TaggedSyntaxTrees.Count() > 0)
			{
				foreach (TaggedSyntaxTree tTree in lib.TaggedSyntaxTrees)
				{
					List<ClassDeclarationSyntax> classes = tTree.TaggedNodes.OfType<ClassDeclarationSyntax>().ToList();
					if (classes != null && classes.Count() > 0)
					{
						foreach (ClassDeclarationSyntax classSyntax in classes) {
							List<IMethodSymbol> mSymbols = this.getOverrideMethodSymbolsFromClass(classSyntax);
							if (mSymbols != null && mSymbols.Count() > 0)
							{
								foreach (IMethodSymbol mSymbol in mSymbols)
								{
									if (mSymbol.OverriddenMethod == symbol)
									{
										symbols.Add(mSymbol);
									}
								}
							}
						}
						
					}
				}
			}

			return symbols;
		}

		private List<IMethodSymbol> getOverrideMethodSymbolsFromClass(ClassDeclarationSyntax node)
		{
			List<IMethodSymbol> symbols = new List<IMethodSymbol>();
			if (node != null)
			{
				List<MethodDeclarationSyntax> methods = node.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();
				if (methods != null && methods.Count() > 0)
				{
					foreach (MethodDeclarationSyntax method in methods)
					{
						IMethodSymbol symbol = this.symbolHelper.GetSymbol(method) as IMethodSymbol;
						if (symbol != null)
						{
							if (symbol.IsOverride)
							{
								symbols.Add(symbol);
							}
						}
					}
				}
			}
			return symbols;
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
