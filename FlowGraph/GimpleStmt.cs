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
		public int linenum { get; set; }

		/// <summary>
		/// Type of statement.
		/// </summary>
		public GimpleStmtType stmtType { get; set; }

		/// <summary>
		/// Raw text copied from the source GIMPLE.
		/// </summary>
		public string text { get; set; }

		/// <summary>
		/// Regex pattern to identify the current <see cref="GimpleStmtType"/>.
		/// </summary>
		protected string pattern { get; set; }

		/// <summary>
		/// List of all the variables used in current statement as a <see cref="List{T}"/> of <see cref="string"/>.
		/// </summary>
		public List<string> vars { get; private set; } = new List<string> ( );

		public static bool isValidIdentifier ( string var )
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
			for ( int i = 0; i < vars.Count; ++i )
				if ( vars[i] == oldName )
					vars[i] = newName;
			return vars;
		}

		/// <summary>
		/// Return the current statement as a <see cref="string"/> rebuilt using the saved variable names.
		/// </summary>
		/// <returns></returns>
		public override string ToString ( )
		{
			return text;
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
		public int number { get; private set; }

		public GBBStmt ( string text )
		{
			this.text = text;
			stmtType = GimpleStmtType.GBB;
			pattern = myPattern;
			var match = Regex.Match ( text, myPattern );
			number = Convert.ToInt32 ( match.Groups["number"].Value );
		}

		/// <summary>
		/// Compare given <paramref name="stmt"/> to <see cref="myPattern"/> using <see cref="Regex"/>.
		/// </summary>
		/// <param name="stmt"></param>
		/// <returns></returns>
		public static bool matches ( string stmt )
		{
			return Regex.IsMatch ( stmt, myPattern );
		}

		public override string ToString ( )
		{
			return $"<bb {number}>:";
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
	/// Represents a <see cref="GimpleStmtType.GCOND"/>.
	/// </summary>
	public class GCondStmt : GimpleStmt
	{
		private static string myPattern = @"if \((?<op1>[\w\.]*) (?<op>\S*) (?<op2>[\w\.]*)\)";

		public string op1 { get; private set; }
		public string op2 { get; private set; }
		public string op { get; private set; }

		public GCondStmt ( string text )
		{
			this.text = text;
			stmtType = GimpleStmtType.GCOND;
			pattern = myPattern;
			var match = Regex.Match ( text, myPattern );
			op1 = match.Groups["op1"].Value;
			op2 = match.Groups["op2"].Value;
			op = match.Groups["op"].Value;
			vars.AddRange ( new[] { op1, op2 }.Where ( x => isValidIdentifier ( x ) ) );
		}


		/// <summary>
		/// Compare given <paramref name="stmt"/> to <see cref="myPattern"/> using <see cref="Regex"/>.
		/// </summary>
		/// <param name="stmt"></param>
		/// <returns></returns>
		public static bool matches ( string stmt )
		{
			return Regex.IsMatch ( stmt, myPattern );
		}

		public override string ToString ( )
		{
			return $"if ({op1} {op} {op2})";
		}

		public override List<string> Rename ( string oldName, string newName )
		{
			if ( op1 == oldName )
				op1 = newName;
			if ( op2 == oldName )
				op2 = newName;
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
			this.text = text;
			stmtType = GimpleStmtType.GELSE;
			pattern = myPattern;
		}


		/// <summary>
		/// Compare given <paramref name="stmt"/> to <see cref="myPattern"/> using <see cref="Regex"/>.
		/// </summary>
		/// <param name="stmt"></param>
		/// <returns></returns>
		public static bool matches ( string stmt )
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

		public int number { get; private set; }

		public GGotoStmt ( string text )
		{
			this.text = text;
			stmtType = GimpleStmtType.GGOTO;
			pattern = myPattern;
			var match = Regex.Match ( text, myPattern );
			number = Convert.ToInt32 ( match.Groups["number"].Value );
		}


		/// <summary>
		/// Compare given <paramref name="stmt"/> to <see cref="myPattern"/> using <see cref="Regex"/>.
		/// </summary>
		/// <param name="stmt"></param>
		/// <returns></returns>
		public static bool matches ( string stmt )
		{
			return Regex.IsMatch ( stmt, myPattern );
		}

		public override string ToString ( )
		{
			return $"goto <bb {number}>;";
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
	/// Represents a <see cref="GimpleStmtType.GLABEL"/>.
	/// </summary>
	public class GLabelStmt : GimpleStmt
	{
		private static string myPattern = "goto <bb [0-9]*>;";

		public GLabelStmt ( string text )
		{
			this.text = text;
			stmtType = GimpleStmtType.GLABEL;
			pattern = myPattern;
			throw new NotImplementedException ( );
		}


		/// <summary>
		/// Compare given <paramref name="stmt"/> to <see cref="myPattern"/> using <see cref="Regex"/>.
		/// </summary>
		/// <param name="stmt"></param>
		/// <returns></returns>
		public static bool matches ( string stmt )
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
			this.text = text;
			stmtType = GimpleStmtType.GSWITCH;
			pattern = myPattern;
			throw new NotImplementedException ( );
		}


		/// <summary>
		/// Compare given <paramref name="stmt"/> to <see cref="myPattern"/> using <see cref="Regex"/>.
		/// </summary>
		/// <param name="stmt"></param>
		/// <returns></returns>
		public static bool matches ( string stmt )
		{
			return Regex.IsMatch ( stmt, myPattern );
		}

		public override List<string> Rename ( string oldName, string newName )
		{
			return base.Rename ( oldName, newName );
		}
	}
	
	/// <summary>
	/// Represents a <see cref="GimpleStmtType.GCALL"/>.
	/// </summary>
	public class GCallStmt : GimpleStmt
	{
		private static string myPattern = @"(?<funcname>\S*) \((?<args>.*)\);";

		public string funcname { get; private set; }
		public List<string> args { get; private set; } = new List<string> ( );

		public GCallStmt ( string text )
		{
			this.text = text;
			stmtType = GimpleStmtType.GCALL;
			pattern = myPattern;
			var match = Regex.Match ( text, myPattern );
			funcname = match.Groups["funcname"].Value;
			string strArgs = match.Groups["args"].Value;
			var matches = Regex.Matches ( strArgs, @"((\"".*\"")|([\w\.])(,\s*(\"".*\"")|([\w\.]))*)" );
			foreach ( Match m in matches )
			{
				args.Add ( m.Value );
				if ( isValidIdentifier ( m.Value ) )
					vars.Add ( m.Value );
			}
		}


		/// <summary>
		/// Compare given <paramref name="stmt"/> to <see cref="myPattern"/> using <see cref="Regex"/>.
		/// </summary>
		/// <param name="stmt"></param>
		/// <returns></returns>
		public static bool matches ( string stmt )
		{
			return Regex.IsMatch ( stmt, myPattern );
		}

		public override string ToString ( )
		{
			return $"{funcname} ({string.Join ( ", ", args )});";
		}

		public override List<string> Rename ( string oldName, string newName )
		{
			for ( int i = 0; i < args.Count; ++i )
				if ( args[i] == oldName )
					args[i] = newName;
			return base.Rename ( oldName, newName );
		}

		public override bool Equals ( object obj )
		{
			if ( obj is GCallStmt )
			{
				var stmt = obj as GCallStmt;
				if ( funcname != stmt.funcname )
					return false;
				return vars.SequenceEqual ( stmt.vars );
			}
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

		public string assignee { get; private set; }
		public string var1 { get; private set; }
		public string var2 { get; private set; }
		public string bb1 { get; private set; }
		public string bb2 { get; private set; }

		public GPhiStmt ( string text )
		{
			this.text = text;
			stmtType = GimpleStmtType.GPHI;
			pattern = myPattern;
			var match = Regex.Match ( text, myPattern );
			assignee = match.Groups["assignee"].Value;
			var1 = match.Groups["var1"].Value;
			var2 = match.Groups["var2"].Value;
			bb1 = match.Groups["bb1"].Value;
			bb2 = match.Groups["bb2"].Value;
			vars.AddRange ( new[] { assignee, var1, var2 }.Where ( x => isValidIdentifier ( x ) ) );
		}


		/// <summary>
		/// Compare given <paramref name="stmt"/> to <see cref="myPattern"/> using <see cref="Regex"/>.
		/// </summary>
		/// <param name="stmt"></param>
		/// <returns></returns>
		public static bool matches ( string stmt )
		{
			return Regex.IsMatch ( stmt, myPattern );
		}

		public override string ToString ( )
		{
			string second = var2 == "" ? "" : $", {var2}({bb2})";
			return $"# {assignee} = PHI <{var1}({bb1}){second}>";
		}

		public override List<string> Rename ( string oldName, string newName )
		{
			if ( assignee == oldName )
				assignee = newName;
			if ( var1 == oldName )
				var1 = newName;
			if ( object.Equals ( var2, oldName ) )
				var2 = newName;
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
		private static string myPattern = @"(?<assignee>[\w\.]*) = (?<var1>[\w\.]*)( (?<op>\S*) (?<var2>[\w\.]*))?;";

		public string assignee { get; private set; }
		public string var1 { get; private set; }
		public string var2 { get; private set; }
		public string op { get; private set; }

		public GAssignStmt ( string text )
		{
			this.text = text;
			stmtType = GimpleStmtType.GASSIGN;
			pattern = myPattern;
			var match = Regex.Match ( text, myPattern );
			assignee = match.Groups["assignee"].Value;
			var1 = match.Groups["var1"].Value;
			var2 = match.Groups["var2"].Value;
			op = match.Groups["op"].Value;
			vars.AddRange ( new[] { assignee, var1, var2 }.Where ( x => isValidIdentifier ( x ) ) );
		}


		/// <summary>
		/// Compare given <paramref name="stmt"/> to <see cref="myPattern"/> using <see cref="Regex"/>.
		/// </summary>
		/// <param name="stmt"></param>
		/// <returns></returns>
		public static bool matches ( string stmt )
		{
			return Regex.IsMatch ( stmt, myPattern );
		}

		public override string ToString ( )
		{
			string second = var2 == "" ? "" : $" {op} {var2}";
			return $"{assignee} = {var1}{second};";
		}

		public override List<string> Rename ( string oldName, string newName )
		{
			if ( assignee == oldName )
				assignee = newName;
			if ( var1 == oldName )
				var1 = newName;
			if ( object.Equals ( var2, oldName ) )
				var2 = newName;
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
	/// Represents a <see cref="GimpleStmtType.GCAST"/>.
	/// </summary>
	public class GCastStmt : GimpleStmt
	{
		private static string myPattern = @"(?<assignee>[\w\.]*) = \((?<cast>.*)\)\s*(?<v>[\w\.]*);";

		public string assignee { get; private set; }
		public string cast { get; private set; }
		public string v { get; private set; }

		public GCastStmt ( string text )
		{
			this.text = text;
			stmtType = GimpleStmtType.GCAST;
			pattern = myPattern;
			var match = Regex.Match ( text, myPattern );
			assignee = match.Groups["assignee"].Value;
			cast = match.Groups["cast"].Value;
			v = match.Groups["v"].Value;
			vars.AddRange ( new[] { assignee, v }.Where ( x => isValidIdentifier ( x ) ) );
		}


		/// <summary>
		/// Compare given <paramref name="stmt"/> to <see cref="myPattern"/> using <see cref="Regex"/>.
		/// </summary>
		/// <param name="stmt"></param>
		/// <returns></returns>
		public static bool matches ( string stmt )
		{
			return Regex.IsMatch ( stmt, myPattern );
		}

		public override string ToString ( )
		{
			return $"{assignee} = ({cast}) {v};";
		}

		public override List<string> Rename ( string oldName, string newName )
		{
			if ( assignee == oldName )
				assignee = newName;
			if ( v == oldName )
				v = newName;
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

		public string retval { get; private set; }

		public GReturnStmt ( string text )
		{
			this.text = text;
			stmtType = GimpleStmtType.GRETURN;
			pattern = myPattern;
			var match = Regex.Match ( text, myPattern );
			retval = match.Groups["retval"].Value;
			vars.AddRange ( new[] { retval }.Where ( x => isValidIdentifier ( x ) ) );
		}
		
		/// <summary>
		/// Compare given <paramref name="stmt"/> to <see cref="myPattern"/> using <see cref="Regex"/>.
		/// </summary>
		/// <param name="stmt"></param>
		/// <returns></returns>
		public static bool matches ( string stmt )
		{
			return Regex.IsMatch ( stmt, myPattern );
		}

		public override string ToString ( )
		{
			string val = retval == "" ? "" : $" {retval}";
			return $"return{val};";
		}

		public override List<string> Rename ( string oldName, string newName )
		{
			if ( object.Equals ( retval, oldName ) )
				retval = newName;
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
}