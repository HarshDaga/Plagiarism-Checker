using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

#pragma warning disable CS1591

namespace FlowGraph
{
	/// <summary>
	/// Represents a <see cref="GimpleStmtType.GCOND"/>.
	/// </summary>
	public class GCondStmt : GimpleStmt
	{
		private static string myPattern = @"if \((?<op1>[\w\.]*) (?<op>\S*) (?<op2>[\w\.]*)\)";

		public string Op1 { get; private set; }
		public string Op2 { get; private set; }
		public string Op { get; private set; }

		public GCondStmt ( string text )
		{
			this.Text = text;
			StmtType = GimpleStmtType.GCOND;
			Pattern = myPattern;
			var match = Regex.Match ( text, myPattern );
			Op1 = match.Groups["op1"].Value;
			Op2 = match.Groups["op2"].Value;
			Op = match.Groups["op"].Value;
			Vars.AddRange ( new[] { Op1, Op2 }.Where ( x => IsValidIdentifier ( x ) ) );
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
			return $"if ({Op1} {Op} {Op2})";
		}

		public override List<string> Rename ( string oldName, string newName )
		{
			if ( Op1 == oldName )
				Op1 = newName;
			if ( Op2 == oldName )
				Op2 = newName;
			return base.Rename ( oldName, newName );
		}
	}
}