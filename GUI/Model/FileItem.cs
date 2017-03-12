using System.IO;
using System.Linq;
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

		public Optimizer optimizer;
		public GFunction gFunc;

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
				if ( optimizer == null )
					return ProgramStatus.Uninitalized;
				if ( optimizer.LastError == OptimizeResult.BadExtension )
					return ProgramStatus.BadFileName;
				if ( optimizer.LastError == OptimizeResult.FileNotFound )
					return ProgramStatus.FileNotFound;
				if ( optimizer.LastError == OptimizeResult.CompileError )
					return ProgramStatus.CompileError;
				if ( optimizer.LastError == OptimizeResult.None )
					return ( gFunc == null ) ? ProgramStatus.Compiled : ProgramStatus.CompiledAndParsed;
				return ProgramStatus.Uninitalized;
			}
		}

		public FileItem ( string fileName )
		{
			this.FileName = fileName;
		}

		public void InitOptimizer ( )
		{
			try
			{
				optimizer = new Optimizer ( _fileName )
				{
					BatchFile = OptimizerSettings.Instance.BatchFile,
					GccFlags = OptimizerSettings.Instance.GccFlags.ToList ( ),
					DotOutputFormats = OptimizerSettings.Instance.DotOutputFormats.ToList ( ),
					Suffixes = OptimizerSettings.Instance.Suffixes.ToList ( ),
					Rebuild = OptimizerSettings.Instance.Rebuild
				};
				if ( !IsFaulty )
					optimizer.Run ( );
			}
			catch ( FileNotFoundException ) { }
		}

		public void Init ( )
		{
			if ( optimizer == null )
				InitOptimizer ( );
			if ( !IsFaulty )
				gFunc = new GFunction ( optimizer.GIMPLE, FileName );
			RaisePropertyChanged ( nameof ( Status ) );
		}
	}
}