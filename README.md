# AntRaceBot

This is a small project specifically for Smallant1, intended to be used to make it easier to verify winners of giveaways for guessing the finishing time of races correctly.

# Installation

This program requires no installation. Simply build from source or download a release from the releases tab and run AntRaceBot.exe.

# Use

The use of this program requires access to a Twitch account (or possibly a registered app) to read and message in chat. You'll need to input the account's credentials (username and OAuth token) in the settings menu to run the bot. If you don't have an OAuth token for your account, you can go [here](https://twitchapps.com/tmi/). More settings are available from the settings menu in-program, or through the settings.xml file in the same folder as the executable.

### Chat Commands
| Command | Description |
| --- | --- |
| !start | Starts counting chat messages as guesses |
| !stop | Stops counting chat messages as guesses |
| !check \<time\> | Determines the winners of the race based on the given time |

### Hotkeys

This program includes support for global hotkeys to start and stop counting chat messages. These hotkeys may be set by clicking Settings -> Open Hotkeys in the program. Note that after changing a hotkey, the program must be restarted for those changes to take effect.

### Formatting Guesses

Guesses (and the argument for the !check command) should be formatted by seperating groups of numbers (e.g. minutes from seconds) with colons, commas, or periods. For example, for a time of 30 seconds and 210 milliseconds, the time would be formattted "30.21", "30:21", or "30,21". Each of these three values are interchangeable and identical from the perspective of the program. In order to be as inclusive of guesses as possible, the guess may be immediately preceeded or proceeded by any character, and will still be counted (e.g. "abc30.21def" counts as a guess for "30.21").