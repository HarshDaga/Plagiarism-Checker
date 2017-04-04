using System;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable CS1591

namespace FlowGraph
{
	using Block = List<string>;

	/// <summary>
	/// Represents a GIMPLE Base Block.
	/// </summary>
	public class GBaseBlock
	{
		/// <summary>
		/// Block Number.
		/// </summary>
		public int Number { get; set; }

		/// <summary>
		/// <see cref="List{T}"/> of <see cref="GimpleStmt"/> containing all the statements used in the block.
		/// </summary>
		public List<GimpleStmt> GStatements { get; set; } = new List<GimpleStmt> ( );

		/// <summary>
		/// <see cref="List{T}"/> of <see cref="GimpleStmt"/> containing all the blocks referenced from <c>this</c> block.
		/// </summary>
		public List<GBaseBlock> OutEdges { get; set; } = new List<GBaseBlock> ( );

		/// <summary>
		/// <see cref="List{T}"/> of <see cref="GimpleStmt"/> containing all the blocks that reference <c>this</c> block.
		/// </summary>
		public List<GBaseBlock> InEdges { get; set; } = new List<GBaseBlock> ( );

		public HashSet<string> Vars { get; private set; } = new HashSet<string> ( );

		/// <summary>
		/// Extract all the <see cref="GBaseBlock"/>s from the given GIMPLE source.
		/// </summary>
		/// <param name="gimple">GIMPLE source file content.</param>
		/// <returns>
		/// <see cref="List{T}"/> of <see cref="GBaseBlock"/> containing all the Base Blocks.
		/// </returns>
		public static List<GBaseBlock> GetBaseBlocks ( List<string> gimple )
		{
			Block block = new Block ( );
			List<Block> blocks = new List<Block> ( );
			bool readingBlock = false;
			foreach ( var line in gimple )
			{
				if ( GBBStmt.Matches ( line ) )
					readingBlock = true;
				if ( line.Length == 0 )
				{
					if ( readingBlock )
						blocks.Add ( block );
					readingBlock = false;
					block = new Block ( );
				}
				if ( readingBlock )
					block.Add ( line.Trim ( ) );
			}

			List<GBaseBlock> baseBlocks = new List<GBaseBlock> ( );
			foreach ( var b in blocks )
				baseBlocks.Add ( new GBaseBlock ( b ) );
			return baseBlocks;
		}

		/// <summary>
		/// Convert a GIMPLE statement <see cref="string"/> to one of the known <see cref="GimpleStmtType"/>s.
		/// </summary>
		/// <param name="stmt">GIMPLE statement as a <see cref="string"/>.</param>
		/// <returns></returns>
		public static GimpleStmt ToGimpleStmt ( string stmt )
		{
			Type[] types =
			{
				typeof (GBBStmt),
				typeof (GCondStmt),
				typeof (GElseStmt),
				typeof (GGotoStmt),
                //typeof (GLabelStmt),
                //typeof (GSwitchStmt),
                typeof (GCallStmt),
				typeof (GPhiStmt),
				typeof (GAssignStmt),
				typeof (GOptimizedStmt),
				typeof (GCastStmt),
				typeof (GReturnStmt)
			};
			foreach ( var type in types )
			{
				if ( Convert.ToBoolean ( type.GetMethod ( "Matches" ).Invoke ( null, new object[] { stmt } ) ) )
					return type.GetConstructor ( new[] { typeof ( string ) } ).Invoke ( new object[] { stmt } ) as GimpleStmt;
			}
			return null;
		}

		private void ReorderPhiStatements ( )
		{
			int start = 1;
			int end = 1;
			for ( ; end < GStatements.Count; ++end )
				if ( !( GStatements[end] is GPhiStmt ) )
					break;

			GStatements =
				GStatements.Take ( start )
				 .Concat ( GStatements.GetRange ( start, end - start ).OrderBy ( s => ( s as GPhiStmt ).Assignee ) )
				 .Concat ( GStatements.GetRange ( end, GStatements.Count - end ) ).
				 ToList ( );

			RenumberLines ( );
		}

		/// <summary>
		/// Create a <see cref="GBaseBlock"/> from the GIMPLE source containing content of the current Base Block.
		/// </summary>
		/// <param name="block">GIMPLE source for current block.</param>
		public GBaseBlock ( List<string> block )
		{
			if ( block.Count == 0 )
				throw new ArgumentOutOfRangeException ( );
			var gBlockDecl = new GBBStmt ( block[0] );
			Number = gBlockDecl.Number;
			int line = 0;
			foreach ( var stmt in block )
			{
				var gStmt = ToGimpleStmt ( stmt );
				if ( gStmt != null )
				{
					gStmt.Linenum = line;
					GStatements.Add ( gStmt );
					Vars.UnionWith ( gStmt.Vars );
				}
				++line;
			}

			ReorderPhiStatements ( );
		}

		public void RenumberLines ( )
		{
			for ( int i = 0; i != GStatements.Count; ++i )
				GStatements[i].Linenum = i;
		}

		public HashSet<string> Rename ( string oldName, string newName )
		{
			Vars.Clear ( );
			GStatements.ForEach ( stmt => Vars.UnionWith ( stmt.Rename ( oldName, newName ) ) );
			return Vars;
		}

		/// <summary>
		/// Find and return the <see cref="GBBStmt.Number"/> of all <see cref="GimpleStmtType.GGOTO"/> statements.
		/// </summary>
		/// <returns></returns>
		public List<int> GetOutEdges ( )
		{
			var gotos = GStatements.OfType<GGotoStmt> ( ).Select ( stmt => stmt.Number ).ToList ( );
			if ( !( GStatements.Last ( ) is GGotoStmt ) )
				gotos.Add ( Number + 1 );
			return gotos;
		}

		/// <summary>
		/// Compare both blocks by comparing their <see cref="GimpleStmt"/>s using <code>SequenceEqual</code>.
		/// </summary>
		/// <param name="bb1">LHS</param>
		/// <param name="bb2">RHS</param>
		/// <returns></returns>
		public static bool operator == ( GBaseBlock bb1, GBaseBlock bb2 )
		{
			if ( Object.ReferenceEquals ( bb1, null ) )
				return Object.ReferenceEquals ( bb2, null );
			else if ( Object.ReferenceEquals ( bb2, null ) )
				return false;

			return bb1.GStatements.SequenceEqual ( bb2.GStatements );
		}

		/// <summary>
		/// Negation of <see cref="operator =="/>.
		/// </summary>
		/// <param name="bb1">LHS</param>
		/// <param name="bb2">RHS</param>
		/// <returns></returns>
		public static bool operator != ( GBaseBlock bb1, GBaseBlock bb2 )
		{
			return !( bb1 == bb2 );
		}

		/// <summary>
		/// Compare to <paramref name="o"/>.
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public override bool Equals ( object o )
		{
			if ( o is GBaseBlock )
				return ( o as GBaseBlock ) == this;
			else
				return false;
		}

		public override int GetHashCode ( ) => base.GetHashCode ( );

		public override string ToString ( ) => string.Join ( "\n", GStatements );
	}
}