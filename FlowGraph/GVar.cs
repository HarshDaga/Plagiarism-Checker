using System;
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

		public HashSet<DefineUseChainEntry> Data
		{
			get => this.data;
			set => this.data = value;
		}
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

		public void Add ( IEnumerable<DefineUseChainEntry> elements )
		{
			foreach ( var element in elements )
				Add ( element.block, element.line );
		}

		private static int CalculateDelta ( List<DefineUseChainEntry> lhs, List<DefineUseChainEntry> rhs )
		{
			int delta = 0;
			int i;
			int limit = Math.Min ( lhs.Count, rhs.Count );

			for ( i = 0; i < limit; i++ )
				if ( lhs[i].line != rhs[i].line )
					break;

			if ( i >= limit )
				return 0;
			delta = rhs[i].line - lhs[i].line;

			for ( ; i < limit; i++ )
				if ( lhs[i].line + delta != rhs[i].line )
					return 0;

			return delta;
		}

		private static List<DefineUseChainEntry> AdjustUsingDelta
			(
			List<DefineUseChainEntry> lhs,
			List<DefineUseChainEntry> rhs,
			int delta
			)
		{
			var adjusted = new List<DefineUseChainEntry> ( );
			int i;

			for ( i = 0; i < Math.Min ( lhs.Count, rhs.Count ); i++ )
				if ( lhs[i].line == rhs[i].line )
					adjusted.Add ( new DefineUseChainEntry ( lhs[i].block, lhs[i].line ) );
				else
					break;

			for ( ; i < lhs.Count; i++ )
				adjusted.Add ( new DefineUseChainEntry ( lhs[i].block, lhs[i].line + delta ) );

			return adjusted;
		}

		public decimal Similarity ( DefineUseChain duc )
		{
			int total = this.Count + duc.Count;
			if ( total == 0 )
				return .5m;

			var lhs_rhs = this.Data
				.Except ( duc.Data )
				.GroupBy ( x => x.block )
				.ToDictionary ( x => x.Key, x => this.Data.Where ( y => y.block == x.Key ).ToList ( ) );
			var rhs_lhs = duc.Data
				.Except ( this.Data )
				.GroupBy ( x => x.block )
				.ToDictionary ( x => x.Key, x => duc.Data.Where ( y => y.block == x.Key ).ToList ( ) );

			var adjusted = new DefineUseChain ( );
			adjusted.Add ( this.Data.Except ( lhs_rhs.SelectMany ( x => x.Value ) ) );
			foreach ( var block in lhs_rhs.Keys )
			{
				if ( !rhs_lhs.ContainsKey ( block ) )
				{
					adjusted.Add ( lhs_rhs[block] );
					continue;
				}

				var delta = CalculateDelta ( lhs_rhs[block], rhs_lhs[block] );
				if ( Math.Abs ( delta ) > 1 )
				{
					adjusted.Add ( lhs_rhs[block] );
					continue;
				}
				var lhs_adjusted = AdjustUsingDelta ( lhs_rhs[block], rhs_lhs[block], delta );
				adjusted.Add ( lhs_adjusted );
			}

			var common = adjusted.Intersect ( duc ).Count;
			return ( 1m * common ) / ( total );
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
		
		public bool IsSubsetOf ( DefineUseChain duc )
		{
			if ( this.Count > duc.Count )
				return false;
			if ( this.Count == 0 && duc.Count == 0 )
				return true;
			decimal similarity = this.Similarity ( duc );
			return similarity == ( 1m * this.Count / ( this.Count + duc.Count ) ) ;
		}

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