#region Usings

using System.Threading;

#endregion

namespace AcePokerSolutions.HUDHelper
{
    public class HudManager
    {
        public void StartMonitor()
        {
            Thread monitorThread = new Thread(MonitorWindows);
            monitorThread.Start();
        }

        private void MonitorWindows()
        {
        }

        public void CreateWindow()
        {
            HudWindow uc = new HudWindow();
            uc.Show();
        }
    }
}