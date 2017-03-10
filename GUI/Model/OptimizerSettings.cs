using GCC_Optimizer;
using System.Collections.ObjectModel;
using System.IO;
using System.Web.Script.Serialization;

namespace GUI.Model
{
	public class OptimizerSettings
	{
		private static OptimizerSettings instance;
		public static OptimizerSettings Instance
		{
			get
			{
				if ( instance == null )
					instance = new OptimizerSettings ( );
				return instance;
			}
		}

		private static readonly string SETTINGSFILE = "OptimizerSettings.json";

		private string batchFile = Optimizer.Defaults.BatchFile;
		private ObservableCollection<string> gccFlags = new ObservableCollection<string> ( Optimizer.Defaults.GccFlags );
		private ObservableCollection<string> suffixes = new ObservableCollection<string> ( Optimizer.Defaults.Suffixes );
		private ObservableCollection<DotOutputFormat> dotOutputFormats = new ObservableCollection<DotOutputFormat> ( Optimizer.Defaults.DotOutputFormats );
		private bool rebuild = Optimizer.Defaults.Rebuild;
		
		public string BatchFile
		{
			get => this.batchFile;
			set
			{
				this.batchFile = value;
				Save ( );
			}
		}
		public ObservableCollection<string> GccFlags
		{
			get => this.gccFlags;
			set
			{
				this.gccFlags = value;
				Save ( );
			}
		}
		public ObservableCollection<string> Suffixes
		{
			get => this.suffixes;
			set
			{
				this.suffixes = value;
				Save ( );
			}
		}
		public ObservableCollection<DotOutputFormat> DotOutputFormats
		{
			get => this.dotOutputFormats;
			set
			{
				this.dotOutputFormats = value;
				Save ( );
			}
		}
		public bool Rebuild
		{
			get => this.rebuild;
			set
			{
				this.rebuild = value;
				Save ( );
			}
		}

		static OptimizerSettings ( )
		{
			if ( !File.Exists ( SETTINGSFILE ) )
				Save ( );
		}

		public static void Save ( )
		{
			if ( instance == null )
				return;
			File.WriteAllText ( SETTINGSFILE, ( new JavaScriptSerializer ( ) ).Serialize ( instance ) );
		}

		public static void Load ( )
		{
			if ( File.Exists ( SETTINGSFILE ) )
				instance = ( new JavaScriptSerializer ( ) )
					.Deserialize<OptimizerSettings> ( File.ReadAllText ( SETTINGSFILE ) );
		}
	}
}