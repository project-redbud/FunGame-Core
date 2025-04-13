using Milimoe.FunGame.Core.Entity;

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

        /// <summary>
        /// 获取可选择的目标列表
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="enemys"></param>
        /// <param name="teammates"></param>
        /// <returns></returns>
        public List<Character> GetSelectableTargets(Character caster, List<Character> enemys, List<Character> teammates);
    }
}
