using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

#pragma warning disable CS1591

namespace FlowGraph
{
	/// <summary>
	/// Represents a <see cref="GimpleStmtType.GBB"/>.
	/// </summary>
	public class GBBStmt : GimpleStmt
	{
		private static readonly string myPattern = "<bb (?<number>[0-9]+)>:";

		/// <summary>
		/// Base Block number.
		/// </summary>
		public int Number { get; private set; }

		public GBBStmt ( string text )
		{
			this.Text = text;
			StmtType = GimpleStmtType.GBB;
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

		public override string ToString ( ) => $"<bb {Number}>:";

		public override List<string> Rename ( string oldName, string newName ) => base.Rename ( oldName, newName );
	}
}