#region

using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Hearthstone_Deck_Tracker;

#endregion

namespace TwitchPlugin
{
	internal class IRC
	{
		public delegate void ChatMsg(TwitchChatMessage msg);

		public delegate void Receive(string msg);

		public delegate void UserColor(string user, string color);

		private readonly string _name;
		private readonly string _nick;
		private readonly string _oauth;
		private TcpClient _connection;
		private NetworkStream _nwStream;
		private StreamReader _reader;
		private StreamWriter _writer;

		public IRC(string name, string nick, string oauth)
		{
			_name = name;
			_nick = nick;
			_oauth = oauth;
		}

		public bool Connected
		{
			get { return _connection.Connected; }
		}

		public event ChatMsg OnChatMsg;

		public bool Connect(string server, int port)
		{
			try
			{
				_connection = new TcpClient(server, port);
			}
			catch(Exception ex)
			{
				Logger.WriteLine("Error connecting: " + ex, "TwitchPlugin");
				return false;
			}

			try
			{
				_nwStream = _connection.GetStream();
				_reader = new StreamReader(_nwStream);
				_writer = new StreamWriter(_nwStream);

				var thread = new Thread(Listen);
				thread.IsBackground = true;
				thread.Start();

				_writer.AutoFlush = true;
				string oauth = _oauth;
				if(!oauth.StartsWith("oauth:"))
					oauth = "oauth:" + _oauth;
				SendData("PASS", oauth, false);
				SendData("NICK", _nick);
				SendData("USER", _name + " 8 * :" + _nick);
				//SendData("JTVCLIENT");
			}
			catch(Exception ex)
			{
				Logger.WriteLine("Communication Error: " + ex, "TwitchPlugin");
				return false;
			}
			return true;
		}

		public void JoinChannel(string channel)
		{
			SendData("JOIN", "#" + channel);
		}

		public void LeaveChannel(string channel)
		{
			SendData("PART", "#" + channel);
		}

		public void Quit()
		{
			SendData("QUIT");
		}

		public void SendMessage(string channel, string message)
		{
			SendData("PRIVMSG #" + channel, ":" + message);
		}

		private void Listen()
		{
			string data;
			while(_connection.Connected && (data = _reader.ReadLine()) != null)
			{
				try
				{
					var dataParts = data.Split(' ');
					if(dataParts[0] == "PING")
					{
						SendData("PONG", dataParts[1]);
						continue;
					}
					if(dataParts[1] == "PRIVMSG")
					{
						var msg = data.Split(':')[2];
						var user = dataParts[0].Remove(0, 1).Split('!')[0];
						var channel = dataParts[2].Remove(0, 1);
						if(OnChatMsg != null)
							OnChatMsg(new TwitchChatMessage(user, channel, msg));
					}
				}
				catch(Exception e)
				{
					Logger.WriteLine(e.StackTrace);
				}
			}
		}

		private void SendData(string cmd, string param = "", bool log = true)
		{
			string data = param == "" ? cmd : cmd + " " + param;
			_writer.WriteLine(data);
			if(Config.Instance.IrcLogging && log)
				Logger.WriteLine(data, "TwitchPlugin-IRC");
		}
	}
}