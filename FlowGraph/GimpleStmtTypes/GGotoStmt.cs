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
		private static string myPattern = "goto <bb (?<number>[0-9]*)>;";

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
		public static bool Matches ( string stmt )
		{
			return Regex.IsMatch ( stmt, myPattern );
		}

		public override string ToString ( )
		{
			return $"goto <bb {Number}>;";
		}

		public override List<string> Rename ( string oldName, string newName )
		{
			return base.Rename ( oldName, newName );
		}

		public override bool Equals ( object obj )
		{
			if ( obj is GGotoStmt )
				return ToString ( ) == ( obj as GGotoStmt ).ToString ( );
			return base.Equals ( obj );
		}

		public override int GetHashCode ( )
		{
			return base.GetHashCode ( );
		}
	}
}