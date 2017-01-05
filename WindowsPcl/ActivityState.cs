﻿using System;
using System.IO;

namespace AdjustSdk.Pcl
{
    internal class ActivityState
    {
        // global counters
        internal int EventCount { get; set; }
        internal int SessionCount { get; set; }

        // session atributes
        internal int SubSessionCount { get; set; }
        internal TimeSpan? SessionLenght { get; set; } // all duration in seconds
        internal TimeSpan? TimeSpent { get; set; }
        internal DateTime? LastActivity { get; set; } // all times in seconds sinze 1970
        internal DateTime? CreatedAt { get; set; }
        internal TimeSpan? LastInterval { get; set; }

        // persistent data
        internal Guid Uuid { get; set; }
        internal bool Enabled { get; set; }
        internal bool AskingAttribution { get; set; }
        internal bool UpdatePackages { get; set; }

        internal ActivityState()
        {
            EventCount = 0;
            SessionCount = 0;
            SubSessionCount = -1; // -1 means unknown
            SessionLenght = null;
            TimeSpent = null;
            LastActivity = null;
            CreatedAt = null;
            LastInterval = null;
            Uuid = Guid.NewGuid();
            Enabled = true;
            AskingAttribution = false;
            UpdatePackages = false;
        }

        internal void ResetSessionAttributes(DateTime now)
        {
            SubSessionCount = 1;
            SessionLenght = new TimeSpan();
            TimeSpent = new TimeSpan();
            LastActivity = now;
            CreatedAt = null;
            LastInterval = null;
        }

        public override string ToString()
        {
            return Util.f("ec:{0} sc:{1} ssc:{2} sl:{3:.0} ts:{4:.0} la:{5:.0}",
                EventCount,
                SessionCount,
                SubSessionCount,
                SessionLenght.SecondsFormat(),
                TimeSpent.SecondsFormat(),
                LastActivity.SecondsFormat()
            );
        }

        #region Serialization
        // does not close stream received. Caller is responsible to close if it wants it
        internal static void SerializeToStream(Stream stream, ActivityState activity)
        {
            var writer = new BinaryWriter(stream);

            writer.Write(activity.EventCount);
            writer.Write(activity.SessionCount);
            writer.Write(activity.SubSessionCount);
            writer.Write(Util.SerializeTimeSpanToLong(activity.SessionLenght));
            writer.Write(Util.SerializeTimeSpanToLong(activity.TimeSpent));
            writer.Write(Util.SerializeDatetimeToLong(activity.LastActivity));
            writer.Write(Util.SerializeDatetimeToLong(activity.CreatedAt));
            writer.Write(Util.SerializeTimeSpanToLong(activity.LastInterval));
            writer.Write(activity.Uuid.ToString());
            writer.Write(activity.Enabled);
            writer.Write(activity.AskingAttribution);
            writer.Write(activity.UpdatePackages);
        }

        // does not close stream received. Caller is responsible to close if it wants it
        internal static ActivityState DeserializeFromStream(Stream stream)
        {
            ActivityState activity = null;
            var reader = new BinaryReader(stream);

            activity = new ActivityState();
            activity.EventCount = reader.ReadInt32();
            activity.SessionCount = reader.ReadInt32();
            activity.SubSessionCount = reader.ReadInt32();
            activity.SessionLenght = Util.DeserializeTimeSpanFromLong(reader.ReadInt64());
            activity.TimeSpent = Util.DeserializeTimeSpanFromLong(reader.ReadInt64());
            activity.LastActivity = Util.DeserializeDateTimeFromLong(reader.ReadInt64());
            activity.CreatedAt = Util.DeserializeDateTimeFromLong(reader.ReadInt64());
            activity.LastInterval = Util.DeserializeTimeSpanFromLong(reader.ReadInt64());

            // create Uuid for migrating devices
            activity.Uuid = Util.TryRead(() => Guid.Parse(reader.ReadString()), () => Guid.NewGuid());
            // default value of IsEnabled for migrating devices
            activity.Enabled = Util.TryRead(() => reader.ReadBoolean(), () => true);
            // default value for AskingAttribution for migrating devices
            activity.AskingAttribution = Util.TryRead(() => reader.ReadBoolean(), () => false);
            
            activity.UpdatePackages = Util.TryRead(() => reader.ReadBoolean(), () => false);

            return activity;
        }
        #endregion Serialization
    }
}