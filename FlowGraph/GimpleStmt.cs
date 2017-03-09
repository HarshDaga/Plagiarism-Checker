using System.Collections.Generic;

#pragma warning disable CS1591

namespace FlowGraph
{

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
}