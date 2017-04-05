using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FlowGraph;
using GalaSoft.MvvmLight;
using GCC_Optimizer;

namespace GUI.Model
{
	public class FileItem : ViewModelBase
	{
		private string _fileName;
		private bool _isChecked;
		private bool _isSelected;

		private Optimizer optimizer;
		private GFunction gFunc;

		public string FileName
		{
			get => Path.GetFileName ( _fileName );
			private set => Set ( nameof ( FileName ), ref _fileName, value );
		}

		public bool IsChecked
		{
			get => _isChecked;
			set => Set ( nameof ( IsChecked ), ref _isChecked, value );
		}

		public bool IsSelected
		{
			get => _isSelected;
			set => Set ( nameof ( IsSelected ), ref _isSelected, value );
		}

		public bool IsFaulty =>
			Status == ProgramStatus.FileNotFound ||
			Status == ProgramStatus.BadFileName ||
			Status == ProgramStatus.CompileError;

		public ProgramStatus Status
		{
			get
			{
				if ( Optimizer == null )
					return ProgramStatus.Uninitalized;
				if ( Optimizer.LastError == OptimizeResult.BadExtension )
					return ProgramStatus.BadFileName;
				if ( Optimizer.LastError == OptimizeResult.FileNotFound )
					return ProgramStatus.FileNotFound;
				if ( Optimizer.LastError == OptimizeResult.CompileError )
					return ProgramStatus.CompileError;
				if ( Optimizer.LastError == OptimizeResult.None )
					return ( GFunc == null ) ? ProgramStatus.Compiled : ProgramStatus.CompiledAndParsed;
				return ProgramStatus.Uninitalized;
			}
		}

		public GFunction GFunc { get => this.gFunc; set => this.gFunc = value; }
		public Optimizer Optimizer { get => this.optimizer; set => this.optimizer = value; }

		public FileItem ( string fileName )
		{
			this.FileName = fileName;
		}

		public void Reset ( )
		{
			this.optimizer = null;
			this.GFunc = null;
			RaisePropertyChanged ( nameof ( Status ) );
		}

		public async Task<ProgramStatus> InitOptimizer ( )
		{
			try
			{
				Optimizer = new Optimizer ( _fileName )
				{
					BatchFile = Settings.Instance.BatchFile,
					GccFlags = Settings.Instance.GccFlags.ToList ( ),
					DotOutputFormats = Settings.Instance.DotOutputFormats.ToList ( ),
					Suffixes = Settings.Instance.Suffixes.ToList ( ),
					Rebuild = Settings.Instance.Rebuild
				};
				if ( !IsFaulty )
					await Task.Run ( ( ) => Optimizer.Run ( ) );
			}
			catch ( FileNotFoundException ) { }

			return Status;
		}

		public async Task<ProgramStatus> InitFlowgraph ( )
		{
			if ( !IsFaulty )
			{
				await Task.Run ( ( ) =>
								{
									GFunc = new GFunction ( Optimizer.GIMPLE, FileName )
									{
										Threshold = Settings.Instance.Threshold,
										Iterations = Settings.Instance.Iterations,
										DumpIntermediateGimple = Settings.Instance.DumpIntermediateGimple
									};
								}
								);
			}

			return Status;
		}

		public async Task<ProgramStatus> Init ( bool Rebuild )
		{
			if ( Optimizer == null || Rebuild )
				await InitOptimizer ( );
			await InitFlowgraph ( );
			RaisePropertyChanged ( nameof ( Status ) );

			return Status;
		}
	}
}