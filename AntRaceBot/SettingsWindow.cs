using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace AntRaceBot
{
    public partial class SettingsWindow : Form
    {

        private void LoadSettings()
        {
            var s = Program.settings;
            channelTextBox.Text = s.channelName;
            usernameTextBox.Text = s.botName;
            passTextBox.Text = s.botPass;
            useStartCheckBox.Checked = s.useStartMessage;
            startTextBox.Text = s.startMessage;
            useStopCheckBox.Checked = s.useStopMessage;
            stopTextBox.Text = s.stopMessage;
            noWinnersTextBox.Text = s.noWinners;
            winnerPreambleTextBox.Text = s.winnerPreamble;
            winnerTextBox.Text = s.winner;
            useDoubleCheckBox.Checked = s.showDoubled;
            doublePreambleTextBox.Text = s.doubledPreamble;
            doubleTextBox.Text = s.doubled;
        }

        private bool SaveSettings()
        {
            var s = Program.settings;
            s.channelName = channelTextBox.Text;
            
            s.useStartMessage = useStartCheckBox.Checked;
            s.startMessage = startTextBox.Text;
            s.useStopMessage = useStopCheckBox.Checked;
            s.stopMessage = stopTextBox.Text;
            s.noWinners = noWinnersTextBox.Text;
            s.winnerPreamble = winnerPreambleTextBox.Text;
            s.winner = winnerTextBox.Text;
            s.showDoubled = useDoubleCheckBox.Checked;
            s.doubledPreamble = doublePreambleTextBox.Text;
            s.doubled = doubleTextBox.Text;
            
            //make sure these values are ok
            s.botName = usernameTextBox.Text;
            s.botPass = passTextBox.Text;
            try {
                ConnectionCredentials cred = new ConnectionCredentials(Program.settings.botName, Program.settings.botPass);
                var clientOptions = new ClientOptions {
                    MessagesAllowedInPeriod = 750,
                    ThrottlingPeriod = TimeSpan.FromSeconds(30)
                };
                WebSocketClient wsClient = new WebSocketClient(clientOptions);
                var Client = new TwitchClient(wsClient);
                Client.Initialize(cred, Program.settings.channelName);
            } catch (Exception) {
                MessageBox.Show("Error creating the Twitch connection. Please check your username and auth.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            Program.SaveSettings();
            Program.bot.Disconnect();
            Program.bot = new Bot();
            Program.mainWindow.InitBotListeners();
            return true;
        }

        public SettingsWindow()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void doneButton_Click(object sender, EventArgs e)
        {
            if (SaveSettings()) Close();
        }

        private void hotkeyButton_Click(object sender, EventArgs e)
        {
            new HotkeySetter().ShowDialog();
        }
    }
}
