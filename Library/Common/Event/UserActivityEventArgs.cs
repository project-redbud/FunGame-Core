using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Library.Common.Event
{
    public class UserActivityEventArgs(long userId, ActivityState activityState, DateTime startTime, DateTime endTime) : GeneralEventArgs
    {
        public long UserId { get; } = userId;
        public ActivityState ActivityState { get; } = activityState;
        public DateTime StartTime { get; } = startTime;
        public DateTime EndTime { get; } = endTime;
        public bool AllowAccess { get; set; } = false;
    }
}
