using System.Collections.Generic;
using System.Text.RegularExpressions;

#pragma warning disable CS1591

namespace FlowGraph
{
	/// <summary>
	/// Represents a <see cref="GimpleStmtType.GELSE"/>.
	/// </summary>
	public class GElseStmt : GimpleStmt
	{
		private static readonly string myPattern = "else";

		public GElseStmt ( string text )
		{
			this.Text = text;
			StmtType = GimpleStmtType.GELSE;
			Pattern = myPattern;
		}

		/// <summary>
		/// Compare given <paramref name="stmt"/> to <see cref="myPattern"/> using <see cref="Regex"/>.
		/// </summary>
		/// <param name="stmt"></param>
		/// <returns></returns>
		public static bool Matches ( string stmt ) => Regex.IsMatch ( stmt, myPattern );

		public override string ToString ( ) => $"else";

		public override List<string> Rename ( string oldName, string newName ) => base.Rename ( oldName, newName );
	}
}