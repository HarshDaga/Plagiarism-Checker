using System;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable CS1591

namespace FlowGraph
{
	using Block = List<string>;

	/// <summary>
	/// Represents the declaration of a GIMPLE variable.
	/// </summary>
	public class GVar
	{
		public class DefineUseChain
		{
			public class DefineUseChainEntry
			{
				public int block;
				public int line;

				public DefineUseChainEntry ( int block, int line )
				{
					this.block = block;
					this.line = line;
				}

				public static bool operator == ( DefineUseChainEntry lhs, DefineUseChainEntry rhs )
				{
					return lhs.block == rhs.block && lhs.line == rhs.line;
				}

				public static bool operator != ( DefineUseChainEntry lhs, DefineUseChainEntry rhs )
				{
					return lhs.block != rhs.block || lhs.line != rhs.line;
				}

				public override bool Equals ( object obj )
				{
					if ( obj is DefineUseChainEntry )
						return this == ( obj as DefineUseChainEntry );
					return base.Equals ( obj );
				}

				public override int GetHashCode ( )
				{
					return ( block << 16 ) | line;
				}

				public override string ToString ( )
				{
					return $"Block: {block} Line: {line}";
				}
			}

			public HashSet<DefineUseChainEntry> data = new HashSet<DefineUseChainEntry> ( );

			public void add ( int block, int line )
			{
				data.Add ( new DefineUseChainEntry ( block, line ) );
			}

			public static bool operator == ( DefineUseChain lhs, DefineUseChain rhs )
			{
				if ( lhs.data.Count != rhs.data.Count )
					return false;
				return lhs.data.SetEquals ( rhs.data );
			}

			public static bool operator != ( DefineUseChain lhs, DefineUseChain rhs )
			{
				return !( lhs == rhs );
			}

			public override bool Equals ( object obj )
			{
				if ( obj is DefineUseChain )
					return this == ( obj as DefineUseChain );
				return base.Equals ( obj );
			}

			public override int GetHashCode ( )
			{
				return base.GetHashCode ( );
			}

			public override string ToString ( )
			{
				return string.Join ( "\n", data );
			}

		}

		public string name { get; set; }
		public string type { get; set; }

		public DefineUseChain ducAssignments { get; set; }
		public DefineUseChain ducReferences { get; set; }

		public GVar ( )
		{
			ducAssignments = new DefineUseChain ( );
			ducReferences = new DefineUseChain ( );
		}

		public bool isSubsetOf ( GVar v )
		{
			return ( ducAssignments.data.IsSubsetOf ( v.ducAssignments.data ) &&
				ducReferences.data.IsSubsetOf ( v.ducReferences.data ) );
		}

		public decimal map ( GVar v )
		{
			decimal result = 0m;
			{
				var common = v.ducAssignments.data.Intersect ( ducAssignments.data ).ToList ( ).Count;
				result += ( 1m * common ) / ( v.ducAssignments.data.Count + ducAssignments.data.Count );
			}
			{
				var common = v.ducReferences.data.Intersect ( ducReferences.data ).ToList ( ).Count;
				result += ( 1m * common ) / ( v.ducReferences.data.Count + ducReferences.data.Count );
			}
			return result;
		}

		public decimal map ( List<GVar> vars )
		{
			var unionAssignments = new HashSet<DefineUseChain.DefineUseChainEntry> ( vars.SelectMany ( v => v.ducAssignments.data ) );
			var unionRefereces = new HashSet<DefineUseChain.DefineUseChainEntry> ( vars.SelectMany ( v => v.ducReferences.data ) );
			decimal result = 0m;
			{
				var common = unionAssignments.Intersect ( ducAssignments.data ).ToList ( ).Count;
				result += ( 1m * common ) / ( unionAssignments.Count + ducAssignments.data.Count );
			}
			{
				var common = unionRefereces.Intersect ( ducReferences.data ).ToList ( ).Count;
				result += ( 1m * common ) / ( unionRefereces.Count + ducReferences.data.Count );
			}
			return result;
		}

		public static bool operator == ( GVar lhs, GVar rhs )
		{
			return lhs.name == rhs.name;
		}

		public static bool operator != ( GVar lhs, GVar rhs )
		{
			return lhs.name != rhs.name;
		}

		public override bool Equals ( object obj )
		{
			if ( obj is GVar )
				return ( obj as GVar ).name == name;
			return base.Equals ( obj );
		}

		public override int GetHashCode ( )
		{
			return name.GetHashCode ( );
		}

		public override string ToString ( )
		{
			return $"{type} {name}";
		}
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
				typeof (GCastStmt),
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
			int line = 1;
			foreach ( var stmt in block )
			{
				var gStmt = toGimpleStmt ( stmt );
				if ( gStmt != null )
				{
					gStmt.linenum = line;
					gStatements.Add ( gStmt );
					vars.UnionWith ( gStmt.vars );
				}
				++line;
			}
		}

		public void RenumberLines ( )
		{
			for ( int i = 0; i != gStatements.Count; ++i )
				gStatements[i].linenum = i + 1;
		}

		public HashSet<string> Rename ( string oldName, string newName )
		{
			vars.Clear ( );
			gStatements.ForEach ( stmt => vars.UnionWith ( stmt.Rename ( oldName, newName ) ) );
			return vars;
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

		public override string ToString ( )
		{
			return string.Join ( "\n", gStatements );
		}
	}
}