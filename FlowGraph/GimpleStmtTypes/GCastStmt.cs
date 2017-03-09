using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

#pragma warning disable CS1591

namespace FlowGraph
{
	/// <summary>
	/// Represents a <see cref="GimpleStmtType.GCAST"/>.
	/// </summary>
	public class GCastStmt : GimpleStmt
	{
		private static string myPattern = @"(?<assignee>[\w\.\[\]\,\ \:]*) = \((?<cast>.*)\)\s*\&?(?<v>[\w\.\[\]\,\ \:]*);";

		private string _assignee;
		private string _v;

		public string Assignee
		{
			get
			{
				if ( GArrayDereference.Matches ( _assignee ) )
					return ( new GArrayDereference ( _assignee ).Name );
				return _assignee;
			}
			private set
			{
				_assignee = value;
			}
		}
		public string Cast { get; private set; }
		public string V
		{
			get
			{
				if ( GArrayDereference.Matches ( _v ) )
					return ( new GArrayDereference ( _v ).Name );
				return _v;
			}
			private set
			{
				_v = value;
			}
		}

		public GCastStmt ( string text )
		{
			this.Text = text;
			StmtType = GimpleStmtType.GCAST;
			Pattern = myPattern;
			var match = Regex.Match ( text, myPattern );
			Assignee = match.Groups["assignee"].Value;
			Cast = match.Groups["cast"].Value;
			V = match.Groups["v"].Value;
			Vars.AddRange ( new[] { Assignee, V }.Where ( x => IsValidIdentifier ( x ) ) );
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
			return $"{_assignee} = ({Cast}) {_v};";
		}

		public override List<string> Rename ( string oldName, string newName )
		{

			if ( Assignee == oldName )
			{
				if ( GArrayDereference.Matches ( _assignee ) )
				{
					var temp = new GArrayDereference ( _assignee )
					{
						Name = newName
					};
					_assignee = temp.ToString ( );
				}
				else
					_assignee = newName;
			}
			if ( V == oldName )
			{
				if ( GArrayDereference.Matches ( _v ) )
				{
					var temp = new GArrayDereference ( _v )
					{
						Name = newName
					};
					_v = temp.ToString ( );
				}
				else
					_v = newName;
			}
			return base.Rename ( oldName, newName );
		}

		public override bool Equals ( object obj )
		{
			if ( obj is GCastStmt )
				return ToString ( ) == ( obj as GCastStmt ).ToString ( );
			return base.Equals ( obj );
		}

		public override int GetHashCode ( )
		{
			return base.GetHashCode ( );
		}
	}
}