using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

#pragma warning disable CS1591

namespace FlowGraph
{
	/// <summary>
	/// Represents a <see cref="GimpleStmtType.GASSIGN"/>.
	/// </summary>
	public class GAssignStmt : GimpleStmt
	{
		private static string myPattern = @"(?<assignee>[\w\.\[\]\,\ \:]*) = (?<var1>[\w\.\[\]\,\ \:]*)( (?<op>\S*) (?<var2>[\w\.]*))?;";

		private string _assignee;
		private string _var1;
		private string _var2;

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
		public string Var1
		{
			get
			{
				if ( GArrayDereference.Matches ( _var1 ) )
					return ( new GArrayDereference ( _var1 ).Name );
				return _var1;
			}
			private set
			{
				_var1 = value;
			}
		}
		public string Var2
		{
			get
			{
				if ( GArrayDereference.Matches ( _var2 ) )
					return ( new GArrayDereference ( _var2 ).Name );
				return _var2;
			}
			private set
			{
				_var2 = value;
			}
		}
		public string Op { get; private set; }

		public GAssignStmt ( string text )
		{
			this.Text = text;
			StmtType = GimpleStmtType.GASSIGN;
			Pattern = myPattern;
			var match = Regex.Match ( text, myPattern );
			Assignee = match.Groups["assignee"].Value;
			Var1 = match.Groups["var1"].Value;
			Var2 = match.Groups["var2"].Value;
			Op = match.Groups["op"].Value;
			Vars.AddRange ( new[] { Assignee, Var1, Var2 }.Where ( x => IsValidIdentifier ( x ) ) );
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
			string second = Var2 == "" ? "" : $" {Op} {_var2}";
			return $"{_assignee} = {_var1}{second};";
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
			if ( Var1 == oldName )
			{
				if ( GArrayDereference.Matches ( _var1 ) )
				{
					var temp = new GArrayDereference ( _var1 )
					{
						Name = newName
					};
					_var1 = temp.ToString ( );
				}
				else
					_var1 = newName;
			}
			if ( object.Equals ( Var2, oldName ) )
			{
				if ( GArrayDereference.Matches ( _var2 ) )
				{
					var temp = new GArrayDereference ( _var2 )
					{
						Name = newName
					};
					_var2 = temp.ToString ( );
				}
				else
					_var2 = newName;
			}
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