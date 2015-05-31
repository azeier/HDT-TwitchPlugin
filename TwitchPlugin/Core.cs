#region

using System;
using System.Collections.Generic;
using System.Linq;
using Hearthstone_Deck_Tracker;

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
			AddCommand("commands", ChatCommands.CommandsCommand, "ChatCommandCommands");
			AddCommand("deck", ChatCommands.DeckCommand, "ChatCommandDeck");
			AddCommand("alldecks", ChatCommands.AllDecksCommand, "ChatCommandAllDecks");
			AddCommand("hdt", ChatCommands.HdtCommand, "ChatCommandHdt");
			AddCommand("stats", () => ChatCommands.StatsCommand(Config.Instance.ChatCommandStatsDefault), "ChatCommandStatsGeneral");
			AddCommand("stats today", () => ChatCommands.StatsCommand("today"), "ChatCommandStatsToday", "ChatCommandStatsGeneral");
			AddCommand("stats week", () => ChatCommands.StatsCommand("week"), "ChatCommandStatsWeek", "ChatCommandStatsGeneral");
			AddCommand("stats season", () => ChatCommands.StatsCommand("season"), "ChatCommandStatsSeason", "ChatCommandStatsGeneral");
			AddCommand("stats total", () => ChatCommands.StatsCommand("total"), "ChatCommandStatsTotal", "ChatCommandStatsGeneral");
			AddCommand("arena", () => ChatCommands.ArenaCommand(Config.Instance.ChatCommandArenaDefault), "ChatCommandArenaGeneral");
			AddCommand("arena today", () => ChatCommands.ArenaCommand("today"), "ChatCommandArenaToday", "ChatCommandArenaGeneral");
			AddCommand("arena week", () => ChatCommands.ArenaCommand("week"), "ChatCommandArenaWeek", "ChatCommandArenaGeneral");
			AddCommand("arena season", () => ChatCommands.ArenaCommand("season"), "ChatCommandArenaSeason", "ChatCommandArenaGeneral");
			AddCommand("arena total", () => ChatCommands.ArenaCommand("total"), "ChatCommandArenaTotal", "ChatCommandArenaGeneral");
			AddCommand("bestdeck", () => ChatCommands.BestDeckCommand(Config.Instance.ChatCommandBestDeckDefault), "ChatCommandBestDeckGeneral");
			AddCommand("bestdeck today", () => ChatCommands.BestDeckCommand("today"), "ChatCommandBestDeckToday", "ChatCommandBestDeckGeneral");
			AddCommand("bestdeck week", () => ChatCommands.BestDeckCommand("week"), "ChatCommandBestDeckWeek", "ChatCommandBestDeckGeneral");
			AddCommand("bestdeck season", () => ChatCommands.BestDeckCommand("season"), "ChatCommandBestDeckSeason",
			           "ChatCommandBestDeckGeneral");
			AddCommand("bestdeck total", () => ChatCommands.BestDeckCommand("total"), "ChatCommandBestDeckTotal", "ChatCommandBestDeckGeneral");
			AddCommand("mostplayed", () => ChatCommands.MostPlayedCommand(Config.Instance.ChatCommandMostPlayedDefault),
			           "ChatCommandMostPlayedGeneral");
			AddCommand("mostplayed today", () => ChatCommands.MostPlayedCommand("today"), "ChatCommandMostPlayedToday",
			           "ChatCommandMostPlayedGeneral");
			AddCommand("mostplayed week", () => ChatCommands.MostPlayedCommand("week"), "ChatCommandMostPlayedWeek",
			           "ChatCommandMostPlayedGeneral");
			AddCommand("mostplayed season", () => ChatCommands.MostPlayedCommand("season"), "ChatCommandMostPlayedSeason",
			           "ChatCommandMostPlayedGeneral");
			AddCommand("mostplayed total", () => ChatCommands.MostPlayedCommand("total"), "ChatCommandMostPlayedTotal",
			           "ChatCommandMostPlayedGeneral");
		}

		public static string TwitchTag
		{
			get { return "TwitchPlugin"; }
		}

		public static List<string> GetCommandNames()
		{
			return Commands.Select(x => x.Key).ToList();
		}

		public static void AddCommand(string command, Action action, string propName, string generalPropName = null)
		{
			Commands.Add(command, new ChatCommand(command, action, propName, generalPropName));
		}

		internal static void Send(string message)
		{
			_irc.SendMessage(Config.Instance.Channel, message);
			Logger.WriteLine(message, "TwitchPlugin");
		}

		public static bool Connect()
		{
			_irc = new IRC(Config.Instance.User, Config.Instance.User, Config.Instance.OAuth);
			var success = _irc.Connect("irc.twitch.tv", 6667);
			if(success)
			{
				_irc.JoinChannel(Config.Instance.Channel);
				_irc.OnChatMsg += HandleChatMessage;
				Send("Hi! (Hearthstone Deck Tracker connected)");
			}
			return success;
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
				Logger.WriteLine(string.Format("Unknown command by {0}: {1}", msg.User, msg.Message), "TwitchPlugin");
		}

		public static void Disconnect()
		{
			if(_irc != null && _irc.Connected)
			{
				Send("Bye! (Hearthstone Deck Tracker disconnected)");
				_irc.LeaveChannel(Config.Instance.Channel);
				_irc.Quit();
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
			Logger.WriteLine(string.Format("Command \"{0}\" requested by {1}.", _command, msg.User), "TwitchPlugin");
			if(_generalConfigItem != null && !Config.GetConfigItem<bool>(_generalConfigItem))
			{
				Logger.WriteLine(string.Format("Command \"{0}\" is disabled (general).", _command), "TwitchPlugin");
				return;
			}
			if(!Config.GetConfigItem<bool>(_configItem))
			{
				Logger.WriteLine(string.Format("Command \"{0}\" is disabled.", _command), "TwitchPlugin");
				return;
			}
			if((DateTime.Now - _lastExecute).TotalSeconds < 10)
			{
				Logger.WriteLine(string.Format("Time since last execute of {0} is less than 10 seconds. Not executing.", _command), "TwitchPlugin");
				return;
			}
			_lastExecute = DateTime.Now;
			_action.Invoke();
		}
	}
}