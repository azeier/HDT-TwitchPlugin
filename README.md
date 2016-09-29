# HDT-TwitchPlugin
This is a plugin for the [Hearthstone Deck Tracker](https://github.com/HearthSim/Hearthstone-Deck-Tracker).  
It logs into a twitch.tv account you provide and responds to chat commands in your channel (listed below).

![commands](http://i.imgur.com/8Jaslz8.png)

[List of all commands](https://github.com/azeier/HDT-TwitchPlugin/wiki/Commands)

# Requirements:
- Latest version of [HDT](https://github.com/HearthSim/Hearthstone-Deck-Tracker)
- You are logged in to your HearthStats account in HDT (`PLUGINS > HEARTHSTATS`)
- HDT is synced with HearthStats, `AUTO UPLOAD NEW GAMES` is enabled

# Instructions
1. Place `TwitchPlugin.dll` in `Hearthstone Deck Tracker/Plugins`  
- Restart/Start HDT  
- Enable plugin via `options > Tracker > Plugins`  
- In options: click `SETTINGS` with the plugin selected OR in the menu bar via `PLUGINS > TWITCH > SETTINGS`  
- Enter connection settings:
 1. username: the twitch.tv account HDT will be using in chat (can be your own or you can create a new one)  
 - oauth key: the "password" used to connect to IRC. If you don't have one yet, click the link below the textbox. (Login with the Twitch account you want HDT to use!)   
 - channel: name of your twitch channel ("twitch.tv/epix37" -> "epix37")  
- Click the `INFO` button in the bottom left to see what the commands do.   
- `PLUGINS > TWITCH > CONNECT`  
- HDT will say "Hi!" in chat to let you know it's connected.  

# Other
Commands can currently be executed once every ten seconds.

# Download:
Get the latest version here: https://github.com/azeier/HDT-TwitchPlugin/releases

# Contact
If you have questions, suggestions or just want to talk feel free to email me!
Email: alex@hearthsim.net
