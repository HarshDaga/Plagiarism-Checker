using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

#pragma warning disable CS1591

namespace FlowGraph
{
	/// <summary>
	/// Represents a <see cref="GimpleStmtType.GCALL"/>.
	/// </summary>
	public class GCallStmt : GimpleStmt
	{
		private static string myPattern = @"(?<funcname>\S*) \((?<args>.*)\);";

		public string FuncName { get; private set; }
		public List<string> Args { get; private set; } = new List<string> ( );

		public GCallStmt ( string text )
		{
			this.Text = text;
			StmtType = GimpleStmtType.GCALL;
			Pattern = myPattern;
			var match = Regex.Match ( text, myPattern );
			FuncName = match.Groups["funcname"].Value;
			string strArgs = match.Groups["args"].Value;
			var matches = Regex.Matches ( strArgs, @"^(\&\"".*\""\[\d*\])|((\"""".*\"""")|([\w\.]*)(,\s*(\"""".*\"""")|([\w\.]))*)$" );
			foreach ( Match m in matches )
			{
				if ( string.IsNullOrWhiteSpace ( m.Value ) )
					continue;
				Args.Add ( m.Value );
				if ( IsValidIdentifier ( m.Value ) )
					Vars.Add ( m.Value );
			}
		}


		/// <summary>
		/// Compare given <paramref name="stmt"/> to <see cref="myPattern"/> using <see cref="Regex"/>.
		/// </summary>
		/// <param name="stmt"></param>
		/// <returns></returns>
		public static bool Matches ( string stmt ) => Regex.IsMatch ( stmt, myPattern );

		public override string ToString ( ) => $"{FuncName} ({string.Join ( ", ", Args )});";

		public override List<string> Rename ( string oldName, string newName )
		{
			for ( int i = 0; i < Args.Count; ++i )
				if ( Args[i] == oldName )
					Args[i] = newName;
			return base.Rename ( oldName, newName );
		}

		public override bool Equals ( object obj )
		{
			if ( obj is GCallStmt )
			{
				var stmt = obj as GCallStmt;
				if ( FuncName != stmt.FuncName )
					return false;
				return Vars.SequenceEqual ( stmt.Vars );
			}
			return base.Equals ( obj );
		}

		public override int GetHashCode ( ) => base.GetHashCode ( );
	}
}