using System.Runtime.InteropServices;
using Gma.System.MouseKeyHook;

namespace App
{
    internal class Watcher
    {
        public const int KEYEVENTF_EXTENDEDKEY = 0x0001; // key down flag
        public const int KEYEVENTF_KEYUP = 0x0002; // key up flag

        // https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes
        public const int VK_WEAPON_HEAL = 0x31; // '1' key
        public const int VK_WEAPON_ATTACK = 0x32; // '2' key

        public static void Do(Action quit)
        {
            [DllImport("user32.dll", SetLastError = true)]
            static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

            bool isEnabled = false;
            bool isHealing = true;

            Console.Write("Press '-' to enable/disable app.\n\n");

            Hook.GlobalEvents().MouseDown += (sender, e) =>
            {
                if (!isEnabled)
                {
                    return;
                }

                if (e.Button == MouseButtons.Left)
                {
                    if (!isHealing)
                    {
                        Console.Write("Attacking!\n");
                    } else
                    {
                        keybd_event(VK_WEAPON_ATTACK, 0, KEYEVENTF_EXTENDEDKEY, 0);
                        keybd_event(VK_WEAPON_ATTACK, 0, KEYEVENTF_KEYUP, 0);
                    }
                    
                }
                
                else if (e.Button == MouseButtons.Right)
                {
                    if (isHealing)
                    {
                        Console.Write("Healing!\n");
                    } else
                    {
                        keybd_event(VK_WEAPON_HEAL, 0, KEYEVENTF_EXTENDEDKEY, 0);
                        keybd_event(VK_WEAPON_HEAL, 0, KEYEVENTF_KEYUP, 0);
                    }
                    
                }
            };
            Hook.GlobalEvents().KeyPress += (sender, e) =>
            {
                if (e.KeyChar == '-')
                {
                    isEnabled = !isEnabled;
                    if (!isEnabled)
                    {
                        isHealing = true;
                    }

                    Console.Write("State: " + (isEnabled ? "enabled" : "disabled") + "\n");
                    if (isEnabled)
                    {
                        Console.Write("Switching to default action: Healing\n");
                    }
                }
                else if (e.KeyChar == '1')
                {
                    isHealing = true;
                    Console.Write("Switching to Healing\n");
                }
                else if (e.KeyChar == '2')
                {
                    isHealing = false;
                    Console.Write("Switching to Attacking\n");
                }
            };
        }

        public static void Main()
        {
            Do(Application.Exit);
            Application.Run(new ApplicationContext());
        }
    }
}
