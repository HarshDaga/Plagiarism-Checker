#pragma warning disable CS1591

namespace GCC_Optimizer
{
	/// <summary>
	/// Result of <see cref="Optimizer.Optimizer(string)"/>
	/// </summary>
	public enum OptimizeResult
	{
		None,
		Success,
		FileNotFound,
		BadExtension,
		CompileError
	}

}