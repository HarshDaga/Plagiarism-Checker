using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

#pragma warning disable CS1591

namespace GCC_Optimizer
{

	/// <summary>
	/// Handles all GCC and dot related activities.
	/// </summary>
	[DebuggerDisplay ( "{FileName}" )]
	public class Optimizer
	{
		public static class Defaults
		{
			public static readonly string BatchFile = "____temp.bat";

			public static readonly List<string> GccFlags =
				new List<string>
				{
					"-Ofast",
					"-fdump-tree-optimized-graph",
					"-fexpensive-optimizations",
					"-frerun-loop-opt",
					"-fgcse",
					"-fmerge-all-constants",
					"-fno-tree-vectorize",
					"-fno-tree-copy-prop",
					"-fno-guess-branch-probability",
					"-fno-ivopts"
				};

			public static readonly List<string> Suffixes =
				new List<string>
				{
					".190t.optimized",
					".191t.optimized"
				};

			public static readonly List<DotOutputFormat> DotOutputFormats =
				new List<DotOutputFormat>
				{
					DotOutputFormat.png,
					DotOutputFormat.jpg,
					DotOutputFormat.pdf,
					DotOutputFormat.plain
				};

			public static readonly bool SuppressOutput = true;
			public static readonly bool Rebuild = true;
			public static readonly OptimizeResult LastError = OptimizeResult.None;
		}

		public string FileName { get; private set; }
		public List<string> DotOutputs { get; private set; }
		public string BatchFile { get; set; }
		public List<string> GccFlags { get; set; }
		public List<string> Suffixes { get; set; }
		public List<DotOutputFormat> DotOutputFormats { get; set; }
		public List<string> OriginalSource { get; private set; } = null;
		public List<string> GIMPLE { get; private set; } = null;
		public List<string> OriginalGIMPLE { get; private set; } = null;
		public OptimizeResult LastError { get; private set; } = Defaults.LastError;
		public string Stdout { get; private set; } = "";
		public string Stderr { get; private set; } = "";
		public bool SuppressOutput { get; set; } = Defaults.SuppressOutput;
		public bool Rebuild { get; set; } = Defaults.Rebuild;

		public Optimizer ( string fileName )
		{
			if ( !File.Exists ( fileName ) )
			{
				LastError = OptimizeResult.FileNotFound;
				return;
				//throw new FileNotFoundException ( $"{fileName} couldn't be found." );
			}

			this.FileName = Path.GetFileName ( fileName );
			if ( Path.IsPathRooted ( fileName ) &&
				!fileName.StartsWith ( Directory.GetCurrentDirectory ( ) ) )
				File.Copy ( fileName, this.FileName, true );

			if ( BatchFile == null )
				BatchFile = Defaults.BatchFile;

			if ( GccFlags == null )
				GccFlags = Defaults.GccFlags.ToList ( );

			if ( DotOutputFormats == null )
				DotOutputFormats = Defaults.DotOutputFormats.ToList ( );

			if ( Suffixes == null )
				Suffixes = Defaults.Suffixes.ToList ( );

			DotOutputs = new List<string> ( );
		}

		public bool Run ( )
		{
			if ( !VerifyFile ( ) )
				return false;

			OriginalSource = File.ReadAllLines ( this.FileName ).ToList ( );

			string prog = Flatten ( );

			if ( !CompileAndOptimize ( ) )
				return false;

			GIMPLE = CleanGIMPLE ( OriginalGIMPLE );

			// Replace with original
			{
				var fileStream = File.Open ( this.FileName, FileMode.Truncate );
				var byteArray = Encoding.ASCII.GetBytes ( prog );
				fileStream.Write ( byteArray, 0, byteArray.Length );
				fileStream.Close ( );
			}

			return true;
		}

