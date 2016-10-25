using System;

namespace DriveHUD.HUD.Services
{
    internal interface IHudReceiver : IDisposable
    {
        void Initialize(string clientHandle);

        void Start();
    }
}
