#region

using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.Utility.Logging;
using MahApps.Metro.Controls.Dialogs;

#endregion

namespace TwitchPlugin
{
	public class UpdateCheck
	{
		public static async void Run(Version current)
		{
			Log.Info("Checking for updates...", "TwitchPlugin");

			const string versionXmlUrl = @"https://raw.githubusercontent.com/azeier/HDT-Data/master/Plugins/twitchplugin-version";
			try
			{
				Log.Info("Current version: " + current, "TwitchPlugin");
				string xml;
				using(var wc = new WebClient())
					xml = await wc.DownloadStringTaskAsync(versionXmlUrl);

				var newVersion = new Version(XmlManager<SerializableVersion>.LoadFromString(xml).ToString());
				Log.Info("Latest version: " + newVersion, "TwitchPlugin");

				if(newVersion > current)
				{
					await Task.Delay(5000);
					var result =
						await
						Hearthstone_Deck_Tracker.API.Core.MainWindow.ShowMessageAsync("TwitchPlugin update available!", "(Plugins can not be updated automatically)",
						                                   MessageDialogStyle.AffirmativeAndNegative,
						                                   new MetroDialogSettings {AffirmativeButtonText = "download", NegativeButtonText = "not now"});
					if(result == MessageDialogResult.Affirmative)
						Process.Start(@"https://github.com/azeier/HDT-TwitchPlugin/releases");
				}
			}
			catch(Exception e)
			{
				Log.Error("Error checking for new version.\n\n" + e, "TwitchPlugin");
			}
		}
	}
}