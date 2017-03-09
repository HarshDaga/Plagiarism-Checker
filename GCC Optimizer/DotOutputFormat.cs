using System.ComponentModel;

#pragma warning disable CS1591

namespace GCC_Optimizer
{
	/// <summary>
	/// Supported 'dot' output formats.
	/// </summary>
	public enum DotOutputFormat
	{
		[Description ( "bmp" )]
		bmp,

		[Description ( "gif" )]
		gif,

		[Description ( "ico" )]
		ico,

		[Description ( "jpg" )]
		jpg,

		[Description ( "pdf" )]
		pdf,

		[Description ( "plain" )]
		plain,

		[Description ( "png" )]
		png,

		[Description ( "ps" )]
		ps,

		[Description ( "svg" )]
		svg,

		[Description ( "tga" )]
		tga,

		[Description ( "vml" )]
		vml
	}

}