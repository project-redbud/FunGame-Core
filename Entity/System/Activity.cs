using System.Text;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
{
    public class Activity(long id, string name, DateTime startTime, DateTime endTime)
    {
        public long Id { get; set; } = id;
        public string Name { get; set; } = name;
        public DateTime StartTime { get; set; } = startTime;
        public DateTime EndTime { get; set; } = endTime;
        public ActivityState Status { get; private set; } = ActivityState.Future;
        public HashSet<Quest> Quests { get; set; } = [];

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
            DateTime upComingTime = StartTime.AddHours(-6);

            if (now < upComingTime)
            {
                newState = ActivityState.Future;
            }
            else if (now >= upComingTime && now < StartTime)
            {
                newState = ActivityState.Upcoming;
            }
            else if (now >= StartTime && now < EndTime)
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

            builder.AppendLine($"☆--- [{Name}] ---☆");
            string status = Status switch
            {
                ActivityState.Future => "预告中",
                ActivityState.Upcoming => "即将开始",
                ActivityState.InProgress => "进行中",
                _ => "已结束"
            };
            builder.AppendLine($"活动状态：{status}");
            builder.AppendLine($"开始时间：{StartTime.ToString(General.GeneralDateTimeFormatChinese)}");
            builder.AppendLine($"结束时间：{EndTime.ToString(General.GeneralDateTimeFormatChinese)}");

            if (showQuests && Quests.Count > 0)
            {
                builder.AppendLine("=== 任务列表 ===");
                builder.AppendLine(string.Join("\r\n", Quests));
            }

            return builder.ToString().Trim();
        }

        public override string ToString()
        {
            return ToString(true);
        }
    }
}
