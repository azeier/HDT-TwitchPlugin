# HDT-TwitchPlugin
This is a plugin for the [Hearthstone Deck Tracker](https://github.com/Epix37/Hearthstone-Deck-Tracker).  
It logs into a twitch.tv account you provide and responds to chat commands in your channel (listed below).   
![commands](http://i.imgur.com/8Jaslz8.png)

#Requirements:
- Latest version of HDT (v0.10.1)
- You are logged in to your HearthStats account in HDT (`PLUGINS > HEARTHSTATS`)
- HDT is synced with HearthStats, `AUTO UPLOAD NEW GAMES` is enabled

#Instructions
1) Place `TwitchPlugin.dll` in `Hearthstone Deck Tracker/Plugins`  
2) Restart/Start HDT  
3) Enable plugin via `options > Tracker > Plugins`  
4) In options: click `SETTINGS` with the plugin selected OR in the menu bar via `PLUGINS > TWITCH > SETTINGS`  
5) Enter connection settings  
5.1) username: the twitch.tv account HDT will be using in chat (can be your own or you can create a new one)  
5.2) oauth key: the "password" used to connect to IRC. If you don't have one yet, click the link below the textbox. (Login with the Twitch account you want HDT to use!)   
5.3) channel: name of your twitch channel ("twitch.tv/epix37" -> "epix37")  
6) Click the `INFO` button in the bottom left to see what the commands do.   
7) `PLUGINS > TWITCH > CONNECT`  
8) HDT will say "Hi!" in chat to let you know it's connected.  

#Other
- Commands can currently be executed once every ten seconds.

#Download:
- Get the latest version here: https://github.com/Epix37/HDT-TwitchPlugin/releases

#Contact
If you have questions, suggestions or just want to talk feel free to email me!
Email: epikz37@gmail.com
