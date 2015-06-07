#region

using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

#endregion

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
			if(Config.Instance.Channel.Contains("/"))	 //user entered channel url instead of name
				Config.Instance.Channel = new string(Config.Instance.Channel.Reverse().TakeWhile(c => c != '/').Reverse().ToArray());
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

		private void TexBoxMinGames_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			var regex = new Regex("[^0-9]+");
			e.Handled = regex.IsMatch(e.Text);
		}
	}
}