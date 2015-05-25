#region

using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Hearthstone_Deck_Tracker;

#endregion

namespace TwitchPlugin
{
	public class Config
	{
		private static Config _instance;

		public static Config Instance
		{
			get { return _instance ?? Load(); }
		}

		public Config()
		{
			AutoPostGameResult = true;
			ChatCommandDeck = true;
			ChatCommandAllDecks = true;
			ChatCommandHdt = true;
			ChatCommandStatsToday = true;
			ChatCommandStatsWeek = true;
		}

		public string User { get; set; }
		public string OAuth { get; set; }
		public string Channel { get; set; }
		public bool AutoPostGameResult { get; set; }
		public bool ChatCommandDeck { get; set; }
		public bool ChatCommandAllDecks { get; set; }
		public bool ChatCommandHdt { get; set; }
		public bool ChatCommandStatsToday { get; set; }
		public bool ChatCommandStatsWeek { get; set; }

		public static T GetConfigItem<T>(string name)
		{
			object prop = Instance.GetType().GetProperty(name).GetValue(Instance, null);
			if(prop == null) { return default(T); }
			return (T)prop;
		}

		private static string FilePath
		{
			get { return Path.Combine(Hearthstone_Deck_Tracker.Config.Instance.ConfigDir, "TwitchPlugin.xml"); }
		}

		public static void Save()
		{
			XmlManager<Config>.Save(FilePath, Instance);
		}

		private static Config Load()
		{
			return _instance = File.Exists(FilePath) ? XmlManager<Config>.Load(FilePath) : new Config();
		}
	}
}