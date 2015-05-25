#region

using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.Stats;

#endregion

namespace TwitchPlugin
{
	public class Core
	{
		private static IRC _irc;

		private static readonly Dictionary<string, ChatCommand> Commands;

		public static string TwitchTag { get { return "TwitchPlugin"; } }

		public static List<string> GetCommandNames()
		{
			return Commands.Select(x => x.Key).ToList();
		}

		static Core()
		{
			Commands = new Dictionary<string, ChatCommand>();
			AddCommand("deck", ChatCommands.DeckCommand, "ChatCommandDeck");
			AddCommand("alldecks", ChatCommands.AllDecksCommand, "ChatCommandAllDecks");
			AddCommand("hdt", ChatCommands.HdtCommand, "ChatCommandHdt");
			AddCommand("stats today", () => ChatCommands.StatsCommand("today"), "ChatCommandStatsToday");
			AddCommand("stats week", () => ChatCommands.StatsCommand("week"), "ChatCommandStatsWeek");
		}


		public static void AddCommand(string command, Action action, string propName)
		{
			Commands.Add(command, new ChatCommand(command, action, propName));
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
			{
				chatCommand.Execute(msg);
			}
			else
			{
				Logger.WriteLine(string.Format("Unknown command by {0}: {1}", msg.User, msg.Message), "TwitchPlugin");
			}
			
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
		private readonly string _command;
		private readonly Action _action;
		private readonly string _configItem;
		private DateTime _lastExecute;
		public ChatCommand(string command, Action action, string configItem)
		{
			_command = command;
			_action = action;
			_lastExecute = DateTime.MinValue;
			_configItem = configItem;
		}

		public void Execute(TwitchChatMessage msg)
		{
			Logger.WriteLine(string.Format("Command \"{0}\" requested by {1}.", _command, msg.User), "TwitchPlugin");
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