using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.Common.Event
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
