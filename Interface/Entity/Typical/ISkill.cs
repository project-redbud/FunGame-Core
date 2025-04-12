namespace Milimoe.FunGame.Core.Interface.Entity
{
    public interface ISkill
    {
        /// <summary>
        /// 可选取自身
        /// </summary>
        public bool CanSelectSelf { get; }

        /// <summary>
        /// 可选取敌对角色
        /// </summary>
        public bool CanSelectEnemy { get; }

        /// <summary>
        /// 可选取友方角色
        /// </summary>
        public bool CanSelectTeammate { get; }
        /// <summary>
        /// 可选取的作用目标数量
        /// </summary>
        public int CanSelectTargetCount { get; }

        /// <summary>
        /// 可选取的作用范围
        /// </summary>
        public double CanSelectTargetRange { get; }

    }
}
