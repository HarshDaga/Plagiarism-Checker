using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

#pragma warning disable CS1591

namespace GCC_Optimizer
{
	/// <summary>
	/// Result of <see cref="Optimizer.Optimizer(string, string, string, List{DotOutputFormat}, List{string}, bool)"/>
	/// </summary>
	public enum OptimizeResult
	{
		None,
		Success,
		FileNotFound,
		BadExtension,
		CompileError
	}

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

	/// <summary>
	/// Handles all GCC and dot related activities.
	/// </summary>
	public class Optimizer
	{
		public string fileName { get; private set; }
		public List<string> dotOutputs { get; private set; }
		private string batchFile { get; set; }
		public string gccFlags { get; set; }
		public List<string> suffixes { get; set; }
		public List<DotOutputFormat> dotOutputFormats { get; set; }
		public List<string> originalSource { get; private set; } = null;
		public List<string> GIMPLE { get; private set; } = null;
		public List<string> originalGIMPLE { get; private set; } = null;
		public OptimizeResult lastError { get; private set; } = OptimizeResult.None;
		public string stdout { get; private set; } = "";
		public string stderr { get; private set; } = "";
		public bool suppressOutput { get; set; } = true;

		public Optimizer (
			string fileName,
			string batchFile = "____temp.bat",
			string gccFlags = "-O3 -fdump-tree-optimized-graph",
			List<DotOutputFormat> dotOutputFormats = default ( List<DotOutputFormat> ),
			List<string> suffixes = default ( List<string> ),
			bool suppressOutput = true
			)
		{
			if ( fileName.Contains ( "\\" ) )
			{
				this.fileName = fileName.Substring ( fileName.LastIndexOf ( '\\' ) + 1 );
				File.Copy ( fileName, this.fileName, true );
			}
			else
				this.fileName = fileName;
			this.batchFile = batchFile;
			this.gccFlags = gccFlags;
			if ( dotOutputFormats == default ( List<DotOutputFormat> ) )
				dotOutputFormats = new List<DotOutputFormat> { DotOutputFormat.png, DotOutputFormat.plain };
			this.dotOutputFormats = dotOutputFormats;
			this.suppressOutput = suppressOutput;
			if ( suffixes == default ( List<string> ) )
				suffixes = new List<string> { ".190t.optimized", ".191t.optimized" };
			this.suffixes = suffixes;
			this.dotOutputs = new List<string> ( );

			if ( !VerifyFile ( ) )
				return;

			originalSource = File.ReadAllLines ( this.fileName ).ToList ( );

			string prog = Flatten ( );

			CompileAndOptimize ( );

			GIMPLE = CleanGIMPLE ( originalGIMPLE );

			// Replace with original
			{
				var fileStream = File.Open ( this.fileName, FileMode.Truncate );
				var byteArray = Encoding.ASCII.GetBytes ( prog );
				fileStream.Write ( byteArray, 0, byteArray.Length );
				fileStream.Close ( );
			}
		}

		private bool VerifyFile ( )
		{
			if ( !fileName.EndsWith ( ".c" ) )
			{
				dotOutputs = null;
				lastError = OptimizeResult.BadExtension;
				return false;
			}
			if ( !File.Exists ( fileName ) )
			{
				lastError = OptimizeResult.FileNotFound;
				return false;
			}

			return true;
		}

		/// <summary>
		/// Add <code>__attribute__((flatten))</code> to <code>main()</code> in the source file.
		/// </summary>
		/// <returns></returns>
		private string Flatten ( )
		{
			var prog = File.ReadAllText ( fileName );
			string pattern = @"(.* )?main\s*\(.*\).*";
			var match = Regex.Match ( prog, pattern );
			if ( !match.Value.Contains ( "flatten" ) )
			{
				var modified = Regex.Replace ( prog, @"(main\(.*\))", " __attribute__((flatten))$1" );
				var fileStream = File.Open ( fileName, FileMode.Truncate );
				var byteArray = Encoding.ASCII.GetBytes ( modified );
				fileStream.Write ( byteArray, 0, byteArray.Length );
				fileStream.Close ( );
			}
			return prog;
		}

