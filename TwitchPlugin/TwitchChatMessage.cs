using System;
using System.Net;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json.Linq;

namespace TwitchPlugin
{
	public delegate void ChangedStatusEventHandler(object sender, EventArgs e);

	public class TwitchChatMessage
	{
		public string User { get; private set; }
		public string Channel { get; private set; }
		public string Message { get; private set; }
		public DateTime Time { get; private set; }

		public TwitchChatMessage(string user, string channel, string message)
		{
			User = user;
			Channel = channel;
			Message = message;
			Time = DateTime.Now;
		}

		public override string ToString()
		{
			return string.Format("<{0}>[{1}] {2}", Channel, User, Message);
		}
	}
}