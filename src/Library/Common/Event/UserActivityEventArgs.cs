using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;

namespace FunGame.Core.Library.Common.Event
{
    public class ActivityEventArgs(long userId, long questId, Activity activity) : EventArgs
    {
        public long UserId { get; } = userId;
        public long QuestId { get; } = questId;
        public Activity Activity { get; } = activity;
        public ActivityState ActivityState { get; } = activity.Status;
        public DateTime? StartTime { get; } = activity.StartTime;
        public DateTime? EndTime { get; } = activity.EndTime;
        public bool AllowAccess { get; set; } = false;
    }
}
