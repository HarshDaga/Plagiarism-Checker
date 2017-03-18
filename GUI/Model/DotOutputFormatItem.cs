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
			get => Settings.Instance.DotOutputFormats.Contains ( format );
			set
			{
				if ( value && !Settings.Instance.DotOutputFormats.Contains ( format ) )
					Settings.Instance.DotOutputFormats.Add ( format );
				else if ( !value )
					Settings.Instance.DotOutputFormats.Remove ( format );
				Settings.Save ( );
				RaisePropertyChanged ( nameof ( IsChecked ) );
			}
		}

		public DotOutputFormatItem ( DotOutputFormat format )
		{
			this.format = format;
		}
	}
}