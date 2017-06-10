using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace SEScriptBuilder.ScriptBuilder.Analyzer
{
	public interface IScriptAnalyzerSymbolHelper
	{
		List<Compilation> Compilations { get; }
		ISymbol GetSymbol(SyntaxNode node);
		SymbolInfo GetSymbolInfo(SyntaxNode node);
		SemanticModel GetModel(SyntaxNode node);
		Compilation GetCompilation(SyntaxNode node);
	}
}
