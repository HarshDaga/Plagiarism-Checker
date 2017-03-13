using System.Windows;
using System.Windows.Controls;
using GUI.ViewModel;
using MahApps.Metro;
using MahApps.Metro.Controls;

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

		private void listViewFiles_SizeChanged ( object sender, SizeChangedEventArgs e )
		{
			ListView listView = sender as ListView;
			GridView gView = listView.View as GridView;

			var workingWidth = listView.ActualWidth - 35; // take into account vertical scrollbar
			var col1 = 75;
			var col3 = 120;
			var col2 = workingWidth - col1 - col3;

			gView.Columns[0].Width = col1;
			gView.Columns[1].Width = col2;
			gView.Columns[2].Width = col3;
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

		private void listViewResults_SizeChanged ( object sender, SizeChangedEventArgs e )
		{
			ListView listView = sender as ListView;
			GridView gView = listView.View as GridView;

			var workingWidth = listView.ActualWidth - 20; // take into account vertical scrollbar
			var col1 = .42;
			var col2 = .42;
			var col3 = .16;

			gView.Columns[0].Width = col1 * workingWidth;
			gView.Columns[1].Width = col2 * workingWidth;
			gView.Columns[2].Width = col3 * workingWidth;
		}
	}
}