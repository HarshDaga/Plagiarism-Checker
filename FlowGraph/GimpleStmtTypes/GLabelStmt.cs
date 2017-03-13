using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

#pragma warning disable CS1591

namespace FlowGraph
{
	/// <summary>
	/// Represents a <see cref="GimpleStmtType.GLABEL"/>.
	/// </summary>
	public class GLabelStmt : GimpleStmt
	{
		private static readonly string myPattern = "goto <bb [0-9]*>;";

		public GLabelStmt ( string text )
		{
			this.Text = text;
			StmtType = GimpleStmtType.GLABEL;
			Pattern = myPattern;
			throw new NotImplementedException ( );
		}
		
		/// <summary>
		/// Compare given <paramref name="stmt"/> to <see cref="myPattern"/> using <see cref="Regex"/>.
		/// </summary>
		/// <param name="stmt"></param>
		/// <returns></returns>
		public static bool Matches ( string stmt ) => Regex.IsMatch ( stmt, myPattern );
	}
}