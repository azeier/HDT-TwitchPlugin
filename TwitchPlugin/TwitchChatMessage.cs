#region

using System;

#endregion

namespace TwitchPlugin
{
	public delegate void ChangedStatusEventHandler(object sender, EventArgs e);

	public class TwitchChatMessage
	{
		public TwitchChatMessage(string user, string channel, string message)
		{
			User = user;
			Channel = channel;
			Message = message;
			Time = DateTime.Now;
		}

		public string User { get; }
		public string Channel { get; }
		public string Message { get; }
		public DateTime Time { get; }

		public override string ToString() => $"<{Channel}>[{User}] {Message}";
	}
}