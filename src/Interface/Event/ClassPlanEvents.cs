using FunGame.Core.Library.Common.Event;

// 模组需要监听哪个职业规划阶段，就实现对应接口；把实例方法挂到 ClassPlanner.Planned 事件即可
namespace FunGame.Core.Interface
{
    /// <summary>
    /// 选择职业 / 流派（含兼职）
    /// </summary>
    public interface IClassPlanSelectClassEvent
    {
        public void ClassPlanSelectClassEvent(object sender, ClassPlanEventArgs e, Dictionary<string, object> data);
    }

    /// <summary>
    /// 职业升级
    /// </summary>
    public interface IClassPlanUpgradeClassEvent
    {
        public void ClassPlanUpgradeClassEvent(object sender, ClassPlanEventArgs e, Dictionary<string, object> data);
    }

    /// <summary>
    /// 结算职业升级路线图奖励（选择权 / 数值提升 / 技能等级提升）
    /// </summary>
    public interface IClassPlanSettleRewardEvent
    {
        public void ClassPlanSettleRewardEvent(object sender, ClassPlanEventArgs e, Dictionary<string, object> data);
    }

    /// <summary>
    /// 学习职业技能 / 被动（消耗选择权）
    /// </summary>
    public interface IClassPlanLearnClassSkillEvent
    {
        public void ClassPlanLearnClassSkillEvent(object sender, ClassPlanEventArgs e, Dictionary<string, object> data);
    }

    /// <summary>
    /// 分配核心属性（1 级初始分配 / 4 与 9 级数值提升）
    /// </summary>
    public interface IClassPlanAllocateAttributeEvent
    {
        public void ClassPlanAllocateAttributeEvent(object sender, ClassPlanEventArgs e, Dictionary<string, object> data);
    }

    /// <summary>
    /// 选择角色定位
    /// </summary>
    public interface IClassPlanSelectRoleTypesEvent
    {
        public void ClassPlanSelectRoleTypesEvent(object sender, ClassPlanEventArgs e, Dictionary<string, object> data);
    }

    /// <summary>
    /// 学习战斗天赋
    /// </summary>
    public interface IClassPlanLearnTalentEvent
    {
        public void ClassPlanLearnTalentEvent(object sender, ClassPlanEventArgs e, Dictionary<string, object> data);
    }

    /// <summary>
    /// 遗忘已学战斗天赋
    /// </summary>
    public interface IClassPlanForgetTalentEvent
    {
        public void ClassPlanForgetTalentEvent(object sender, ClassPlanEventArgs e, Dictionary<string, object> data);
    }

    /// <summary>
    /// 激活 / 转换战斗天赋
    /// </summary>
    public interface IClassPlanActivateTalentEvent
    {
        public void ClassPlanActivateTalentEvent(object sender, ClassPlanEventArgs e, Dictionary<string, object> data);
    }

    /// <summary>
    /// 洗点
    /// </summary>
    public interface IClassPlanResetEvent
    {
        public void ClassPlanResetEvent(object sender, ClassPlanEventArgs e, Dictionary<string, object> data);
    }
}
