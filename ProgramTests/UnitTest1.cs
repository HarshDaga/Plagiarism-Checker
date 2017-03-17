using FlowGraph;
using GCC_Optimizer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProgramTests
{

	[TestClass]
	public class UnitTest1
	{
		public static GFunction GetGFunction ( string fileName )
		{
			var optimizer = new Optimizer ( fileName )
			{
				Rebuild = true
			};
			optimizer.Run ( );
			var gimple = optimizer.GIMPLE;

			var gFunction = new GFunction ( gimple, fileName );

			return gFunction;
		}

		[TestMethod]
		public void ExactSame ( )
		{
			GFunction fact = GetGFunction ( "fib.c" );
			GFunction fib_same = GetGFunction ( "fib same.c" );

			Assert.IsTrue ( fact == fib_same );
		}

		[TestMethod]
		public void CommentsAndIndentation ( )
		{
			GFunction fib = GetGFunction ( "fib.c" );
			GFunction fib_comments_indent = GetGFunction ( "fib comments and indentation.c" );

			Assert.IsTrue ( fib == fib_comments_indent );
		}

		[TestMethod]
		public void Macros ( )
		{
			GFunction fib = GetGFunction ( "fib.c" );
			GFunction fib_macro = GetGFunction ( "fib macro.c" );

			Assert.IsTrue ( fib == fib_macro );
		}

		[TestMethod]
		public void SingularDecl ( )
		{
			GFunction fib = GetGFunction ( "fib.c" );
			GFunction fib_singular_decl = GetGFunction ( "fib singular decl.c" );

			Assert.IsTrue ( fib == fib_singular_decl );
		}

		[TestMethod]
		public void RedundantExpressions ( )
		{
			GFunction fib = GetGFunction ( "fib.c" );
			GFunction fib_redundant_expr = GetGFunction ( "fib redundant expressions.c" );

			Assert.IsTrue ( fib == fib_redundant_expr );
		}

		[TestMethod]
		public void RedundantVar ( )
		{
			GFunction fib = GetGFunction ( "fib.c" );
			GFunction fib_redundant_var = GetGFunction ( "fib redundant var.c" );

			Assert.IsTrue ( fib == fib_redundant_var );
		}

		[TestMethod]
		public void Datatype ( )
		{
			GFunction fib = GetGFunction ( "fib.c" );
			GFunction fib_datatype = GetGFunction ( "fib datatype.c" );

			Assert.IsTrue ( fib_datatype == fib );
		}

		[TestMethod]
		public void LoopReversal ( )
		{
			GFunction fib = GetGFunction ( "fib.c" );
			GFunction fib_loop_rev = GetGFunction ( "fib loop reversed.c" );

			Assert.IsTrue ( fib == fib_loop_rev );
		}

		[TestMethod]
		public void ConditionReversed ( )
		{
			GFunction max = GetGFunction ( "max.c" );
			GFunction max_cond_rev = GetGFunction ( "max condition rev.c" );

			Assert.IsTrue ( max == max_cond_rev );
		}

		[TestMethod]
		public void Refactored ( )
		{
			GFunction fib = GetGFunction ( "fib.c" );
			GFunction fib_refactored = GetGFunction ( "fib refactored.c" );

			Assert.IsTrue ( fib == fib_refactored );
		}

		[TestMethod]
		public void VarRenamed ( )
		{
			GFunction fib = GetGFunction ( "fib.c" );
			GFunction fib_refactored = GetGFunction ( "fib var renamed.c" );

			Assert.IsTrue ( fib == fib_refactored );
		}
	}
}