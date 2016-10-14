﻿using System;

namespace AdjustSdk.Pcl
{
    public class AdjustInstance
    {
        private IActivityHandler ActivityHandler { get; set; }

        private ILogger _Logger = AdjustFactory.Logger;

        public AdjustInstance()
        {
        }

        public void ApplicationLaunching(AdjustConfig adjustConfig, DeviceUtil deviceUtil)
        {
            if (ActivityHandler != null)
            {
                _Logger.Error("Adjust already initialized");
                return;
            }

            ActivityHandler = AdjustSdk.Pcl.ActivityHandler.GetInstance(adjustConfig, deviceUtil);
        }

        public void TrackEvent(AdjustEvent adjustEvent)
        {
            if (!CheckActivityHandler()) { return; }
            ActivityHandler.TrackEvent(adjustEvent);
        }

        public void ApplicationActivated()
        {
            if (!CheckActivityHandler()) { return; }
            ActivityHandler.TrackSubsessionStart();
        }

        public void ApplicationDeactivated()
        {
            if (!CheckActivityHandler()) { return; }
            ActivityHandler.TrackSubsessionEnd();
        }

        public void SetEnabled(bool enabled)
        {
            if (!CheckActivityHandler()) { return; }
            ActivityHandler.SetEnabled(enabled);
        }

        public bool IsEnabled()
        {
            if (!CheckActivityHandler()) { return false; }
            return ActivityHandler.IsEnabled();
        }

        public void SetOfflineMode(bool offlineMode)
        {
            if (!CheckActivityHandler()) { return; }
            ActivityHandler.SetOfflineMode(offlineMode);
        }

        public void AppWillOpenUrl(Uri uri)
        {
            if (!CheckActivityHandler()) { return; }
            ActivityHandler.OpenUrl(uri);
        }

        private bool CheckActivityHandler()
        {
            if (ActivityHandler == null)
            {
                _Logger.Error("Please initialize Adjust by calling 'ApplicationLaunching' before");
                return false;
            }

            return true;
        }
    }
}