using System.Text;
using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
{
    public class Activity : BaseEntity
    {
        public DateTime? StartTime { get; set; } = null;
        public DateTime? EndTime { get; set; } = null;
        public string Description { get; set; } = "";
        public ActivityState Status { get; private set; } = ActivityState.Future;
        public HashSet<Quest> Quests { get; set; } = [];
        public long Predecessor { get; set; } = -1;
        public ActivityState PredecessorStatus { get; set; } = ActivityState.Future;

        public Activity(long id, string name, DateTime? startTime = null, DateTime? endTime = null)
        {
            Id = id;
            Name = name;
            StartTime = startTime;
            EndTime = endTime;
        }

        public Activity() { }

        // 事件
        public event Action<ActivityEventArgs>? UserAccess;
        public event Action<ActivityEventArgs>? UserGetActivityInfo;

        public void UnRegisterUserAccess()
        {
            UserAccess = null;
        }

        public void UnRegisterUserGetActivityInfo()
        {
            UserGetActivityInfo = null;
        }

        public void UpdateState()
        {
            ActivityState newState;
            DateTime now = DateTime.Now;
            DateTime? upComingTime = StartTime?.AddHours(-16);

            if (Predecessor != -1 && PredecessorStatus != ActivityState.Ended)
            {
                // 如果有前置活动且前置活动未结束，则当前活动状态为未来
                newState = ActivityState.Future;
                Status = newState;
                return;
            }

            if (upComingTime != null && now < upComingTime)
            {
                newState = ActivityState.Future;
            }
            else if (upComingTime != null && now >= upComingTime && now < StartTime)
            {
                newState = ActivityState.Upcoming;
            }
            else if ((StartTime is null || now >= StartTime) && (EndTime is null || now < EndTime))
            {
                newState = ActivityState.InProgress;
            }
            else
            {
                newState = ActivityState.Ended;
            }

            if (Status != newState)
            {
                Status = newState;
                foreach (Quest quest in Quests)
                {
                    if (newState == ActivityState.InProgress)
                    {
                        if (quest.Status == QuestState.NotStarted && quest.QuestType == QuestType.Progressive)
                        {
                            quest.Status = QuestState.InProgress;
                        }
                    }
                    else if (newState == ActivityState.Ended)
                    {
                        if (quest.Status == QuestState.NotStarted || quest.Status == QuestState.InProgress)
                        {
                            quest.Status = QuestState.Missed;
                        }
                    }
                }
            }
        }

        public bool AllowUserAccess(long userId, long questId = 0)
        {
            UpdateState();
            ActivityEventArgs args = new(userId, questId, this);
            UserAccess?.Invoke(args);
            return args.AllowAccess;
        }

        public void GetActivityInfo(long userId, long questId = 0)
        {
            UpdateState();
            ActivityEventArgs args = new(userId, questId, this);
            UserGetActivityInfo?.Invoke(args);
        }

        public string ToString(bool showQuests)
        {
            UpdateState();
            StringBuilder builder = new();

            builder.AppendLine($"☆--- {Name} ---☆");
            builder.AppendLine($"{Description}");
            builder.AppendLine($"活动状态：{CommonSet.GetActivityStatus(Status)}");
            builder.AppendLine(GetTimeString());

            if (showQuests && Quests.Count > 0)
            {
                builder.AppendLine("=== 任务列表 ===");
                builder.AppendLine(string.Join("\r\n", Quests));
            }

            return builder.ToString().Trim();
        }

        public string GetTimeString(bool full = true)
        {
            if (Predecessor != -1 && PredecessorStatus != ActivityState.Ended)
            {
                return $"在前置活动结束后开启";
            }
            if (full)
            {
                if (StartTime != null && EndTime != null)
                {
                    return $"开始时间：{StartTime.Value.ToString(General.GeneralDateTimeFormatChinese)}\r\n结束时间：{EndTime.Value.ToString(General.GeneralDateTimeFormatChinese)}";
                }
                else if (StartTime != null && EndTime is null)
                {
                    return $"活动时间：{StartTime.Value.ToString(General.GeneralDateTimeFormatChinese)} 起";
                }
                else if (StartTime is null && EndTime != null)
                {
                    return $"活动将在 {EndTime.Value.ToString(General.GeneralDateTimeFormatChinese)} 结束";
                }
            }
            else
            {
                if (StartTime != null && EndTime != null)
                {
                    return $"活动时间：{StartTime.Value.ToString(General.GeneralDateTimeFormatChinese)} - {EndTime.Value.ToString(General.GeneralDateTimeFormatChinese)}";
                }
                else if (StartTime != null && EndTime is null)
                {
                    return $"活动时间：{StartTime.Value.ToString(General.GeneralDateTimeFormatChinese)} 起";
                }
                else if (StartTime is null && EndTime != null)
                {
                    return $"截止于 {EndTime.Value.ToString(General.GeneralDateTimeFormatChinese)}";
                }
            }
            return "活动时间：长期";
        }

        public override string ToString()
        {
            return ToString(true);
        }

        public override bool Equals(IBaseEntity? other)
        {
            return other is Activity && GetIdName() == other?.GetIdName();
        }
    }
}
