using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace SEScriptBuilder.ScriptBuilder.Analyzer
{
	public class ScriptAnalyzerSymbolHelper : IScriptAnalyzerSymbolHelper
	{
		private List<Compilation> compilations;
		public List<Compilation> Compilations { get { return this.compilations; } }
		private List<SemanticModel> models;

		public ScriptAnalyzerSymbolHelper(Compilation[] comps)
		{
			compilations = comps.ToList();
		}

		public ISymbol GetSymbol(SyntaxNode node) {
			ISymbol symbol = null;
			if (node != null)
			{
				SemanticModel model = this.GetModel(node);
				if (model != null)
				{
					symbol = model.GetDeclaredSymbol(node);
				}
			}
			return symbol;
		}

		public SymbolInfo GetSymbolInfo(SyntaxNode node)
		{
			SymbolInfo sInfo = new SymbolInfo();
			if (node != null)
			{
				SemanticModel model = this.GetModel(node);
				if (model != null)
				{
					sInfo = model.GetSymbolInfo(node);
				}
			}
			return sInfo;
		}

		public SemanticModel GetModel(SyntaxNode node)
		{
			SemanticModel model = null;
			if (this.models == null)
			{
				this.models = new List<SemanticModel>();
			}

			if (node != null && node.SyntaxTree != null)
			{
				if (!this.models.Exists(m => m.SyntaxTree == node.SyntaxTree)){
					Compilation comp = this.GetCompilation(node);
					if (comp != null)
					{
						model = comp.GetSemanticModel(node.SyntaxTree);
						this.models.Add(model);
					}
				} else {
					model = this.models.Where(m => m.SyntaxTree == node.SyntaxTree).First();
				}
			}
			return model;
		}

		public Compilation GetCompilation(SyntaxNode node)
		{
			Compilation compilation = null;
			if (node != null && node.SyntaxTree != null && this.Compilations.Count() > 0)
			{
				foreach (Compilation comp in this.Compilations)
				{
					if (comp.SyntaxTrees.Contains(node.SyntaxTree))
					{
						compilation = comp;
					}
				}
			}
			return compilation;
		}

		
	}
}
