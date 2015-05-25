using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Hearthstone_Deck_Tracker;

namespace TwitchPlugin
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class SettingsWindow
	{
		public SettingsWindow()
		{
			InitializeComponent();
			TextBoxUser.Text = Config.Instance.User;
			TexBoxChannel.Text = Config.Instance.Channel;
			TextBoxOAuth.Password = Config.Instance.OAuth;
		}

		private void Button_ClickSave(object sender, RoutedEventArgs e)
		{
			Config.Instance.Channel = TexBoxChannel.Text;
			Config.Instance.User = TextBoxUser.Text;
			Config.Instance.OAuth = TextBoxOAuth.Password.Replace("oauth:", "");
			Config.Save();
			Close();
		}
		
		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			Process.Start(e.Uri.AbsoluteUri);
		}

		private void Button_ClickInfo(object sender, RoutedEventArgs e)
		{
			FlyoutCommandsInfo.IsOpen = !FlyoutCommandsInfo.IsOpen;
		}
	}
}
