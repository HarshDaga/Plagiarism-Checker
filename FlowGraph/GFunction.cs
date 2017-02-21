﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

#pragma warning disable CS1591

namespace FlowGraph
{
	/// <summary>
	/// Represents a function in the GIMPLE file.
	/// </summary>
	public class GFunction
	{
		/// <summary>
		/// All lines in the GIMPLE file.
		/// </summary>
		public List<string> gimple { get; private set; }
		/// <summary>
		/// Name of the function.
		/// </summary>
		public string name { get; private set; }
		/// <summary>
		/// funcdef_no of the function.
		/// </summary>
		public int funcdef_no { get; private set; }
		/// <summary>
		/// decl_uid of the function.
		/// </summary>
		public int decl_uid { get; private set; }
		/// <summary>
		/// symbol_order of the function.
		/// </summary>
		public int symbol_order { get; private set; }
		/// <summary>
		/// Functions formal arguments.
		/// </summary>
		public string args { get; private set; }
		/// <summary>
		/// All the <see cref="GBaseBlock"/>s in the function.
		/// </summary>
		public List<GBaseBlock> blocks { get; private set; } = new List<GBaseBlock> ( );
		/// <summary>
		/// <see cref="Dictionary{TKey, TValue}"/> of <see cref="int"/> mapped to <see cref="GBaseBlock"/> by the number.
		/// </summary>
		public Dictionary<int, GBaseBlock> blockMap { get; private set; } = new Dictionary<int, GBaseBlock> ( );
		/// <summary>
		/// <see cref="HashSet{T}"/> of all the <see cref="GVar"/> declared in the function.
		/// </summary>
		public HashSet<GVar> gVarsDecl { get; set; } = new HashSet<GVar> ( );
		/// <summary>
		/// <see cref="HashSet{T}"/> of all the variable names used in the function as <see cref="string"/>.
		/// </summary>
		public HashSet<string> gVarsUsed { get; set; } = new HashSet<string> ( );

		/// <summary>
		/// Build all the <see cref="GBaseBlock"/>s and other members using the GIMPLE input file.
		/// </summary>
		/// <param name="gimple"><see cref="List{T}"/> of <see cref="string"/> from the GIMPLE source.</param>
		public GFunction ( List<string> gimple )
		{
			this.gimple = gimple;
			string pattern = @";; Function (?<name>\S*) \((\S*, funcdef_no=(?<funcdef_no>[0-9]*), decl_uid=(?<decl_uid>[0-9]*), cgraph_uid=[0-9]*, symbol_order=(?<symbol_order>[0-9]*))\).*";
			bool insideFunc = false, insideBody = false;
			List<string> body = new List<string> ( );
			foreach ( var line in gimple )
			{
				if ( Regex.IsMatch ( line, pattern ) )
				{
					insideFunc = true;
					var match = Regex.Match ( line, pattern );
					name = match.Groups["name"].Value;
					funcdef_no = Convert.ToInt32 ( match.Groups["funcdef_no"].Value );
					decl_uid = Convert.ToInt32 ( match.Groups["decl_uid"].Value );
					symbol_order = Convert.ToInt32 ( match.Groups["symbol_order"].Value );
				}
				if ( !insideFunc )
					continue;
				if ( !insideBody && line != "{" )
					continue;
				insideBody = true;
				if ( new string[] { "{", "}" }.Contains ( line ) )
					continue;
				body.Add ( line );
			}
			gVarsDecl = getVarsDecl ( body );
			blocks = GBaseBlock.getBaseBlocks ( body );

			getVarsUsed ( );

			createEdges ( );
		}

		static HashSet<GVar> getVarsDecl ( List<string> gimple )
		{
			HashSet<GVar> vars = new HashSet<GVar> ( );
			foreach ( var line in gimple )
			{
				if ( line == "" || GBBStmt.matches ( line ) )
					break;
				string declPattern = @"\s*(?<type>.*) (?<name>\S*);";
				var match = Regex.Match ( line, declPattern );
				var gVar = new GVar
				{
					name = match.Groups["name"].Value,
					type = match.Groups["type"].Value
				};
				vars.Add ( gVar );
			}

			return vars;
		}

		void getVarsUsed ( )
		{
			foreach ( var block in blocks )
				gVarsUsed.UnionWith ( block.vars );
		}

		void createEdges ( )
		{
			foreach ( var block in blocks )
				blockMap[block.number] = block;
			foreach ( var block in blocks )
			{
				var numbers = block.getOutEdges ( );
				foreach ( var number in numbers )
				{
					if ( !blockMap.ContainsKey ( number ) )
						continue;
					block.outEdges.Add ( blockMap[number] );
					blockMap[number].inEdges.Add ( block );
				}
			}
		}

		/// <summary>
		/// Compare the <see cref="GFunction.blocks"/> of both <see cref="GFunction"/> using <code>SequenceEqual()</code>.
		/// </summary>
		/// <param name="lhs">LHS</param>
		/// <param name="rhs">RHS</param>
		/// <returns></returns>
		public static bool operator == ( GFunction lhs, GFunction rhs )
		{
			return lhs.blocks.SequenceEqual ( rhs.blocks );
		}

		/// <summary>
		/// Negation of <see cref="operator =="/>.
		/// </summary>
		/// <param name="lhs">LHS</param>
		/// <param name="rhs">RHS</param>
		/// <returns></returns>
		public static bool operator != ( GFunction lhs, GFunction rhs )
		{
			return !( lhs == rhs );
		}

        public override bool Equals ( object obj )
        {
            if ( obj is GFunction )
                return this == ( obj as GFunction );
            return base.Equals ( obj );
        }

        public override int GetHashCode ( )
        {
            return base.GetHashCode ( );
        }
        //public Dictionary<string, string> getVarMap ( GFunction gFunc )
        //{

        //}
    }
}
