using Milimoe.FunGame.Core.Entity;

namespace Milimoe.FunGame.Core.Interface.Entity
{
    /// <summary>
    /// ISkill 是技能的通用接口，包含一些基本属性，实现类：<see cref="NormalAttack"/> 和 <see cref="Skill"/>
    /// </summary>
    public interface ISkill : IBaseEntity
    {
        /// <summary>
        /// 此技能所属的角色
        /// </summary>
        public Character? Character { get; }

        /// <summary>
        /// 技能描述
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// 技能的通用描述
        /// </summary>
        public string GeneralDescription { get; }

        /// <summary>
        /// 技能等级，等于 0 时可以称之为尚未学习
        /// </summary>
        public int Level { get; }

        /// <summary>
        /// 是否可用 [ 此项为高优先级 ]
        /// </summary>
        public bool Enable { get; }

        /// <summary>
        /// 效果持续生效中 [ 此项为高优先级 ] [ 此项设置为true后不允许再次释放，防止重复释放 ]
        /// </summary>
        public bool IsInEffect { get; }

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
        /// 选取所有敌对角色，优先级大于 <see cref="CanSelectTargetCount"/>
        /// </summary>
        public bool SelectAllEnemies { get; }

        /// <summary>
        /// 选取所有友方角色，优先级大于 <see cref="CanSelectTargetCount"/>，默认包含自身
        /// </summary>
        public bool SelectAllTeammates { get; }

        /// <summary>
        /// 可选取的作用目标数量
        /// </summary>
        public int CanSelectTargetCount { get; }

        /// <summary>
        /// 可选取的作用范围 [ 单位：格 ]
        /// </summary>
        public int CanSelectTargetRange { get; }

        /// <summary>
        /// 实际魔法消耗 [ 魔法 ]
        /// </summary>
        public double RealMPCost { get; }

        /// <summary>
        /// 实际吟唱时间 [ 魔法 ]
        /// </summary>
        public double RealCastTime { get; }

        /// <summary>
        /// 实际能量消耗 [ 战技 ]
        /// </summary>
        public double RealEPCost { get; }

        /// <summary>
        /// 实际冷却时间
        /// </summary>
        public double RealCD { get; }

        /// <summary>
        /// 剩余冷却时间
        /// </summary>
        public double CurrentCD { get; }

        /// <summary>
        /// 实际硬直时间
        /// </summary>
        public double RealHardnessTime { get; }

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
