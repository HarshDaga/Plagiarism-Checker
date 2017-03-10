using System.ComponentModel;

#pragma warning disable CS1591

namespace GCC_Optimizer
{
	/// <summary>
	/// Supported 'dot' output formats.
	/// </summary>
	public enum DotOutputFormat
	{
		[Description ( "png" )]
		png,

		[Description ( "jpg" )]
		jpg,

		[Description ( "pdf" )]
		pdf,

		[Description ( "plain" )]
		plain,

		[Description ( "svg" )]
		svg,

		[Description ( "bmp" )]
		bmp,

		[Description ( "gif" )]
		gif,

		[Description ( "ico" )]
		ico,

		[Description ( "ps" )]
		ps,

		[Description ( "tga" )]
		tga,

		[Description ( "vml" )]
		vml
	}

}