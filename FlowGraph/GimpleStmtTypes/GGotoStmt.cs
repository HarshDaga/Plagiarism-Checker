using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

#pragma warning disable CS1591

namespace FlowGraph
{
	/// <summary>
	/// Represents a <see cref="GimpleStmtType.GGOTO"/>.
	/// </summary>
	public class GGotoStmt : GimpleStmt
	{
		private static readonly string myPattern = "goto <bb (?<number>[0-9]*)>;";

		public int Number { get; private set; }

		public GGotoStmt ( string text )
		{
			this.Text = text;
			StmtType = GimpleStmtType.GGOTO;
			Pattern = myPattern;
			var match = Regex.Match ( text, myPattern );
			Number = Convert.ToInt32 ( match.Groups["number"].Value );
		}

		/// <summary>
		/// Compare given <paramref name="stmt"/> to <see cref="myPattern"/> using <see cref="Regex"/>.
		/// </summary>
		/// <param name="stmt"></param>
		/// <returns></returns>
		public static bool Matches ( string stmt ) => Regex.IsMatch ( stmt, myPattern );

		public override string ToString ( ) => $"goto <bb {Number}>;";
	}
}