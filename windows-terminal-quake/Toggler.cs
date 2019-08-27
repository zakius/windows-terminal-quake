using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsTerminalQuake.Native;

namespace WindowsTerminalQuake
{
    public class Toggler : IDisposable
    {
        private Process _process;
      
        public Toggler(Process process)
        {
            _process = process;

            var isOpen = false;

            var stepCount = 0;

            HotKeyManager.RegisterHotKey(Keys.Oemtilde, KeyModifiers.Control);

            HotKeyManager.HotKeyPressed += (s, a) =>
            {
                if (isOpen)
                {
                    isOpen = false;

                    if (stepCount > 0)
                    {
                        User32.ShowWindow(_process.MainWindowHandle, NCmdShow.RESTORE);
                        User32.SetForegroundWindow(_process.MainWindowHandle);

                        var bounds = GetScreenWithCursor().Bounds;

                        for (int i = stepCount - 1; i >= 0; i--)
                        {
                            User32.MoveWindow(_process.MainWindowHandle, bounds.X, bounds.Y + (-bounds.Height + (bounds.Height / stepCount * i)), bounds.Width, bounds.Height, true);

                            Task.Delay(1).GetAwaiter().GetResult();
                        }
                        User32.SetWindowLong(process.MainWindowHandle, User32.GWL_EX_STYLE, (User32.GetWindowLong(process.MainWindowHandle, User32.GWL_EX_STYLE) | User32.WS_EX_TOOLWINDOW) & ~User32.WS_EX_APPWINDOW);
                        User32.ShowWindow(_process.MainWindowHandle, NCmdShow.MINIMIZE);
                    }
                    else
                    {
                        User32.SetForegroundWindow(_process.MainWindowHandle);
                        User32.ShowWindow(_process.MainWindowHandle, NCmdShow.MINIMIZE);
                        User32.ShowWindow(_process.MainWindowHandle, NCmdShow.HIDE);
                    }                    
                }
                else
                {
                    isOpen = true;

                    if (stepCount > 0)
                    {
                        User32.ShowWindow(_process.MainWindowHandle, NCmdShow.RESTORE);
                        User32.SetForegroundWindow(_process.MainWindowHandle);

                        var bounds = GetScreenWithCursor().Bounds;

                        for (int i = 1; i <= stepCount; i++)
                        {
                            User32.MoveWindow(_process.MainWindowHandle, bounds.X, bounds.Y + (-bounds.Height + (bounds.Height / stepCount * i)), bounds.Width, bounds.Height, true);

                            Task.Delay(1).GetAwaiter().GetResult();
                        }
                    }

                    User32.SetForegroundWindow(_process.MainWindowHandle);
                    User32.ShowWindow(_process.MainWindowHandle, NCmdShow.MAXIMIZE);
                }
            };
            
           
            Thread.Sleep(500);
            User32.SetForegroundWindow(_process.MainWindowHandle);
            User32.ShowWindow(_process.MainWindowHandle, NCmdShow.MINIMIZE);
            User32.ShowWindow(_process.MainWindowHandle, NCmdShow.HIDE);
        }

        public void Dispose()
        {
            ResetTerminal(_process);
        }

        private static Screen GetScreenWithCursor()
        {
            return Screen.AllScreens.FirstOrDefault(s => s.Bounds.Contains(Cursor.Position));
        }

        private static void ResetTerminal(Process process)
        {
            User32.SetForegroundWindow(process.MainWindowHandle);
            User32.ShowWindow(process.MainWindowHandle, NCmdShow.MAXIMIZE);
        }
    }
}