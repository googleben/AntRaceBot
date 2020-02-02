using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using static AntRaceBot.Bot;

namespace AntRaceBot
{

    public class Settings
    {
        public string channelName = "smallant1";
        public string botName = "usernameId";
        public string botPass = "oauth:TokenGoesHere";
        public Keys startKeys = Keys.Q;
        public ModifierKeys startMods = ModifierKeys.Alt | ModifierKeys.Control;
        public Keys stopKeys = Keys.E;
        public ModifierKeys stopMods = ModifierKeys.Alt | ModifierKeys.Control;
        public bool useStartMessage = true;
        public string startMessage = "Start your guesses! Use the format \"xx.xx\" or \"x:xx.xx\" and only guess once - incorrectly formatted guesses won't be counted and guessing twice disqualifies you!";
        public bool useStopMessage = true;
        public string stopMessage = "Stop guessing!";
        public string noWinners = "Nobody guessed {time}. Better luck next time!";
        public string winnerPreamble = "Winners:";
        public string winner = "{displayName} - \"{message}\"";
        public bool showDoubled = true;
        public string doubledPreamble = "These people would've won but guessed twice:";
        public string doubled = "{displayName}";
    }

    

    static class Program
    {

        private delegate void Func();

        public static Settings settings = new Settings();
        public static Bot bot;
        public static MainWindow mainWindow;
        public static KeyboardHook khook;

        private static DateTime hotkeyDebounce = DateTime.MinValue;
        private static XmlSerializer settingsSerializer = new XmlSerializer(typeof(Settings));

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            LoadSettings();
            bot = new Bot();
            mainWindow = new MainWindow();
            SetupHotkeys();
            
            Application.Run(mainWindow);
            
        }

        public static void SaveSettings()
        {
            StreamWriter outw = new StreamWriter("settings.xml", false);
            settingsSerializer.Serialize(outw, settings);
            outw.Close();
        }

        public static void LoadSettings()
        {
            if (File.Exists("settings.xml")) {
                FileStream fs = new FileStream("settings.xml", FileMode.Open);
                settings = (Settings)settingsSerializer.Deserialize(fs);
                fs.Close();
            } else {
                settings = new Settings();
            }
        }

        public static void SetupHotkeys()
        {
            khook = new KeyboardHook();
            khook.RegisterHotKey(settings.startMods, settings.startKeys);
            khook.RegisterHotKey(settings.stopMods, settings.stopKeys);
            khook.KeyPressed += HandleHotkey;
        }

        public static void HandleHotkey(object sender, KeyPressedEventArgs a)
        {
            if (DateTime.Now - hotkeyDebounce < TimeSpan.FromSeconds(1)) return;
            if (a.Modifier == settings.startMods && a.Key == settings.startKeys && !bot.IsActive) {
                bot.StartGuessing(settings.channelName);
            } else if (a.Modifier == settings.stopMods && a.Key == settings.stopKeys && bot.IsActive) {
                bot.StopGuessing(settings.channelName);
            }
            hotkeyDebounce = DateTime.Now;
        }

    }
}
