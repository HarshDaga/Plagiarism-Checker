using System.Windows;
using System.Windows.Controls;
using GUI.ViewModel;
using MahApps.Metro;
using MahApps.Metro.Controls;
using LiveCharts;

namespace GUI
{
	using AppSettings = Properties.Settings;

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : MetroWindow
	{
		/// <summary>
		/// Initializes a new instance of the MainWindow class.
		/// </summary>
		public MainWindow ( )
		{
			InitializeComponent ( );
			LoadSettings ( );
			Closing += ( s, e ) => ViewModelLocator.Cleanup ( );
		}

		private void LoadSettings ( )
		{
			ThemeManager.ChangeAppTheme ( Application.Current, AppSettings.Default.Theme );
			var accent = ThemeManager.GetAccent ( AppSettings.Default.Accent );
			var theme = ThemeManager.DetectAppStyle ( Application.Current );
			ThemeManager.ChangeAppStyle ( Application.Current, accent, theme.Item1 );
			cbAccent.SelectedValue = accent;
			tbtTheme.IsChecked = AppSettings.Default.Theme == "BaseLight";
		}

		private void cbAccent_SelectionChanged ( object sender, SelectionChangedEventArgs e )
		{
			if ( cbAccent.SelectedItem is Accent selectedAccent )
			{
				var theme = ThemeManager.DetectAppStyle ( Application.Current );
				ThemeManager.ChangeAppStyle ( Application.Current, selectedAccent, theme.Item1 );
				AppSettings.Default.Accent = selectedAccent.Name;
				AppSettings.Default.Save ( );
			}
		}

		private void tbtTheme_Click ( object sender, RoutedEventArgs e )
		{
			string themeName = ( tbtTheme.IsChecked.HasValue && tbtTheme.IsChecked.Value ) ?
				"BaseLight" : "BaseDark";
			AppSettings.Default.Theme = themeName;
			AppSettings.Default.Save ( );
			ThemeManager.ChangeAppTheme ( Application.Current, themeName );
		}

		private void btSettings_Click ( object sender, RoutedEventArgs e )
		{
			flySettings.IsOpen = !flySettings.IsOpen;
		}
	}
}