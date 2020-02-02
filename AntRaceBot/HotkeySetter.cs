using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AntRaceBot
{
    public partial class HotkeySetter : Form
    {

        private static string KeysToString(ModifierKeys mods, Keys keys)
        {
            string ans = "";
            for (int n = 1; n <= 8; n <<= 1) {
                ModifierKeys m = (ModifierKeys)n;
                if (mods.HasFlag(m)) ans += $" + {m.ToString()}";
            }
            ans += $" + {keys.ToString()}";
            if (ans.Length > 0) ans = ans.Substring(" + ".Length);
            return ans;
        }

        private void setButtonText()
        {
            startButton.Text = KeysToString(Program.settings.startMods, Program.settings.startKeys);
            stopButton.Text = KeysToString(Program.settings.stopMods, Program.settings.stopKeys);
        }

        public HotkeySetter()
        {
            InitializeComponent();
            setButtonText();
        }

        private void doneButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            KeyEventHandler kdh = null;
            kdh = delegate (object s, KeyEventArgs a) {
                if (a.KeyCode == Keys.Menu || a.KeyCode == Keys.ShiftKey || a.KeyCode == Keys.ControlKey) return;
                ModifierKeys mod = 0;
                if (a.Modifiers.HasFlag(Keys.Control)) mod |= AntRaceBot.ModifierKeys.Control;
                if (a.Modifiers.HasFlag(Keys.Alt)) mod |= AntRaceBot.ModifierKeys.Alt;
                if (a.Modifiers.HasFlag(Keys.Shift)) mod |= AntRaceBot.ModifierKeys.Shift;
                Program.settings.startMods = mod;
                Program.settings.startKeys = a.KeyCode;
                startButton.KeyDown -= kdh;
                setButtonText();
                Program.SaveSettings();
            };
            startButton.KeyDown += kdh;
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            KeyEventHandler kdh = null;
            kdh = delegate (object s, KeyEventArgs a) {
                if (a.KeyCode == Keys.Menu || a.KeyCode == Keys.ShiftKey || a.KeyCode == Keys.ControlKey) return;
                ModifierKeys mod = 0;
                if (a.Modifiers.HasFlag(Keys.Control)) mod |= AntRaceBot.ModifierKeys.Control;
                if (a.Modifiers.HasFlag(Keys.Alt)) mod |= AntRaceBot.ModifierKeys.Alt;
                if (a.Modifiers.HasFlag(Keys.Shift)) mod |= AntRaceBot.ModifierKeys.Shift;
                Program.settings.stopMods = mod;
                Program.settings.stopKeys = a.KeyCode;
                stopButton.KeyDown -= kdh;
                setButtonText();
                Program.SaveSettings();
            };
            stopButton.KeyDown += kdh;
        }
    }
}
