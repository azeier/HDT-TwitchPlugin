using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.Stats;

namespace TwitchPlugin
{
	public class ChatCommands
	{
		const string HssUrl = "http://hss.io/d/";
		public static void AllDecksCommand()
		{
			var decks = DeckList.Instance.Decks.Where(d => d.Tags.Contains(Core.TwitchTag)).ToList();
			if(!decks.Any())
				return;
			var response = decks.Select(d => string.Format("{0}: {1}", d.Name, HssUrl + d.HearthStatsId)).Aggregate((c, n) => c + ", " + n);
			Core.Send(response);
		}

		public static void DeckCommand()
		{
			var deck = DeckList.Instance.ActiveDeck;
			if(deck.IsArenaDeck)
				Core.Send(string.Format("Current arena run ({0}): {1}, DeckList: {2}", deck.Class, deck.WinLossString, "[currently only supported for constructed decks]"));
			else
				Core.Send(string.Format("Currently using \"{0}\", Winrate: {1} ({2}), Decklist: {3}", deck.Name, deck.WinPercentString, deck.WinLossString, HssUrl + deck.HearthStatsId));
		}

		public static void StatsCommand(string arg)
		{
			var games = DeckStatsList.Instance.DeckStats.SelectMany(ds => ds.Games).ToList();
			switch(arg)
			{
				case "today":
					games = games.Where(g => g.StartTime.Date == DateTime.Today).ToList();
					break;
				case "week":
					games = games.Where(g => g.StartTime.Date > DateTime.Today.AddDays(-7)).ToList();
					break;
			}
			var numGames = games.Count;
			var numDecks = games.Select(g => g.DeckId).Distinct().Count();
			var wins = games.Count(g => g.Result == GameResult.Win);
			Core.Send(string.Format("Played {0} games with {1} decks. Total stats: {2}-{3}", numGames, numDecks, wins, numGames - wins));
		}

		public static void HdtCommand()
		{
			Core.Send(string.Format("Hearthstone Deck Tracker: https://github.com/Epix37/Hearthstone-Deck-Tracker/releases"));
		}


		private static GameStats _lastGame;
		public static void OnGameEnd()
		{
			_lastGame = Game.CurrentGameStats.CloneWithNewId();
		}

		public static void OnInMenu()
		{
			if(!Config.Instance.AutoPostGameResult)
				return;
			if(_lastGame == null)
				return;
			var deck = DeckList.Instance.ActiveDeck;
			Core.Send(string.Format("{0} VS {1} ({2}) after {3}: {4}", _lastGame.Result, _lastGame.OpponentName, _lastGame.OpponentHero, _lastGame.Duration, deck.WinLossString));
			_lastGame = null;
		}
	}
}
