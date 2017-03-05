using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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

	public DefineUseChain ducAssignments { get; set; } = new DefineUseChain ( );
	public DefineUseChain ducReferences { get; set; } = new DefineUseChain ( );

	public GVar ( ) { }

	public bool isSubsetOf ( GVar v )
	{
		return ( ducAssignments.data.IsSubsetOf ( v.ducAssignments.data ) &&
			ducReferences.data.IsSubsetOf ( v.ducReferences.data ) );
	}

	public decimal map ( GVar v )
	{
		decimal result = 0m;
		int totalAssignments = v.ducAssignments.data.Count + ducAssignments.data.Count;
		int totalReferences = v.ducReferences.data.Count + ducReferences.data.Count;
		if ( totalAssignments != 0 )
		{
			var common = v.ducAssignments.data.Intersect ( ducAssignments.data ).ToList ( ).Count;
			result += ( 1m * common ) / ( totalAssignments );
		}
		if ( totalReferences != 0 )
		{
			var common = v.ducReferences.data.Intersect ( ducReferences.data ).ToList ( ).Count;
			result += ( 1m * common ) / ( totalReferences );
		}
		if ( totalAssignments == 0 || totalReferences == 0 )
			result *= 2m;
		return result;
	}

	public decimal map ( List<GVar> vars )
	{
		var unionAssignments = new HashSet<DefineUseChain.DefineUseChainEntry> ( vars.SelectMany ( v => v.ducAssignments.data ) );
		var unionRefereces = new HashSet<DefineUseChain.DefineUseChainEntry> ( vars.SelectMany ( v => v.ducReferences.data ) );
		int totalAssignments = unionAssignments.Count + ducAssignments.data.Count;
		int totalReferences = unionRefereces.Count + ducReferences.data.Count;
		decimal result = 0m;
		if ( totalAssignments != 0 )
		{
			var common = unionAssignments.Intersect ( ducAssignments.data ).ToList ( ).Count;
			result += ( 1m * common ) / ( totalAssignments );
		}
		if ( totalReferences != 0 )
		{
			var common = unionRefereces.Intersect ( ducReferences.data ).ToList ( ).Count;
			result += ( 1m * common ) / ( totalReferences );
		}
		if ( totalAssignments == 0 || totalReferences == 0 )
			result *= 2m;
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

public class GArrayDereference : GVar
{
	public string offset { get; set; }

	public GArrayDereference ( string str )
	{
		string pattern = @"MEM\[base: (?<name>[\w\.]+), offset: (?<offset>\w+)]";
		var match = Regex.Match ( str, pattern );
		name = match.Groups["name"].Value;
		offset = match.Groups["offset"].Value;
	}

	public static bool matches ( string str )
	{
		return Regex.IsMatch ( str, @"^MEM\[base: (?<name>[\w\.]+), offset: (?<offset>\w+)]$" );
	}

	public override int GetHashCode ( )
	{
		return name.GetHashCode ( );
	}

	public override string ToString ( )
	{
		return $"MEM[base: {name}, offset: {offset}]";
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

