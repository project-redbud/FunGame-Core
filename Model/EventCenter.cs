using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Event;

namespace Milimoe.FunGame.Core.Model
{
    public class EventCenter
    {
        public static EventCenter Instance { get; } = new();

        public Dictionary<string, HashSet<Activity>> Events { get; } = [];

        public Dictionary<string, Store> Stores { get; set; } = [];

        public HashSet<Activity> this[string name]
        {
            get
            {
                if (Events.TryGetValue(name, out HashSet<Activity>? activities) && activities != null)
                {
                    return activities;
                }
                return [];
            }
            set
            {
                AddOrUpdateEvent(name, value);
            }
        }

        public Activity? GetActivity(string eventName, string activityName)
        {
            if (Events.TryGetValue(eventName, out HashSet<Activity>? activities) && activities != null && activities.FirstOrDefault(a => a.Name == activityName) is Activity activity)
            {
                return activity;
            }
            return null;
        }

        public void AddOrUpdateEvent(string name, IEnumerable<Activity> activities)
        {
            Events[name] = new(activities);
        }

        public bool RemoveEvent(string name)
        {
            return Events.Remove(name);
        }

        public Store? GetStore(string name)
        {
            if (Stores.TryGetValue(name, out Store? store))
            {
                return store;
            }
            return null;
        }

        public void AddOrUpdateStore(string name, Store store)
        {
            Stores[name] = store;
        }

        public bool RemoveStore(string name)
        {
            return Stores.Remove(name);
        }

        public bool AddActivity(string eventName, Activity activity)
        {
            if (Events.TryGetValue(eventName, out HashSet<Activity>? activities) && activities != null)
            {
                return activities.Add(activity);
            }
            else
            {
                Events[eventName] = [activity];
            }
            return false;
        }

        public bool RemoveActivity(string eventName, Activity activity)
        {
            if (Events.TryGetValue(eventName, out HashSet<Activity>? activities) && activities != null)
            {
                return activities.Remove(activity);
            }
            return false;
        }

        public void RegisterUserAccessEventHandler(string eventName, Action<ActivityEventArgs> handler)
        {
            if (Events.TryGetValue(eventName, out HashSet<Activity>? activities) && activities != null)
            {
                foreach (Activity activity in activities)
                {
                    activity.UserAccess += handler;
                }
            }
        }

        public void RegisterUserGetActivityInfoEventHandler(string eventName, Action<ActivityEventArgs> handler)
        {
            if (Events.TryGetValue(eventName, out HashSet<Activity>? activities) && activities != null)
            {
                foreach (Activity activity in activities)
                {
                    activity.UserGetActivityInfo += handler;
                }
            }
        }

        public void UnRegisterUserAccess(string eventName)
        {
            if (Events.TryGetValue(eventName, out HashSet<Activity>? activities) && activities != null)
            {
                foreach (Activity activity in activities)
                {
                    activity.UnRegisterUserAccess();
                }
            }
        }

        public void UnRegisterUserGetActivityInfo(string eventName)
        {
            if (Events.TryGetValue(eventName, out HashSet<Activity>? activities) && activities != null)
            {
                foreach (Activity activity in activities)
                {
                    activity.UnRegisterUserGetActivityInfo();
                }
            }
        }
    }
}
