using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GUI
{
	/// <summary>
	/// Interaction logic for StringListBox.xaml
	/// </summary>
	public partial class StringListBox : UserControl
	{
		public StringListBox ( )
		{
			InitializeComponent ( );
			Box.DataContext = this;
			TitleLabel.DataContext = this;
		}

		#region Events
		private void AddClicked ( object sender, RoutedEventArgs e )
		{
			ItemsSource.Add ( AddStringBox.Text );
			AddStringBox.Clear ( );
			object o = this.DataContext;
			if ( !( o is INotifyCollectionChanged ) )
			{
				this.DataContext = null;
				this.DataContext = o;
			}
		}

		private void MenuDeleteClicked ( object sender, RoutedEventArgs e )
		{
			ItemsSource.Remove ( Box.SelectedItem.ToString ( ) );
		}

		private void OnDeletePressed ( object sender, KeyEventArgs e )
		{
			if ( e.Key == Key.Delete )
			{
				MenuDeleteClicked ( sender, e );
			}
		}

		private void OnEnterPressed ( object sender, KeyEventArgs e )
		{
			if ( e.Key == Key.Enter )
			{
				AddClicked ( this, e );
			}
		}
		#endregion

		#region ItemsSource DependencyProperty
		public ICollection<string> ItemsSource
		{
			get { return (ICollection<string>) GetValue ( ItemsSourceProperty ); }
			set { SetValue ( ItemsSourceProperty, value ); }
		}

		public static readonly DependencyProperty ItemsSourceProperty =
			DependencyProperty.Register ( "ItemsSource", typeof ( ICollection<string> ), typeof ( StringListBox ), null );
		#endregion

		#region Title DependencyProperty
		public string Title
		{
			get { return (string) GetValue ( TitleProperty ); }
			set { SetValue ( TitleProperty, value ); }
		}

		public static readonly DependencyProperty TitleProperty =
			DependencyProperty.Register ( "TitleProperty", typeof ( string ), typeof ( StringListBox ), null );
		#endregion
	}
}
