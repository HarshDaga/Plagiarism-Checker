using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

#pragma warning disable CS1591

namespace FlowGraph
{
	/// <summary>
	/// Represents a <see cref="GimpleStmtType.GPHI"/>.
	/// </summary>
	public class GPhiStmt : GimpleStmt
	{
		private static readonly string myPattern = @"# (?<assignee>[\w\.]*) = PHI <(?<var1>[\w\.]*)\((?<bb1>\S*)\)(, (?<var2>[\w\.]*)\((?<bb2>\S*)\))?>";

		public string Assignee { get; private set; }
		public string Var1 { get; private set; }
		public string Var2 { get; private set; }
		public string Bb1 { get; private set; }
		public string Bb2 { get; private set; }

		public GPhiStmt ( string text )
		{
			this.Text = text;
			StmtType = GimpleStmtType.GPHI;
			Pattern = myPattern;
			var match = Regex.Match ( text, myPattern );
			Assignee = match.Groups["assignee"].Value;
			Var1 = match.Groups["var1"].Value;
			Var2 = match.Groups["var2"].Value;
			Bb1 = match.Groups["bb1"].Value;
			Bb2 = match.Groups["bb2"].Value;
			Vars.AddRange ( new[] { Assignee, Var1, Var2 }.Where ( x => IsValidIdentifier ( x ) ) );
		}
		
		/// <summary>
		/// Compare given <paramref name="stmt"/> to <see cref="myPattern"/> using <see cref="Regex"/>.
		/// </summary>
		/// <param name="stmt"></param>
		/// <returns></returns>
		public static bool Matches ( string stmt ) => Regex.IsMatch ( stmt, myPattern );

		public override string ToString ( )
		{
			string second = Var2 == "" ? "" : $", {Var2}({Bb2})";
			return $"# {Assignee} = PHI <{Var1}({Bb1}){second}>";
		}

		public override List<string> Rename ( string oldName, string newName )
		{
			if ( Assignee == oldName )
				Assignee = newName;
			if ( Var1 == oldName )
				Var1 = newName;
			if ( object.Equals ( Var2, oldName ) )
				Var2 = newName;
			return base.Rename ( oldName, newName );
		}
	}
}