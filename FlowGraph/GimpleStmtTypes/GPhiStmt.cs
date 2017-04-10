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
		private static readonly string myPattern = @"# (?<assignee>[\w\.]*) = PHI <(?<branches>(([\w\.\-\&\*]*)\((\S*)\)(, )?)+)>";

		public string Assignee { get; private set; }
		public List<(string v, string bb)> Branches { get; set; } = new List<(string v, string bb)> ( );

		public GPhiStmt ( string text )
		{
			this.Text = text;
			StmtType = GimpleStmtType.GPHI;
			Pattern = myPattern;
			var match = Regex.Match ( text, myPattern );
			Assignee = match.Groups["assignee"].Value;
			var strBranches = match.Groups["branches"].Value;
			string branchPattern = @"(?<v>[\w\.]+)\((?<bb>\d+)\)";

			foreach ( Match m in Regex.Matches ( strBranches, branchPattern ) )
			{
				Branches.Add ( (m.Groups["v"].Value, m.Groups["bb"].Value) );
			}
			if ( IsValidIdentifier ( Assignee ) )
				Vars.Add ( Assignee );
			Vars.AddRange ( Branches.Select ( b => b.v ).Where ( v => IsValidIdentifier ( v ) ) );
		}
		
		/// <summary>
		/// Compare given <paramref name="stmt"/> to <see cref="myPattern"/> using <see cref="Regex"/>.
		/// </summary>
		/// <param name="stmt"></param>
		/// <returns></returns>
		public static bool Matches ( string stmt ) => Regex.IsMatch ( stmt, myPattern );

		public override string ToString ( )
		{
			var strBranches = string.Join ( ", ", Branches.Select ( b => $"{b.v}({b.bb})" ) );
			return $"# {Assignee} = PHI <{strBranches}>";
		}

		public override List<string> Rename ( string oldName, string newName )
		{
			if ( Assignee == oldName )
				Assignee = newName;
			List<(string v, string bb)> newBranches = new List<(string v, string bb)> ( );
			foreach ( var branch in Branches )
			{
				if ( branch.v == oldName )
					newBranches.Add ( (newName, branch.bb) );
				else
					newBranches.Add ( branch );
			}
			Branches = newBranches;
			return base.Rename ( oldName, newName );
		}
	}
}