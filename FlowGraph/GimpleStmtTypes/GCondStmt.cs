using System;
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
		private static readonly string myPattern = @"if \((?<op1>[\w\.\-\&\*]*) (?<op>\S*) (?<op2>[\w\.\-\&\*]*)\)";

		public string Op1 { get; set; }
		public string Op2 { get; set; }
		public string Op { get; set; }

		public GCondStmt ( string text )
		{
			this.Text = text;
			StmtType = GimpleStmtType.GCOND;
			Pattern = myPattern;
			var match = Regex.Match ( text, myPattern );
			Op1 = match.Groups["op1"].Value;
			Op2 = match.Groups["op2"].Value;
			Op = match.Groups["op"].Value;

			//if ( Op == "==" || Op == "!=" )
			//{
			//	if ( Op1.CompareTo ( Op2 ) > 0 )
			//	{
			//		var temp = Op1;
			//		Op1 = Op2;
			//		Op2 = temp;
			//	}
			//}

			Vars.AddRange ( new[] { Op1, Op2 }.Where ( x => IsValidIdentifier ( x ) ) );
		}
		
		/// <summary>
		/// Compare given <paramref name="stmt"/> to <see cref="myPattern"/> using <see cref="Regex"/>.
		/// </summary>
		/// <param name="stmt"></param>
		/// <returns></returns>
		public static bool Matches ( string stmt ) => Regex.IsMatch ( stmt, myPattern );

		private static string NormalizeRelationalOperator ( string op )
		{
			Dictionary<string, string> dict = new Dictionary<string, string>
			{
				["=="] = "!=",
				["!="] = "!=",
				["<"] = "<",
				["<="] = "<=",
				[">"] = "<=",
				[">="] = "<"
			};

			if ( dict.ContainsKey ( op ) )
				return dict[op];
			return op;
		}

		public bool Normalize ( )
		{
			var normalized = NormalizeRelationalOperator ( Op );
			if ( normalized == Op )
				return false;

			Op = normalized;

			return true;
		}

		public override string ToString ( ) => $"if ({Op1} {Op} {Op2})";

		public override List<string> Rename ( string oldName, string newName )
		{
			if ( Op1 == oldName )
				Op1 = newName;
			if ( Op2 == oldName )
				Op2 = newName;
			return base.Rename ( oldName, newName );
		}

		public static bool operator == ( GCondStmt lhs, GCondStmt rhs )
		{
			if ( Object.ReferenceEquals ( lhs, null ) )
				return Object.ReferenceEquals ( rhs, null );
			else if ( Object.ReferenceEquals ( rhs, null ) )
				return false;

			if ( lhs.Op == "==" || lhs.Op == "!=" )
			{
				if ( lhs.Op1 == rhs.Op1 && lhs.Op2 == rhs.Op2 && lhs.Op == rhs.Op )
					return true;
				if ( lhs.Op1 == rhs.Op2 && lhs.Op2 == rhs.Op1 && lhs.Op == rhs.Op )
					return true;
			}

			var lhsOp = NormalizeRelationalOperator(lhs.Op);
			var rhsOp = NormalizeRelationalOperator(rhs.Op);
			string lhsOp1, lhsOp2, rhsOp1, rhsOp2;

			if ( lhsOp != lhs.Op )
			{
				lhsOp1 = lhs.Op2;
				lhsOp2 = lhs.Op1;
			}
			else
			{
				lhsOp1 = lhs.Op1;
				lhsOp2 = lhs.Op2;
			}
			if ( rhsOp != rhs.Op )
			{
				rhsOp1 = rhs.Op2;
				rhsOp2 = rhs.Op1;
			}
			else
			{
				rhsOp1 = rhs.Op1;
				rhsOp2 = rhs.Op2;
			}

			return ( lhsOp1 == rhsOp1 && lhsOp2 == rhsOp2 && lhsOp == rhsOp );
		}

		public static bool operator != ( GCondStmt lhs, GCondStmt rhs )
		{
			return !( lhs == rhs );
		}

		public override bool Equals ( object obj )
		{
			if ( obj is GCondStmt )
				return ( obj as GCondStmt ) == this;
			else
				return false;
		}

		public override int GetHashCode ( ) => base.GetHashCode ( );
	}
}