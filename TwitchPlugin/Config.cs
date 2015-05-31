#region

using System.IO;
using Hearthstone_Deck_Tracker;

#endregion

namespace TwitchPlugin
{
	public class Config
	{
		private static Config _instance;

		public Config()
		{
			AutoPostGameResult = true;
			ChatCommandCommands = true;
			ChatCommandDeck = true;
			ChatCommandAllDecks = true;
			ChatCommandHdt = true;
			ChatCommandStatsGeneral = true;
			ChatCommandStatsToday = true;
			ChatCommandStatsWeek = true;
			ChatCommandStatsSeason = true;
			ChatCommandStatsTotal = true;
			ChatCommandArenaGeneral = true;
			ChatCommandArenaToday = true;
			ChatCommandArenaWeek = true;
			ChatCommandArenaSeason = true;
			ChatCommandArenaTotal = true;
			ChatCommandBestDeckGeneral = true;
			ChatCommandBestDeckToday = true;
			ChatCommandBestDeckWeek = true;
			ChatCommandBestDeckSeason = true;
			ChatCommandBestDeckTotal = true;
			ChatCommandMostPlayedGeneral = true;
			ChatCommandMostPlayedToday = true;
			ChatCommandMostPlayedWeek = true;
			ChatCommandMostPlayedSeason = true;
			ChatCommandMostPlayedTotal = true;
			BestDeckGamesThreshold = 3;
		}

		public static Config Instance
		{
			get { return _instance ?? Load(); }
		}

		public string User { get; set; }
		public string OAuth { get; set; }
		public string Channel { get; set; }
		public bool AutoPostGameResult { get; set; }
		public bool ChatCommandCommands { get; set; }
		public bool ChatCommandDeck { get; set; }
		public bool ChatCommandAllDecks { get; set; }
		public bool ChatCommandHdt { get; set; }
		public bool ChatCommandStatsGeneral { get; set; }
		public bool ChatCommandStatsToday { get; set; }
		public bool ChatCommandStatsWeek { get; set; }
		public bool ChatCommandStatsSeason { get; set; }
		public bool ChatCommandStatsTotal { get; set; }
		public bool ChatCommandArenaGeneral { get; set; }
		public bool ChatCommandArenaToday { get; set; }
		public bool ChatCommandArenaWeek { get; set; }
		public bool ChatCommandArenaSeason { get; set; }
		public bool ChatCommandArenaTotal { get; set; }
		public bool ChatCommandBestDeckGeneral { get; set; }
		public bool ChatCommandBestDeckToday { get; set; }
		public bool ChatCommandBestDeckWeek { get; set; }
		public bool ChatCommandBestDeckSeason { get; set; }
		public bool ChatCommandBestDeckTotal { get; set; }
		public bool ChatCommandMostPlayedGeneral { get; set; }
		public bool ChatCommandMostPlayedToday { get; set; }
		public bool ChatCommandMostPlayedWeek { get; set; }
		public bool ChatCommandMostPlayedSeason { get; set; }
		public bool ChatCommandMostPlayedTotal { get; set; }
		public int BestDeckGamesThreshold { get; set; }

		private static string FilePath
		{
			get { return Path.Combine(Hearthstone_Deck_Tracker.Config.Instance.ConfigDir, "TwitchPlugin.xml"); }
		}

		public static T GetConfigItem<T>(string name)
		{
			object prop = Instance.GetType().GetProperty(name).GetValue(Instance, null);
			if(prop == null)
				return default(T);
			return (T)prop;
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