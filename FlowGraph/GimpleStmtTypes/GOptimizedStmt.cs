using System.Collections.Generic;
using System.Text.RegularExpressions;

#pragma warning disable CS1591

namespace FlowGraph
{
	/// <summary>
	/// Represents a <see cref="GimpleStmtType.GOPTIMIZED"/>.
	/// </summary>
	public class GOptimizedStmt : GimpleStmt
	{
		private static string myPattern = @"(?<assignee>[\w\.]*) = (?<func>\w*)\s*<(?<args>.*)>;";

		public string Assignee { get; private set; }
		public string FuncName { get; private set; }
		public List<string> Args { get; private set; } = new List<string> ( );

		public GOptimizedStmt ( string text )
		{
			this.Text = text;
			StmtType = GimpleStmtType.GOPTIMIZED;
			Pattern = myPattern;
			var match = Regex.Match ( text, myPattern );
			Assignee = match.Groups["assignee"].Value;
			FuncName = match.Groups["func"].Value;
			string strArgs = match.Groups["args"].Value;
			var matches = Regex.Matches ( strArgs, @"((\"".*\"")|([\w\.])(,\s*(\"".*\"")|([\w\.]))*)" );
			Vars.Add ( Assignee );
			foreach ( Match m in matches )
			{
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
		public static bool Matches ( string stmt )
		{
			return Regex.IsMatch ( stmt, myPattern );
		}

		public override string ToString ( )
		{
			return $"{Assignee} = {FuncName} <{string.Join ( ", ", Args )}>;";
		}

		public override List<string> Rename ( string oldName, string newName )
		{
			if ( Assignee == oldName )
				Assignee = newName;
			for ( int i = 0; i < Args.Count; ++i )
				if ( Args[i] == oldName )
					Args[i] = newName;
			return base.Rename ( oldName, newName );
		}

		public override bool Equals ( object obj )
		{
			if ( obj is GAssignStmt )
				return ToString ( ) == ( obj as GAssignStmt ).ToString ( );
			return base.Equals ( obj );
		}

		public override int GetHashCode ( )
		{
			return base.GetHashCode ( );
		}
	}
}