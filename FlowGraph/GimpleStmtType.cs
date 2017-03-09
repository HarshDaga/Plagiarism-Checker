using System.ComponentModel;

#pragma warning disable CS1591

namespace FlowGraph
{
	/// <summary>
	/// GIMPLE Statement Types
	/// </summary>
	public enum GimpleStmtType
	{
		/// <summary>
		/// Base Block
		/// </summary>
		[Description ( "Base Block" )]
		GBB,

		/// <summary>
		/// Conditional Statements
		/// </summary>
		[Description ( "If" )]
		GCOND,

		/// <summary>
		/// Else statement
		/// </summary>
		[Description ( "Else" )]
		GELSE,

		/// <summary>
		/// Goto statement
		/// </summary>
		[Description ( "Go to" )]
		GGOTO,

		/// <summary>
		/// Label
		/// </summary>
		[Description ( "Label" )]
		GLABEL,

		/// <summary>
		/// Switch statement
		/// </summary>
		[Description ( "Switch" )]
		GSWITCH,

		/// <summary>
		/// Function call
		/// </summary>
		[Description ( "Call" )]
		GCALL,

		/// <summary>
		/// PHI statement
		/// </summary>
		[Description ( "PHI" )]
		GPHI,

		/// <summary>
		/// Assignment statement
		/// </summary>
		[Description ( "Assignment" )]
		GASSIGN,

		/// <summary>
		/// Optimized statement
		/// </summary>
		[Description ( "Optimized" )]
		GOPTIMIZED,

		/// <summary>
		/// Casting statement
		/// </summary>
		[Description ( "Casting" )]
		GCAST,

		/// <summary>
		/// Return statement
		/// </summary>
		[Description ( "Return" )]
		GRETURN
	}
}