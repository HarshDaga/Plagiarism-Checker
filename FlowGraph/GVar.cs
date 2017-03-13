using System.Collections.Generic;
using System.Linq;

#pragma warning disable CS1591

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

		private HashSet<DefineUseChainEntry> data = new HashSet<DefineUseChainEntry> ( );

		public HashSet<DefineUseChainEntry> Data { get => this.data; set => this.data = value; }
		public int Count => Data.Count;

		public DefineUseChain ( )
		{
		}

		public DefineUseChain ( IEnumerable<DefineUseChain> ducs )
		{
			foreach ( var duc in ducs )
				data.UnionWith ( duc.data );
		}

		public void Add ( int block, int line ) => Data.Add ( new DefineUseChainEntry ( block, line ) );

		public decimal Similarity ( DefineUseChain duc )
		{
			int totalAssignments = this.Count + duc.Count;
			if ( totalAssignments == 0 )
				return 0m;

			var common = this.Intersect ( duc ).Count;
			return ( 1m * common ) / ( totalAssignments );
		}

		public DefineUseChain Intersect ( DefineUseChain duc )
		{
			DefineUseChain result = new DefineUseChain
			{
				Data = new HashSet<DefineUseChainEntry> ( this.Data.Intersect ( duc.Data ) )
			};
			return result;
		}

		public DefineUseChain Union ( DefineUseChain duc )
		{
			DefineUseChain result = new DefineUseChain
			{
				Data = new HashSet<DefineUseChainEntry> ( this.Data.Union ( duc.Data ) )
			};
			return result;
		}

		public bool IsSubsetOf ( DefineUseChain duc ) => Data.IsSubsetOf ( duc.Data );

		public bool IsProperSubsetOf ( DefineUseChain duc ) => Data.IsProperSubsetOf ( duc.Data );

		public bool IsSupersetOf ( DefineUseChain duc ) => Data.IsSupersetOf ( duc.Data );

		public bool IsProperSupersetOf ( DefineUseChain duc ) => Data.IsProperSupersetOf ( duc.Data );

		public static bool operator == ( DefineUseChain lhs, DefineUseChain rhs )
		{
			if ( lhs.Data.Count != rhs.Data.Count )
				return false;
			return lhs.Data.SetEquals ( rhs.Data );
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

		public override int GetHashCode ( ) => base.GetHashCode ( );

		public override string ToString ( ) => string.Join ( "\n", Data );

	}

	public string Name { get; set; }
	public string Type { get; set; }

	public DefineUseChain DucAssignments { get; set; } = new DefineUseChain ( );
	public DefineUseChain DucReferences { get; set; } = new DefineUseChain ( );

	public GVar ( )
	{
	}

	public bool IsSubsetOf ( GVar v )
	{
		return DucAssignments.IsSubsetOf ( v.DucAssignments ) &&
			DucReferences.IsSubsetOf ( v.DucReferences );
	}

	public decimal Map ( GVar v ) => Map ( new List<GVar> { v } );

	public decimal Map ( List<GVar> vars )
	{
		var unionAssignments = new DefineUseChain ( vars.Select ( v => v.DucAssignments ) );
		var unionRefereces = new DefineUseChain ( vars.Select ( v => v.DucReferences ) );
		int totalAssignments = unionAssignments.Count + DucAssignments.Count;
		int totalReferences = unionRefereces.Count + DucReferences.Count;

		decimal result = 0m;

		result += unionAssignments.Similarity ( DucAssignments );
		result += unionRefereces.Similarity ( DucReferences );

		if ( totalAssignments == 0 || totalReferences == 0 )
			result *= 2m;

		return result;
	}

	public static bool operator == ( GVar lhs, GVar rhs ) => lhs.Name == rhs.Name;

	public static bool operator != ( GVar lhs, GVar rhs ) => lhs.Name != rhs.Name;

	public override bool Equals ( object obj )
	{
		if ( obj is GVar )
			return ( obj as GVar ).Name == Name;
		return base.Equals ( obj );
	}

	public override int GetHashCode ( ) => Name.GetHashCode ( );

	public override string ToString ( ) => $"{Type} {Name}";
}