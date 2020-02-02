using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace AntRaceBot
{
    class Bot
    {
        public TwitchClient Client { get; }

        public delegate void MessageDelegate(ChatMessage a);
        private delegate void Func();
        private delegate void UpdateWinnersFunc(List<ChatMessage> winners, string time);

        private List<ChatMessage> messages = new List<ChatMessage>();
        public bool IsActive { get; private set; } = false;

        public Bot()
        {
            try {
                ConnectionCredentials cred = new ConnectionCredentials(Program.settings.botName, Program.settings.botPass);
                var clientOptions = new ClientOptions {
                    MessagesAllowedInPeriod = 750,
                    ThrottlingPeriod = TimeSpan.FromSeconds(30)
                };
                WebSocketClient wsClient = new WebSocketClient(clientOptions);
                Client = new TwitchClient(wsClient);
                Client.Initialize(cred, Program.settings.channelName);

                Client.OnMessageReceived += (sender, args) => Debug.WriteLine("#" + args.ChatMessage.Channel + " <" + args.ChatMessage.Username + ">: " + args.ChatMessage.Message);
                AddOnMessage(MessageHandler);
                AddOnMessage(CommandHandler);
            } catch(Exception) {
                MessageBox.Show("Error creating the Twitch connection. Please check your username and auth.", "Some title", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (Program.settings.botName != "usernameId") Client.Connect();
        }

        public void Disconnect()
        {
            Client.Disconnect();
        }

        public void Reconnect()
        {
            if (Client.IsConnected) Client.Connect();
            else Client.Reconnect();
        }

        //handles adding messages to the list to check when we're done
        private void MessageHandler(ChatMessage msg)
        {
            if (IsActive) messages.Add(msg);
        }

        //handles chat commands
        private void CommandHandler(ChatMessage msg)
        {
            if (msg.UserType == TwitchLib.Client.Enums.UserType.Moderator || msg.UserType == TwitchLib.Client.Enums.UserType.Broadcaster) {
                //!start
                if (msg.Message.ToLower().StartsWith("!start")) {
                    StartGuessing(msg.Channel);
                }
                //!stop
                if (msg.Message.ToLower().StartsWith("!stop")) {
                    StopGuessing(msg.Channel);
                }
                //!check <seconds>.<fraction>
                if (msg.Message.ToLower().StartsWith("!check")) {
                    //extract the time and clean it to use the . seperator
                    string time = msg.Message.ToLower().Substring("!check ".Length);
                    string timeClean = time.Replace(":", ".").Replace(",", ".").Trim();

                    //find all the correct answers
                    List<ChatMessage> correct = new List<ChatMessage>();
                    List<ChatMessage> doubled = new List<ChatMessage>();
                    //loop through the messages we collected
                    foreach (var m in messages) {
                        //we clean the contents to make sure it uses . as the seperator as well
                        if (m.Message.Replace(":", ".").Replace(",", ".").Contains(timeClean)) {
                            //make sure they guessed only once
                            int numGuesses = 0;
                            foreach (var m2 in messages) {
                                Debug.WriteLine(m2.Message);
                                //and once again we check for several seperators, but this time by using regex
                                if (m2.Username == m.Username && Regex.IsMatch(m2.Message, @"(\d+[:.,])+\d+")) {
                                    numGuesses++;
                                    if (numGuesses > 1) break;
                                }
                            }
                            //now add their message to the correct list
                            if (numGuesses < 2) correct.Add(m);
                            else doubled.Add(m);
                        }
                    }
                    //create and populate the response string
                    string response = "";
                    if (correct.Count == 0) {
                        //no winners
                        response += Program.settings.noWinners.Replace("{time}", time)+" ";
                    } else {
                        //some winners
                        response += Program.settings.winnerPreamble+" ";
                        correct.ForEach(m => response += Program.settings.winner.Replace("{displayName}", m.DisplayName).Replace("{message}", m.Message)+" ");
                    }
                    if (doubled.Count > 0 && Program.settings.showDoubled) {
                        //some people doubled up on guesses and might think they won
                        response += Program.settings.doubledPreamble+" ";
                        doubled.ForEach(m => response += Program.settings.doubled.Replace("{displayName}", m.DisplayName).Replace("{message}", m.Message) + " ");
                    }
                    Client.SendMessage(Program.settings.channelName, response);
                    //now handle updating the UI
                    UpdateWinnersFunc f = Program.mainWindow.UpdateWinners;
                    Program.mainWindow.Invoke(f, correct, time);
                }
            }
        }

        public void StartGuessing(string channel)
        {
            if (IsActive) return;
            IsActive = true;
            messages.Clear();
            if (Program.settings.useStartMessage) Client.SendMessage(channel, Program.settings.startMessage);
            Func f = Program.mainWindow.StartGuesses;
            Program.mainWindow.Invoke(f);
        }
        public void StopGuessing(string channel)
        {
            if (!IsActive) return;
            IsActive = false;
            if (Program.settings.useStopMessage) Client.SendMessage(channel, Program.settings.stopMessage);
            Func f = Program.mainWindow.StopGuesses;
            Program.mainWindow.Invoke(f);
        }

        public void AddOnMessage(MessageDelegate d)
        {
            Client.OnMessageReceived += (s, a) => d.Invoke(a.ChatMessage);
        }

    }
}
