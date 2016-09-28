#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Utility.Logging;
using static TwitchPlugin.ChatCommands;

#endregion

namespace TwitchPlugin
{
	public class Core
	{
		private static IRC _irc;
		private static readonly Dictionary<string, ChatCommand> Commands;

		static Core()
		{
			Commands = new Dictionary<string, ChatCommand>();
			AddCommand("commands", CommandsCommand, "ChatCommandCommands");
			AddCommand("deck", DeckCommand, "ChatCommandDeck");
			AddCommand("alldecks", AllDecksCommand, "ChatCommandAllDecks");
			AddCommand("hdt", HdtCommand, "ChatCommandHdt");
			AddCommand("stats", () => StatsCommand(Config.Instance.ChatCommandStatsDefault), "ChatCommandStatsGeneral");
			AddCommand("stats today", () => StatsCommand("today"), "ChatCommandStatsToday", "ChatCommandStatsGeneral");
			AddCommand("stats week", () => StatsCommand("week"), "ChatCommandStatsWeek", "ChatCommandStatsGeneral");
			AddCommand("stats season", () => StatsCommand("season"), "ChatCommandStatsSeason", "ChatCommandStatsGeneral");
			AddCommand("stats total", () => StatsCommand("total"), "ChatCommandStatsTotal", "ChatCommandStatsGeneral");
			AddCommand("arena", () => ArenaCommand(Config.Instance.ChatCommandArenaDefault), "ChatCommandArenaGeneral");
			AddCommand("arena today", () => ArenaCommand("today"), "ChatCommandArenaToday", "ChatCommandArenaGeneral");
			AddCommand("arena week", () => ArenaCommand("week"), "ChatCommandArenaWeek", "ChatCommandArenaGeneral");
			AddCommand("arena season", () => ArenaCommand("season"), "ChatCommandArenaSeason", "ChatCommandArenaGeneral");
			AddCommand("arena total", () => ArenaCommand("total"), "ChatCommandArenaTotal", "ChatCommandArenaGeneral");
			AddCommand("bestdeck", () => BestDeckCommand(Config.Instance.ChatCommandBestDeckDefault), "ChatCommandBestDeckGeneral");
			AddCommand("bestdeck today", () => BestDeckCommand("today"), "ChatCommandBestDeckToday", "ChatCommandBestDeckGeneral");
			AddCommand("bestdeck week", () => BestDeckCommand("week"), "ChatCommandBestDeckWeek", "ChatCommandBestDeckGeneral");
			AddCommand("bestdeck season", () => BestDeckCommand("season"), "ChatCommandBestDeckSeason",
			           "ChatCommandBestDeckGeneral");
			AddCommand("bestdeck total", () => BestDeckCommand("total"), "ChatCommandBestDeckTotal", "ChatCommandBestDeckGeneral");
			AddCommand("mostplayed", () => MostPlayedCommand(Config.Instance.ChatCommandMostPlayedDefault),
			           "ChatCommandMostPlayedGeneral");
			AddCommand("mostplayed today", () => MostPlayedCommand("today"), "ChatCommandMostPlayedToday",
			           "ChatCommandMostPlayedGeneral");
			AddCommand("mostplayed week", () => MostPlayedCommand("week"), "ChatCommandMostPlayedWeek",
			           "ChatCommandMostPlayedGeneral");
			AddCommand("mostplayed season", () => MostPlayedCommand("season"), "ChatCommandMostPlayedSeason",
			           "ChatCommandMostPlayedGeneral");
			AddCommand("mostplayed total", () => MostPlayedCommand("total"), "ChatCommandMostPlayedTotal",
			           "ChatCommandMostPlayedGeneral");
		}

		public static string TwitchTag => "TwitchPlugin";

		public static List<string> GetCommandNames() => Commands.Select(x => x.Key).ToList();

		public static void AddCommand(string command, Action action, string propName, string generalPropName = null) => Commands.Add(command, new ChatCommand(command, action, propName, generalPropName));

		internal static void Send(string message)
		{
			if(_irc == null)
				return;
			_irc.SendMessage(Config.Instance.Channel.ToLower(), message);
			Log.Info(message, "TwitchPlugin");
		}

		public static bool Connect()
		{
			Log.Info("Logging in as " + Config.Instance.User);
			_irc = new IRC(Config.Instance.User, Config.Instance.User, Config.Instance.OAuth);
			if (!_irc.Connect("irc.twitch.tv", 6667))
				return false;
			_irc.JoinChannel(Config.Instance.Channel.ToLower());
			_irc.OnChatMsg += HandleChatMessage;
			Send("Hi! (Hearthstone Deck Tracker connected)");
			return true;
		}

		private static void HandleChatMessage(TwitchChatMessage msg)
		{
			if(!msg.Message.StartsWith("!"))
				return;
			var cmd = msg.Message.Substring(1);
			ChatCommand chatCommand;
			if(Commands.TryGetValue(cmd, out chatCommand))
				chatCommand.Execute(msg);
			else
				Log.Info($"Unknown command by {msg.User}: {msg.Message}", "TwitchPlugin");
		}

		public static void Disconnect()
		{
			if(_irc == null || !_irc.Connected)
				return;
			Send("Bye! (Hearthstone Deck Tracker disconnected)");
			_irc.LeaveChannel(Config.Instance.Channel.ToLower());
			_irc.Quit();
		}

		private static string _currentFileContent;
		public static void Update()
		{
			if(!Config.Instance.SaveStatsToFile)
				return;
			if(DeckList.Instance.ActiveDeckVersion == null)
				return;
			var games = DeckList.Instance.ActiveDeckVersion.GetRelevantGames();
			var wins = games.Count(g => g.Result == GameResult.Win);
			var losses = games.Count(g => g.Result == GameResult.Loss);
			var resultString = $"{wins} - {losses}";
			if(_currentFileContent == resultString)
				return;
			try
			{
				using(var sr = new StreamWriter(Config.Instance.StatsFileFullPath))
					sr.WriteLine(resultString);
				_currentFileContent = resultString;
			}
			catch(Exception ex)
			{
				//uncomment for v0.11.5 ?
				//Hearthstone_Deck_Tracker.API.Errors.ShowErrorMessage("TwitchPlugin", ex.ToString());
				Log.Error("Error writing to stats file: " + ex, "TwitchPlugin");
			}
		}
	}

	public class ChatCommand
	{
		private readonly Action _action;
		private readonly string _command;
		private readonly string _configItem;
		private readonly string _generalConfigItem;
		private DateTime _lastExecute;

		public ChatCommand(string command, Action action, string configItem, string generalConfigItem = null)
		{
			_command = command;
			_action = action;
			_lastExecute = DateTime.MinValue;
			_configItem = configItem;
			_generalConfigItem = generalConfigItem;
		}

		public void Execute(TwitchChatMessage msg)
		{
			Log.Info($"Command \"{_command}\" requested by {msg.User}.", "TwitchPlugin");
			if(_generalConfigItem != null && !Config.GetConfigItem<bool>(_generalConfigItem))
			{
				Log.Info($"Command \"{_command}\" is disabled (general).", "TwitchPlugin");
				return;
			}
			if(!Config.GetConfigItem<bool>(_configItem))
			{
				Log.Info($"Command \"{_command}\" is disabled.", "TwitchPlugin");
				return;
			}
			if((DateTime.Now - _lastExecute).TotalSeconds < 10)
			{
				Log.Info($"Time since last execute of {_command} is less than 10 seconds. Not executing.", "TwitchPlugin");
				return;
			}
			_lastExecute = DateTime.Now;
			_action.Invoke();
		}
	}
}