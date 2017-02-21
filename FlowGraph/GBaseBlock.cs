using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

#pragma warning disable CS1591

namespace FlowGraph
{
	using Block = List<string>;

	/// <summary>
	/// Represents the declaration of a GIMPLE variable.
	/// </summary>
	public class GVar
	{
		public string name { get; set; }
		public string type { get; set; }
	}

	//public class GValue
	//{
	//    public bool isVar { get; set; }
	//    public bool isConstant
	//    {
	//        get
	//        {
	//            return !isVar;
	//        }
	//        set
	//        {
	//            isVar = !value;
	//        }
	//    }
	//    public object Value
	//    {
	//        get
	//        {
	//            if ( isVar )
	//                return gVar;
	//            else
	//                return gConst;
	//        }
	//        private set
	//        {
	//            if ( value is GVar )
	//            {
	//                isVar = true;
	//                gVar = value as GVar;
	//            }
	//            else
	//            {
	//                isVar = false;
	//                gConst = value;
	//            }
	//        }
	//    }
	//    GVar gVar;
	//    object gConst;

	//    public GValue ( GVar gVar )
	//    {
	//        Value = gVar;
	//        gConst = null;
	//    }

	//    public GValue ( string gConst )
	//    {
	//        Value = gConst;
	//        gVar = null;
	//    }
	//}

	/// <summary>
	/// Represents a GIMPLE Base Block.
	/// </summary>
	public class GBaseBlock
	{
		/// <summary>
		/// Block Number.
		/// </summary>
		public int number { get; private set; }

		/// <summary>
		/// <see cref="List{T}"/> of <see cref="GimpleStmt"/> containing all the statements used in the block.
		/// </summary>
		public List<GimpleStmt> gStatements { get; private set; } = new List<GimpleStmt> ( );
		/// <summary>
		/// <see cref="List{T}"/> of <see cref="GimpleStmt"/> containing all the blocks referenced from <c>this</c> block.
		/// </summary>
		public List<GBaseBlock> outEdges { get; set; } = new List<GBaseBlock> ( );
		/// <summary>
		/// <see cref="List{T}"/> of <see cref="GimpleStmt"/> containing all the blocks that reference <c>this</c> block.
		/// </summary>
		public List<GBaseBlock> inEdges { get; set; } = new List<GBaseBlock> ( );
		public HashSet<string> vars { get; private set; } = new HashSet<string> ( );

		/// <summary>
		/// Extract all the <see cref="GBaseBlock"/>s from the given GIMPLE source.
		/// </summary>
		/// <param name="gimple">GIMPLE source file content.</param>
		/// <returns>
		/// <see cref="List{T}"/> of <see cref="GBaseBlock"/> containing all the Base Blocks.
		/// </returns>
		public static List<GBaseBlock> getBaseBlocks ( List<string> gimple )
		{
			Block block = new Block ( );
			List<Block> blocks = new List<Block> ( );
			bool readingBlock = false;
			foreach ( var line in gimple )
			{
				if ( GBBStmt.matches ( line ) )
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
		public static GimpleStmt toGimpleStmt ( string stmt )
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
				typeof (GReturnStmt)
			};
			foreach ( var type in types )
			{
				if ( Convert.ToBoolean ( type.GetMethod ( "matches" ).Invoke ( null, new object[] { stmt } ) ) )
					return type.GetConstructor ( new[] { typeof ( string ) } ).Invoke ( new object[] { stmt } ) as GimpleStmt;
			}
			return null;
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
			number = gBlockDecl.number;
			foreach ( var stmt in block )
			{
				var gStmt = toGimpleStmt ( stmt );
				if ( gStmt != null )
				{
					gStatements.Add ( gStmt );
					vars.UnionWith ( gStmt.vars );
				}
			}
		}

		/// <summary>
		/// Find and return the <see cref="GBBStmt.number"/> of all <see cref="GimpleStmtType.GGOTO"/> statements.
		/// </summary>
		/// <returns></returns>
		public List<int> getOutEdges ( )
		{
			var gotos = gStatements.Where ( x => x is GGotoStmt ).Cast<GGotoStmt> ( ).Select ( stmt => stmt.number ).ToList ( );
			if ( !( gStatements.Last ( ) is GGotoStmt ) )
				gotos.Add ( number + 1 );
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
			return bb1.gStatements.SequenceEqual ( bb2.gStatements );
		}

		/// <summary>
		/// Negation of <see cref="operator =="/>.
		/// </summary>
		/// <param name="bb1">LHS</param>
		/// <param name="bb2">RHS</param>
		/// <returns></returns>
		public static bool operator != ( GBaseBlock bb1, GBaseBlock bb2 )
		{
			return !bb1.gStatements.SequenceEqual ( bb2.gStatements );
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

		public override int GetHashCode ( )
		{
			return base.GetHashCode ( );
		}
	}
}
