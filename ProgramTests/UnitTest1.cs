using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlowGraph;
using GCC_Optimizer;

namespace ProgramTests
{

	[TestClass]
	public class UnitTest1
	{
		public static GFunction getGFunction ( string fileName )
		{
			var optimizer = new Optimizer ( fileName );
			var gimple = optimizer.GIMPLE;

			var gFunction = new GFunction ( gimple );

			return gFunction;
		}

		[TestMethod]
		public void ExactSame ( )
		{
			GFunction fact = getGFunction ( "fib.c" );
			GFunction fib_same = getGFunction ( "fib same.c" );

			Assert.IsTrue ( fact == fib_same );
		}

		[TestMethod]
		public void CommentsAndIndentation ( )
		{
			GFunction fib = getGFunction ( "fib.c" );
			GFunction fib_comments_indent = getGFunction ( "fib comments and indentation.c" );

			Assert.IsTrue ( fib == fib_comments_indent );
		}

		[TestMethod]
		public void Macros ( )
		{
			GFunction fib = getGFunction ( "fib.c" );
			GFunction fib_macro = getGFunction ( "fib macro.c" );

			Assert.IsTrue ( fib == fib_macro );
		}

		[TestMethod]
		public void SingularDecl ( )
		{
			GFunction fib = getGFunction ( "fib.c" );
			GFunction fib_singular_decl = getGFunction ( "fib singular decl.c" );

			Assert.IsTrue ( fib == fib_singular_decl );
		}

		[TestMethod]
		public void RedundantExpressions ( )
		{
			GFunction fib = getGFunction ( "fib.c" );
			GFunction fib_redundant_expr = getGFunction ( "fib redundant expressions.c" );

			Assert.IsTrue ( fib == fib_redundant_expr );
		}

		[TestMethod]
		public void RedundantVar ( )
		{
			GFunction fib = getGFunction ( "fib.c" );
			GFunction fib_redundant_var = getGFunction ( "fib redundant var.c" );

			Assert.IsTrue ( fib == fib_redundant_var );
		}

		[TestMethod]
		public void Datatype ( )
		{
			GFunction fib = getGFunction ( "fib.c" );
			GFunction fib_redundant_var = getGFunction ( "fib datatype.c" );

			Assert.IsTrue ( fib == fib_redundant_var );
		}

		[TestMethod]
		public void LoopReversal ( )
		{
			GFunction fib = getGFunction ( "fib.c" );
			GFunction fib_loop_rev = getGFunction ( "fib loop reversed.c" );

			Assert.IsTrue ( fib == fib_loop_rev );
		}

		[TestMethod]
		public void ConditionReversed ( )
		{
			GFunction max = getGFunction ( "max.c" );
			GFunction max_cond_rev = getGFunction ( "max condition rev.c" );

			Assert.IsTrue ( max == max_cond_rev );
		}

		[TestMethod]
		public void Refactored ( )
		{
			GFunction fib = getGFunction ( "fib.c" );
			GFunction fib_refactored = getGFunction ( "fib refactored.c" );

			Assert.IsTrue ( fib == fib_refactored );
		}

		[TestMethod]
		public void VarRenamed ( )
		{
			GFunction fib = getGFunction ( "fib.c" );
			GFunction fib_refactored = getGFunction ( "fib var renamed.c" );

			Assert.IsTrue ( fib == fib_refactored );
		}
	}
}
