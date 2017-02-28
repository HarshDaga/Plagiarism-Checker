using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
		/// Threshold to be used when comparing variables (0 - 1)
		/// Default: 0.95m
		/// </summary>
		public decimal threshold { get; set; } = 0.95m;

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
		/// <see cref="Dictionary{TKey, TValue}"/> of <see cref="string"/> mapped to <see cref="GVar"/> by the variable name with its GIMPLE suffix.
		/// </summary>
		public Dictionary<string, GVar> usedToDeclMap { get; set; } = new Dictionary<string, GVar> ( );

		/// <summary>
		/// <see cref="Dictionary{TKey, TValue}"/> of <see cref="string"/> mapped to <see cref="GVar"/> by the variable name with its GIMPLE suffix.
		/// </summary>
		public List<GimpleStmt> gStatements
		{
			get
			{
				return blocks.SelectMany ( b => b.gStatements ).ToList ( );
			}
		}

		/// <summary>
		/// Build all the <see cref="GBaseBlock"/>s and other members using the GIMPLE input file.
		/// </summary>
		/// <param name="gimple"><see cref="List{T}"/> of <see cref="string"/> from the GIMPLE source.</param>
		public GFunction ( List<string> gimple )
		{
			initialize ( gimple );
		}

		private void initialize ( List<string> gimple )
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

			blocks = GBaseBlock.getBaseBlocks ( body );

			getVarsDecl ( body );

			getVarsUsed ( );

			buildUsedToDeclMap ( );

			removeCasts ( );

			getVarsUsed ( );

			buildUsedToDeclMap ( );

			buildDUC ( );

			createEdges ( );
		}

		private void getVarsDecl ( List<string> gimple )
		{
			gVarsDecl.Clear ( );
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
				gVarsDecl.Add ( gVar );
			}

		}

		private void getVarsUsed ( )
		{
			gVarsUsed.Clear ( );
			foreach ( var block in blocks )
				gVarsUsed.UnionWith ( block.vars );
		}

		private void buildUsedToDeclMap ( )
		{
			usedToDeclMap.Clear ( );
			foreach ( var used in gVarsUsed )
			{
				foreach ( var v in gVarsDecl )
				{
					if ( Regex.IsMatch ( used, $"^{v.name}(_[0123456789]+)?$" ) )
					{
						usedToDeclMap[used] = v;
					}
				}
				if ( !usedToDeclMap.ContainsKey ( used ) )
				{
					var v = new GVar ( )
					{
						name = used,
						type = "void *"
					};
					gVarsDecl.Add ( v );
					usedToDeclMap[used] = v;
				}
			}
		}

		private void buildDUC ( )
		{
			foreach ( var v in gVarsDecl )
			{
				v.ducAssignments.data.Clear ( );
				v.ducReferences.data.Clear ( );
			}
			foreach ( var block in blocks )
			{
				gVarsUsed.UnionWith ( block.vars );
				foreach ( var stmt in block.gStatements )
				{
					if ( stmt is GAssignStmt || stmt is GPhiStmt || stmt is GOptimizedStmt )
					{
						usedToDeclMap[stmt.vars[0]].ducAssignments.add ( block.number, stmt.linenum );
						for ( int i = 1; i < stmt.vars.Count; ++i )
							usedToDeclMap[stmt.vars[i]].ducReferences.add ( block.number, stmt.linenum );
					}
					else
					{
						foreach ( var v in stmt.vars )
							usedToDeclMap[v].ducReferences.add ( block.number, stmt.linenum );
					}
				}
			}
		}

		public void Rename ( string oldName, string newName )
		{
			blocks.ForEach ( b => b.Rename ( oldName, newName ) );

			if ( gVarsDecl.Count ( v => v.name == oldName ) != 0 )
				gVarsDecl.Where ( v => v.name == oldName ).FirstOrDefault ( ).name =
					usedToDeclMap.ContainsKey ( newName ) ? usedToDeclMap[newName].name : newName;
			gVarsDecl = new HashSet<GVar> ( gVarsDecl );

			if ( gVarsUsed.Remove ( oldName ) )
			{
				gVarsUsed.Add ( newName );
				if ( usedToDeclMap.ContainsKey ( oldName ) )
				{
					usedToDeclMap[newName] = usedToDeclMap[oldName];
					usedToDeclMap.Remove ( oldName );
				}
			}
		}

		private void createEdges ( )
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

		private void removeCasts ( )
		{
			blocks
				.SelectMany ( b => b.gStatements )
				.Where ( s => s is GCastStmt )
				.Select ( s => s as GCastStmt )
				.ToList ( )
				.ForEach ( cast => Rename ( cast.assignee, cast.v ) );
			blocks.ForEach ( b =>
			{
				b.gStatements.RemoveAll ( s => s is GCastStmt );
				b.RenumberLines ( );
			}
			);
		}

		private void preprocess ( GFunction gFunc )
		{
			var varMap = getVarMap ( gFunc );

			foreach ( var pair in varMap )
			{
				var duc = pair.Value
					.SelectMany ( x => x.ducAssignments.data )
					.Union ( pair.Value.SelectMany ( x => x.ducReferences.data ) )
					.ToList ( );

				foreach ( var entry in duc )
				{
					var lhsStmt = blockMap[entry.block].gStatements[entry.line - 1];
					var rhsStmt = gFunc.blockMap[entry.block].gStatements[entry.line - 1];

					if ( rhsStmt.vars.Count == lhsStmt.vars.Count )
					{
						for ( int i = 0; i < lhsStmt.vars.Count; ++i )
						{
							string lvar = lhsStmt.vars[i];
							string rvar = rhsStmt.vars[i];
							if ( gFunc.usedToDeclMap.ContainsKey ( rvar ) &&
								varMap[usedToDeclMap[lvar]].Contains ( gFunc.usedToDeclMap[rvar] ) )
								rhsStmt.Rename ( rvar, lvar );
						}
					}
				}
			}
		}

		public decimal Compare ( GFunction gFunc )
		{
			preprocess ( gFunc );
			decimal count = 0;
			var lhsStmts = gStatements;
			var rhsStmts = gFunc.gStatements;
			for ( int i = 0; i < lhsStmts.Count && i < rhsStmts.Count; ++i )
				if ( lhsStmts[i] == rhsStmts[i] )
					++count;
			decimal result = 200m * count / ( lhsStmts.Count + rhsStmts.Count );

			gFunc.initialize ( gFunc.gimple );

			return result;
		}

		/// <summary>
		/// Compare the <see cref="GFunction.blocks"/> of both <see cref="GFunction"/> using <code>SequenceEqual()</code>.
		/// </summary>
		/// <param name="lhs">LHS</param>
		/// <param name="rhs">RHS</param>
		/// <returns></returns>
		public static bool operator == ( GFunction lhs, GFunction rhs )
		{
			lhs.preprocess ( rhs );
			bool result = lhs.blocks.SequenceEqual ( rhs.blocks );

			rhs.initialize ( rhs.gimple );

			return result;
		}

		/// <summary>
		/// Negation of <see cref="operator !="/>.
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

		public Dictionary<GVar, List<GVar>> getVarMap ( GFunction gFunc )
		{
			Dictionary<GVar, List<GVar>> varMap = new Dictionary<GVar, List<GVar>> ( );

			Dictionary<GVar, Dictionary<GVar, decimal>> correlation = new Dictionary<GVar, Dictionary<GVar, decimal>> ( );
			foreach ( var v1 in gVarsDecl )
			{
				foreach ( var v2 in gFunc.gVarsDecl )
				{
					if ( !correlation.ContainsKey ( v1 ) )
						correlation[v1] = new Dictionary<GVar, decimal> ( );
					correlation[v1][v2] = v1.map ( v2 );
				}
				var probable = correlation[v1]
					.Where ( x => x.Value != 0 )
					.OrderBy ( x => x.Value )
					.Select ( x => x.Key )
					.ToList ( );
				var minimal = new List<GVar> ( );
				foreach ( var v in probable )
				{
					if ( !v.isSubsetOf ( v1 ) )
						continue;
					minimal.Add ( v );
					if ( v1.map ( minimal ) > threshold )
					{
						varMap[v1] = minimal;
						break;
					}
				}
			}

			foreach ( var v1 in gFunc.gVarsDecl )
			{
				foreach ( var v2 in gVarsDecl )
				{
					var probable = correlation
						.Where ( x => x.Value[v1] != 0m )
						.OrderBy ( x => x.Value[v1] )
						.Select ( x => x.Key )
						.ToList ( );
					var minimal = new List<GVar> ( );
					foreach ( var v in probable )
					{
						minimal.Add ( v );
						if ( v1.map ( minimal ) > threshold )
						{
							minimal.ForEach ( m =>
							{
								if ( !varMap.ContainsKey ( m ) )
									varMap[m] = new List<GVar> { v1 };
							}
							);
							break;
						}
					}
				}
			}

			//foreach ( var v1 in gVarsDecl )
			//	foreach ( var v2 in gFunc.gVarsDecl )
			//		if ( v1.map ( v2 ) == 1m )
			//			varMap[v1] = v2;

			return varMap;
		}
	}
}