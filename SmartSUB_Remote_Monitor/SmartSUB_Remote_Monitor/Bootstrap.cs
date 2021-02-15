using System;
using SmartSUB_Remote_Monitor.Services;

namespace SmartSUB_Remote_Monitor
{
    public static class Bootstrap
    {
        public static void Begin(Func<IDeviceInstallationService> deviceInstallationService)
        {
            ServiceContainer.Register(deviceInstallationService);

            ServiceContainer.Register<ISmartSUBNotificationActionService>(()
                => new NotificationActionService());
        }
    }
}