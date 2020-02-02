using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TwitchLib.Client.Models;

namespace AntRaceBot
{
    public partial class MainWindow : Form
    {

        private delegate void Func();

        public MainWindow()
        {
            InitializeComponent();
            InitBotListeners();
            this.FormClosing += (a, b) => Environment.Exit(0);
        }

        public void InitBotListeners()
        {
            Program.bot.Client.OnConnected += (s, a) => {
                Func x = () => {
                    connectedLabel.Text = "Connected";
                    connectedLabel.ForeColor = Color.Green;
                };
                connectedLabel.Invoke(x);
            };
            Program.bot.Client.OnDisconnected += (s, a) => {
                Func x = () => {
                    connectedLabel.Text = "Not Connected";
                    connectedLabel.ForeColor = Color.Red;
                };
                connectedLabel.Invoke(x);
            };
        }

        public void StartGuesses()
        {
            winnersList.Items.Clear();
            isActiveLabel.Text = "Counting Guesses";
            isActiveLabel.ForeColor = Color.Green;
            timeWasLabel.Text = "";
        }

        public void StopGuesses()
        {
            isActiveLabel.Text = "Not Counting Guesses";
            isActiveLabel.ForeColor = Color.Red;
        }

        public void UpdateWinners(List<ChatMessage> winners, string time)
        {
            winnersList.Items.Clear();
            winners.ConvertAll((w) => new ListViewItem(new string[] { w.DisplayName, w.Message })).ForEach(w => winnersList.Items.Add(w));
            timeWasLabel.Text = $"Time Was {time}";
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            Program.bot.StartGuessing(Program.settings.channelName);
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            Program.bot.StopGuessing(Program.settings.channelName);
        }

        private void reconnectButton_Click(object sender, EventArgs e)
        {
            Program.bot.Reconnect();
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            new SettingsWindow().ShowDialog();
        }
    }
}
