using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

#pragma warning disable CS1591

namespace FlowGraph
{
	/// <summary>
	/// GIMPLE Statement Types
	/// </summary>
	public enum GimpleStmtType
	{
		/// <summary>
		/// Base Block
		/// </summary>
		[Description ( "Base Block" )]
		GBB,

		/// <summary>
		/// Conditional Statements
		/// </summary>
		[Description ( "If" )]
		GCOND,

		/// <summary>
		/// Else statement
		/// </summary>
		[Description ( "Else" )]
		GELSE,

		/// <summary>
		/// Goto statement
		/// </summary>
		[Description ( "Go to" )]
		GGOTO,

		/// <summary>
		/// Label
		/// </summary>
		[Description ( "Label" )]
		GLABEL,

		/// <summary>
		/// Switch statement
		/// </summary>
		[Description ( "Switch" )]
		GSWITCH,

		/// <summary>
		/// Function call
		/// </summary>
		[Description ( "Call" )]
		GCALL,

		/// <summary>
		/// PHI statement
		/// </summary>
		[Description ( "PHI" )]
		GPHI,

		/// <summary>
		/// Assignment statement
		/// </summary>
		[Description ( "Assignment" )]
		GASSIGN,

		/// <summary>
		/// Optimized statement
		/// </summary>
		[Description ( "Optimized" )]
		GOPTIMIZED,

		/// <summary>
		/// Casting statement
		/// </summary>
		[Description ( "Casting" )]
		GCAST,

		/// <summary>
		/// Return statement
		/// </summary>
		[Description ( "Return" )]
		GRETURN
	}

	/// <summary>
	/// <para>
	/// GIMPLE Statement.
	/// </para>
	/// <para>
	/// Base class for all GIMPLE statements.
	/// </para>
	/// </summary>
	public abstract class GimpleStmt
	{

		/// <summary>
		/// Line number.
		/// </summary>
		public int Linenum { get; set; }

		/// <summary>
		/// Type of statement.
		/// </summary>
		public GimpleStmtType StmtType { get; set; }

		/// <summary>
		/// Raw text copied from the source GIMPLE.
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		/// Regex pattern to identify the current <see cref="GimpleStmtType"/>.
		/// </summary>
		protected string Pattern { get; set; }

		/// <summary>
		/// List of all the variables used in current statement as a <see cref="List{T}"/> of <see cref="string"/>.
		/// </summary>
		public List<string> Vars { get; private set; } = new List<string> ( );

		public static bool IsValidIdentifier ( string var )
		{
			return var.Length > 0 && ( char.IsLetter ( var[0] ) || var[0] == '_' );
		}

		/// <summary>
		/// Change all variable names from <paramref name="oldName"/> to <paramref name="newName"/>.
		/// </summary>
		/// <param name="oldName">Old variable name.</param>
		/// <param name="newName">New variable name.</param>
		/// <returns>
		/// <see cref="List{T}"/> of <see cref="string"/> containing all variable names used in this statement.
		/// </returns>
		public virtual List<string> Rename ( string oldName, string newName )
		{
			for ( int i = 0; i < Vars.Count; ++i )
				if ( Vars[i] == oldName )
					Vars[i] = newName;
			return Vars;
		}

		/// <summary>
		/// Return the current statement as a <see cref="string"/> rebuilt using the saved variable names.
		/// </summary>
		/// <returns></returns>
		public override string ToString ( )
		{
			return Text;
		}

		/// <summary>
		/// Cast <paramref name="obj"/> to a <see cref="GimpleStmt"/> and compare them as a <see cref="string"/>.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals ( object obj )
		{
			if ( obj is GimpleStmt )
				return ToString ( ) == ( obj as GimpleStmt ).ToString ( );
			return base.Equals ( obj );
		}

		public override int GetHashCode ( )
		{
			return base.GetHashCode ( );
		}

		/// <summary>
		/// Check if both are null first and then if both are same using <see cref="Equals(object)"/>.
		/// </summary>
		/// <param name="stmt1">LHS</param>
		/// <param name="stmt2">RHS</param>
		/// <returns></returns>
		public static bool operator == ( GimpleStmt stmt1, GimpleStmt stmt2 )
		{
			if ( ReferenceEquals ( stmt1, null ) )
				return ReferenceEquals ( stmt2, null );
			return stmt1.Equals ( stmt2 );
		}

		/// <summary>
		/// Negation of <see cref="operator =="/>.
		/// </summary>
		/// <param name="stmt1">LHS</param>
		/// <param name="stmt2">RHS</param>
		/// <returns></returns>
		public static bool operator != ( GimpleStmt stmt1, GimpleStmt stmt2 )
		{
			return !( stmt1 == stmt2 );
		}
	}

	/// <summary>
	/// Represents a <see cref="GimpleStmtType.GBB"/>.
	/// </summary>
	public class GBBStmt : GimpleStmt
	{
		private static string myPattern = "<bb (?<number>[0-9]+)>:";

		/// <summary>
		/// Base Block number.
		/// </summary>
		public int Number { get; private set; }

		public GBBStmt ( string text )
		{
			this.Text = text;
			StmtType = GimpleStmtType.GBB;
			Pattern = myPattern;
			var match = Regex.Match ( text, myPattern );
			Number = Convert.ToInt32 ( match.Groups["number"].Value );
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
			return $"<bb {Number}>:";
		}