		private bool VerifyFile ( )
		{
			if ( !FileName.EndsWith ( ".c" ) )
			{
				DotOutputs = null;
				LastError = OptimizeResult.BadExtension;
				return false;
			}
			if ( !File.Exists ( FileName ) )
			{
				LastError = OptimizeResult.FileNotFound;
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
			var prog = File.ReadAllText ( FileName );
			string pattern = @"(.* )?main\s*\(.*\).*";
			var match = Regex.Match ( prog, pattern );
			if ( !match.Value.Contains ( "flatten" ) )
			{
				var modified = Regex.Replace ( prog, @"(main\s*\(.*\))", " __attribute__((flatten))$1" );
				var fileStream = File.Open ( FileName, FileMode.Truncate );
				var byteArray = Encoding.ASCII.GetBytes ( modified );
				fileStream.Write ( byteArray, 0, byteArray.Length );
				fileStream.Close ( );
			}

			return prog;
		}

		private bool GimpleExists ( )
		{
			var prefix = Path.GetFileNameWithoutExtension ( FileName );
			var dir = new DirectoryInfo ( prefix );
			if ( dir.Exists )
			{
				var files = dir.GetFiles ( );
				var cfile = files.FirstOrDefault ( f => f.Name == FileName );
				if ( cfile != null )
				{
					var gimple = files.FirstOrDefault ( f => f.Name == $"{prefix}.GIMPLE" );
					if ( gimple != null )
					{
						OriginalGIMPLE = File.ReadAllLines ( gimple.FullName ).ToList ( );
						return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Compile, Optimize and Convert the output given the source filename.
		/// </summary>
		/// <returns>true on successful compilation.</returns>
		private bool CompileAndOptimize ( )
		{
			if ( !Rebuild && GimpleExists ( ) )
				return true;

			var prefix = Path.GetFileNameWithoutExtension ( FileName );
			var dir = new DirectoryInfo ( prefix );

			string cmd;

			// Compile the file
			{
				cmd = $"gcc {string.Join ( " ", GccFlags )} \"" + FileName + "\"\n";
				var results = ExecCmd ( cmd );
				Stdout += results.stdout;
				Stderr += results.stderr;
				if ( results.stderr.Length > 0 )
				{
					LastError = OptimizeResult.CompileError;                             // Compilation Error
					return false;
				}
				File.Delete ( "a.exe" );
			}

			string oldOptimizedGIMPLE = null, oldOptimizedDot = null;
			foreach ( var suffix in Suffixes )
				if ( File.Exists ( FileName + suffix ) )
					oldOptimizedGIMPLE = FileName + suffix;
			oldOptimizedDot = oldOptimizedGIMPLE + ".dot";

			if ( dir.Exists )
			{
				foreach ( var f in dir.GetFiles ( ) )
				{
					try
					{
						if ( f.Name.StartsWith ( Path.GetFileNameWithoutExtension ( FileName ) + "." ) )
							f.Delete ( );
					}
					catch ( System.Exception )
					{
					}
				}
			}
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

			// Rearrange files
			{
				File.Move ( oldOptimizedGIMPLE, $"{dir.Name}\\{optimizedGIMPLE}" );
				File.Move ( oldOptimizedDot, $"{dir.Name}\\{optimizedDot}" );
				File.Copy ( FileName, $"{dir.Name}\\{FileName}", true );
			}

			OriginalGIMPLE = File.ReadAllLines ( $"{dir.Name}\\{optimizedGIMPLE}" ).ToList ( );

			foreach ( var format in DotOutputFormats )
			{
				cmd = $"dot -T{format} -O \"{prefix}\\{optimizedDot}\"";
				var results = ExecCmd ( cmd );
				Stdout += results.stdout;
				Stderr += results.stderr;
				DotOutputs.Add ( $"{optimizedDot}.{format}" );
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
		/// <returns>A ValueTuple of (<see cref="string"/> stdout, <see cref="string"/> stderr).</returns>
		public (string stdout, string stderr) ExecCmd ( string cmd )
		{
			if ( BatchFile.EndsWith ( ".bat" ) )
				BatchFile = BatchFile + ".bat";
			var fileStream = File.Create ( BatchFile );
			var byteArray = Encoding.ASCII.GetBytes ( cmd );
			fileStream.Write ( byteArray, 0, byteArray.Length );
			fileStream.Close ( );

			Process p = new Process ( );
			// Redirect the output stream of the child process.
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.RedirectStandardError = true;
			p.StartInfo.FileName = BatchFile;
			p.StartInfo.CreateNoWindow = true;
			p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			p.Start ( );
			// Do not wait for the child process to exit before
			// reading to the end of its redirected stream.
			// p.WaitForExit();
			// Read the output stream first and then wait.
			string output = p.StandardOutput.ReadToEnd ( );
			string error = p.StandardError.ReadToEnd ( );
			if ( !SuppressOutput )
			{
				Console.WriteLine ( output );
				Console.WriteLine ( error );
			}
			p.WaitForExit ( );

			File.Delete ( BatchFile );

			return (output, error);
		}

	}

}