﻿using System;
using System.Collections.Generic;

namespace AdjustSdk.Pcl
{
    public interface IActivityHandler
    {
        void Init(AdjustConfig adjustConfig, DeviceUtil deviceUtil);

        void FinishedTrackingActivity(Dictionary<string, string> jsonDict);

        void TrackEvent(AdjustEvent adjustEvent);

        void TrackSubsessionEnd();

        void TrackSubsessionStart();

        void SetEnabled(bool enabled);

        bool IsEnabled();

        void SetOfflineMode(bool offline);

        void OpenUrl(Uri uri);

        bool UpdateAttribution(AdjustAttribution attribution);

        void SetAskingAttribution(bool askingAttribution);

        ActivityPackage GetAttributionPackage();

        ActivityPackage GetDeeplinkClickPackage(Dictionary<string, string> extraParameters, 
            AdjustAttribution attribution, 
            string deeplink);
    }
}