		public override List<string> Rename ( string oldName, string newName )
		{
			return base.Rename ( oldName, newName );
		}

		public override bool Equals ( object obj )
		{
			if ( obj is GBBStmt )
				return ToString ( ) == ( obj as GBBStmt ).ToString ( );
			return base.Equals ( obj );
		}

		public override int GetHashCode ( )
		{
			return base.GetHashCode ( );
		}
	}

	/// <summary>
	/// Represents a <see cref="GimpleStmtType.GPHI"/>.
	/// </summary>
	public class GPhiStmt : GimpleStmt
	{
		private static string myPattern = @"# (?<assignee>[\w\.]*) = PHI <(?<var1>[\w\.]*)\((?<bb1>\S*)\)(, (?<var2>[\w\.]*)\((?<bb2>\S*)\))?>";

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
		public static bool Matches ( string stmt )
		{
			return Regex.IsMatch ( stmt, myPattern );
		}

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

		public override bool Equals ( object obj )
		{
			if ( obj is GPhiStmt )
				return ToString ( ) == ( obj as GPhiStmt ).ToString ( );
			return base.Equals ( obj );
		}

		public override int GetHashCode ( )
		{
			return base.GetHashCode ( );
		}
	}

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
			StmtType = GimpleStmtType.GCALL;
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

	/// <summary>
	/// Represents a <see cref="GimpleStmtType.GELSE"/>.
	/// </summary>
	public class GElseStmt : GimpleStmt
	{
		private static string myPattern = "else";

		public GElseStmt ( string text )
		{
			this.Text = text;
			StmtType = GimpleStmtType.GELSE;
			Pattern = myPattern;
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
			return $"else";
		}

		public override List<string> Rename ( string oldName, string newName )
		{
			return base.Rename ( oldName, newName );
		}

		public override bool Equals ( object obj )
		{
			if ( obj is GElseStmt )
				return ToString ( ) == ( obj as GElseStmt ).ToString ( );
			return base.Equals ( obj );
		}

		public override int GetHashCode ( )
		{
			return base.GetHashCode ( );
		}
	}

	/// <summary>
	/// Represents a <see cref="GimpleStmtType.GGOTO"/>.
	/// </summary>
	public class GGotoStmt : GimpleStmt
	{
		private static string myPattern = "goto <bb (?<number>[0-9]*)>;";

		public int Number { get; private set; }

		public GGotoStmt ( string text )
		{
			this.Text = text;
			StmtType = GimpleStmtType.GGOTO;
			Pattern = myPattern;
			var match = Regex.Match ( text, myPattern );
			Number = Convert.ToInt32 ( match.Groups["number"].Value );
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
			return $"goto <bb {Number}>;";
		}

		public override List<string> Rename ( string oldName, string newName )
		{
			return base.Rename ( oldName, newName );
		}

		public override bool Equals ( object obj )
		{
			if ( obj is GGotoStmt )
				return ToString ( ) == ( obj as GGotoStmt ).ToString ( );
			return base.Equals ( obj );
		}

		public override int GetHashCode ( )
		{
			return base.GetHashCode ( );
		}
	}

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
		public static bool Matches ( string stmt )
		{
			return Regex.IsMatch ( stmt, myPattern );
		}

		public override string ToString ( )
		{
			return $"{FuncName} ({string.Join ( ", ", Args )});";
		}

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

		public override int GetHashCode ( )
		{
			return base.GetHashCode ( );
		}
	}

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

	/// <summary>
	/// Represents a <see cref="GimpleStmtType.GRETURN"/>.
	/// </summary>
	public class GReturnStmt : GimpleStmt
	{
		private static string myPattern = @"return( (?<retval>\S*))?;";

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
		public static bool Matches ( string stmt )
		{
			return Regex.IsMatch ( stmt, myPattern );
		}

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

		public override bool Equals ( object obj )
		{
			if ( obj is GReturnStmt )
				return ToString ( ) == ( obj as GReturnStmt ).ToString ( );
			return base.Equals ( obj );
		}

		public override int GetHashCode ( )
		{
			return base.GetHashCode ( );
		}
	}

	/// <summary>
	/// Represents a <see cref="GimpleStmtType.GLABEL"/>.
	/// </summary>
	public class GLabelStmt : GimpleStmt
	{
		private static string myPattern = "goto <bb [0-9]*>;";

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
		public static bool Matches ( string stmt )
		{
			return Regex.IsMatch ( stmt, myPattern );
		}

		public override List<string> Rename ( string oldName, string newName )
		{
			return base.Rename ( oldName, newName );
		}
	}

	/// <summary>
	/// Represents a <see cref="GimpleStmtType.GSWITCH"/>.
	/// </summary>
	public class GSwitchStmt : GimpleStmt
	{
		private static string myPattern = "goto <bb [0-9]*>;";

		public GSwitchStmt ( string text )
		{
			this.Text = text;
			StmtType = GimpleStmtType.GSWITCH;
			Pattern = myPattern;
			throw new NotImplementedException ( );
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

		public override List<string> Rename ( string oldName, string newName )
		{
			return base.Rename ( oldName, newName );
		}
	}
}