using GalaSoft.MvvmLight;
using GCC_Optimizer;

namespace GUI.Model
{
	public class DotOutputFormatItem : ViewModelBase
	{
		private DotOutputFormat format;

		public DotOutputFormat Format
		{
			get => format;
			set => Set ( nameof ( Format ), ref format, value );
		}

		public string Name => format.ToString ( );

		public bool IsChecked
		{
			get => OptimizerSettings.Instance.DotOutputFormats.Contains ( format );
			set
			{
				if ( value && !OptimizerSettings.Instance.DotOutputFormats.Contains ( format ) )
					OptimizerSettings.Instance.DotOutputFormats.Add ( format );
				else if ( !value )
					OptimizerSettings.Instance.DotOutputFormats.Remove ( format );
				OptimizerSettings.Save ( );
				RaisePropertyChanged ( nameof ( IsChecked ) );
			}
		}

		public DotOutputFormatItem ( DotOutputFormat format )
		{
			this.format = format;
		}
	}
}