		/// <summary>
		/// Compile, Optimize and Convert the output given the source filename.
		/// </summary>
		/// <returns></returns>
		private bool CompileAndOptimize ( )
		{
			string cmd = $"gcc {gccFlags} \"" + fileName + "\"\n";
			var results = ExecCmd ( cmd );
			stdout += results.Item1;
			stderr += results.Item2;
			if ( results.Item2.Length > 0 )
			{
				lastError = OptimizeResult.CompileError;                             // Compilation Error
				return false;
			}
			File.Delete ( "a.exe" );

			string oldOptimizedGIMPLE = null, oldOptimizedDot = null;
			foreach ( var suffix in suffixes )
				if ( File.Exists ( fileName + suffix ) )
					oldOptimizedGIMPLE = fileName + suffix;
			oldOptimizedDot = oldOptimizedGIMPLE + ".dot";
			var prefix = fileName.Substring ( 0, fileName.Length - 2 );
			var dir = new DirectoryInfo ( prefix );
			if ( dir.Exists )
				foreach ( var f in dir.GetFiles ( ) )
					f.Delete ( );
			else
				dir.Create ( );
			while ( !dir.Exists )
				dir.Refresh ( );
			var file = new FileInfo ( oldOptimizedGIMPLE );
			while ( !file.Exists )
				file.Refresh ( );
			var optimizedGIMPLE = prefix + ".GIMPLE";
			var optimizedDot = prefix + ".opt.dot";
			//Thread.Sleep ( 1000 );
			File.Move ( oldOptimizedGIMPLE, $"{dir.Name}\\{optimizedGIMPLE}" );
			File.Move ( oldOptimizedDot, $"{dir.Name}\\{optimizedDot}" );
			File.Copy ( fileName, $"{dir.Name}\\{fileName}" );
			originalGIMPLE = File.ReadAllLines ( $"{dir.Name}\\{optimizedGIMPLE}" ).ToList ( );
			foreach ( var format in dotOutputFormats )
			{
				cmd = $"dot -T{format} -O \"{prefix}\\{optimizedDot}\"";
				results = ExecCmd ( cmd );
				stdout += results.Item1;
				stderr += results.Item2;
				dotOutputs.Add ( $"{optimizedDot}.{format}" );
			}

			return true;
		}

		/// <summary>
		/// Remove all lines except <code>main()</code> and its body.
		/// </summary>
		/// <param name="original">Original GIMPLE file.</param>
		/// <returns></returns>
		private static List<string> CleanGIMPLE ( List<string> original )
		{
			List<string> clean = new List<string> ( );
			int start = 0;
			while ( !original[start].StartsWith ( ";; Function main" ) )
				++start;
			int end = start + 1;
			while ( original[end] != "}" )
				++end;
			clean = original.GetRange ( start, end - start + 1 );
			return clean;
		}

		/// <summary>
		/// <para>
		/// Makes a temporary batch file and executes it as a separate process.
		/// </para>
		/// </summary>
		/// <param name="cmd"></param>
		/// <returns><see cref="Tuple{STDOUT, STDERR}"/>.</returns>
		public Tuple<string, string> ExecCmd ( string cmd )
		{
			var fileStream = File.Create ( batchFile );
			var byteArray = Encoding.ASCII.GetBytes ( cmd );
			fileStream.Write ( byteArray, 0, byteArray.Length );
			fileStream.Close ( );

			Process p = new Process ( );
			// Redirect the output stream of the child process.
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.RedirectStandardError = true;
			p.StartInfo.FileName = batchFile;
			p.Start ( );
			// Do not wait for the child process to exit before
			// reading to the end of its redirected stream.
			// p.WaitForExit();
			// Read the output stream first and then wait.
			string output = p.StandardOutput.ReadToEnd ( );
			string error = p.StandardError.ReadToEnd ( );
			if ( !suppressOutput )
			{
				Console.WriteLine ( output );
				Console.WriteLine ( error );
			}
			p.WaitForExit ( );

			File.Delete ( batchFile );

			return new Tuple<string, string> ( output, error );
		}

	}
}