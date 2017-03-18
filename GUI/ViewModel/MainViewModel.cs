using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GCC_Optimizer;
using GongSolutions.Wpf.DragDrop;
using GUI.Model;

namespace GUI.ViewModel
{
	/// <summary>
	/// This class contains properties that the main View can data bind to.
	/// </summary>
	public sealed class MainViewModel : ViewModelBase, IDropTarget
	{
		private readonly IDataService _dataService;
		private ObservableCollection<FileItem> files;
		private ObservableCollection<ResultItem> results;
		private ObservableCollection<DotOutputFormatItem> dotOutputFormats;

		#region Optimizer Settings

		public string BatchFile
		{
			get => Settings.Instance.BatchFile;
			set
			{
				Settings.Instance.BatchFile = value;
				RaisePropertyChanged ( nameof ( BatchFile ) );
			}
		}

		public ObservableCollection<string> GccFlags
		{
			get => Settings.Instance.GccFlags;
			set
			{
				Settings.Instance.GccFlags = value;
				RaisePropertyChanged ( nameof ( GccFlags ) );
			}
		}

		public ObservableCollection<string> Suffixes
		{
			get => Settings.Instance.Suffixes;
			set
			{
				Settings.Instance.Suffixes = value;
				RaisePropertyChanged ( nameof ( Suffixes ) );
			}
		}

		public ObservableCollection<DotOutputFormatItem> DotOutputFormats
		{
			get => dotOutputFormats;
			set => Set ( nameof ( DotOutputFormats ), ref dotOutputFormats, value );
		}

		public bool Rebuild
		{
			get => Settings.Instance.Rebuild;
			set
			{
				Settings.Instance.Rebuild = value;
				RaisePropertyChanged ( nameof ( Rebuild ) );
			}
		}

		#endregion Optimizer Settings

		#region FlowGraph Settings

		public decimal Threshold
		{
			get => Settings.Instance.Threshold;
			set
			{
				Settings.Instance.Threshold = value;
				RaisePropertyChanged ( nameof ( Threshold ) );
			}
		}

		public int Iterations
		{
			get => Settings.Instance.Iterations;
			set
			{
				Settings.Instance.Iterations = value;
				RaisePropertyChanged ( nameof ( Iterations ) );
			}
		}

		public bool DumpIntermediateGimple
		{
			get => Settings.Instance.DumpIntermediateGimple;
			set
			{
				Settings.Instance.DumpIntermediateGimple = value;
				RaisePropertyChanged ( nameof ( DumpIntermediateGimple ) );
			}
		}

		#endregion

		public RelayCommand AddFileCommand { get; private set; }
		public RelayCommand DeleteFileCommand { get; private set; }
		public RelayCommand CompareFileCommand { get; private set; }

		public ObservableCollection<FileItem> Files
		{
			get => files;
			set => Set ( nameof ( Files ), ref files, value );
		}

		public ObservableCollection<ResultItem> Results
		{
			get => results;
			set => Set ( nameof ( Results ), ref results, value );
		}

		/// <summary>
		/// Initializes a new instance of the MainViewModel class.
		/// </summary>
		public MainViewModel ( IDataService dataService )
		{
			_dataService = dataService;
			_dataService.GetData (
				( item, error ) =>
				{
					if ( error != null )
					{
						// Report error here
						return;
					}
				} );

			this.AddFileCommand = new RelayCommand ( this.AddFile, this.CanAddFile );
			this.DeleteFileCommand = new RelayCommand ( this.DeleteFile, this.CanDeleteFile );
			this.CompareFileCommand = new RelayCommand ( this.CompareFile, this.CanCompareFile );
			this.Files = new ObservableCollection<FileItem> ( );
			this.dotOutputFormats = new ObservableCollection<DotOutputFormatItem> ( );
			foreach ( var e in Enum.GetValues ( typeof ( DotOutputFormat ) ).Cast<DotOutputFormat> ( ) )
				dotOutputFormats.Add ( new DotOutputFormatItem ( e ) );

			Settings.Load ( );
		}

		#region Button Commands

		private bool CanCompareFile ( ) => Files.Count ( f => f.IsChecked && !f.IsFaulty ) > 0;

		private void CompareFile ( )
		{
			foreach ( var file in Files.Where ( f => f.IsChecked ) )
			{
				switch ( file.Status )
				{
					case ProgramStatus.Uninitalized:
					case ProgramStatus.Compiled:
						file.Init ( );
						break;

					case ProgramStatus.CompiledAndParsed:
						if ( Rebuild )
							file.Init ( );
						break;

					default:
						break;
				}
			}

			var result = new List<ResultItem> ( );
			var toCompare = Files.Where ( f => f.IsChecked && f.Status == ProgramStatus.CompiledAndParsed ).ToList ( );
			for ( int i = 0; i != toCompare.Count; ++i )
			{
				for ( int j = i + 1; j < toCompare.Count; j++ )
				{
					var lhs = toCompare[i].gFunc;
					var rhs = toCompare[j].gFunc;
					result.Add ( new ResultItem ( lhs, rhs ) );
				}
			}
			Results = new ObservableCollection<ResultItem> ( result.OrderBy ( x => x.Percentage * -1m ) );
		}

		private bool CanDeleteFile ( ) => Files.Any ( f => f.IsSelected );

		private void DeleteFile ( )
		{
			var toRemove = Files.Where ( x => x.IsSelected ).ToList ( );
			foreach ( var file in toRemove )
				Files.Remove ( file );
		}

		private bool CanAddFile ( ) => true;

		private void AddFile ( )
		{
			var ofd = new Microsoft.Win32.OpenFileDialog ( )
			{
				Filter = "C Files (*.c)|*.c",
				DefaultExt = ".c"
			};
			var result = ofd.ShowDialog ( );
			if ( result.HasValue && result.Value )
			{
				var fileName = ofd.FileName;
				Files.Add ( new FileItem ( fileName ) { IsChecked = true } );
			}
		}

		#endregion Button Commands

		#region Drag Drop Handlers

		void IDropTarget.DragOver ( IDropInfo dropInfo )
		{
			var dragFileList = ( (DataObject) dropInfo.Data ).GetFileDropList ( ).Cast<string> ( );
			dropInfo.Effects = dragFileList.Any ( item =>
			  {
				  var extension = Path.GetExtension ( item ).ToLower ( );
				  return extension != null && extension.Equals ( ".c" );
			  } ) ? DragDropEffects.Copy : DragDropEffects.None;
		}

		void IDropTarget.Drop ( IDropInfo dropInfo )
		{
			var dragFileList = ( (DataObject) dropInfo.Data ).GetFileDropList ( ).Cast<string> ( );
			foreach ( var fileName in dragFileList.Where ( f => Path.GetExtension ( f ).ToLower ( ) == ".c" ) )
			{
				Files.Add ( new FileItem ( fileName ) { IsChecked = true } );
			}
			CompareFileCommand.RaiseCanExecuteChanged ( );
			dropInfo.Effects = dragFileList.Any ( item =>
			  {
				  var extension = Path.GetExtension ( item ).ToLower ( );
				  return extension != null && extension.Equals ( ".c" );
			  } ) ? DragDropEffects.Copy : DragDropEffects.None;
		}

		#endregion Drag Drop Handlers

		////public override void Cleanup()
		////{
		////    // Clean up if needed

		////    base.Cleanup();
		////}
	}
}