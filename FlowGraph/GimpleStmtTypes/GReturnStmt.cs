using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

#pragma warning disable CS1591

namespace FlowGraph
{
	/// <summary>
	/// Represents a <see cref="GimpleStmtType.GRETURN"/>.
	/// </summary>
	public class GReturnStmt : GimpleStmt
	{
		private static readonly string myPattern = @"return( (?<retval>\S*))?;";

		public string Retval { get; private set; }

		public GReturnStmt ( string text )
		{
			this.Text = text;
			StmtType = GimpleStmtType.GRETURN;
			Pattern = myPattern;
			var match = Regex.Match ( text, myPattern );
			Retval = match.Groups["retval"].Value;
			Vars.AddRange ( new[] { Retval }.Where ( x => IsValidIdentifier ( x ) ) );
		}

		/// <summary>
		/// Compare given <paramref name="stmt"/> to <see cref="myPattern"/> using <see cref="Regex"/>.
		/// </summary>
		/// <param name="stmt"></param>
		/// <returns></returns>
		public static bool Matches ( string stmt ) => Regex.IsMatch ( stmt, myPattern );

		public override string ToString ( )
		{
			string val = Retval == "" ? "" : $" {Retval}";
			return $"return{val};";
		}

		public override List<string> Rename ( string oldName, string newName )
		{
			if ( object.Equals ( Retval, oldName ) )
				Retval = newName;
			return base.Rename ( oldName, newName );
		}
	}
}