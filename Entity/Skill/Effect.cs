using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Entity
{
    /// <summary>
    /// 特殊效果类，需要继承
    /// </summary>
    public abstract class Effect : BaseEntity
    {
        /// <summary>
        /// 作用于自身
        /// </summary>
        public virtual bool TargetSelf { get; } = false;

        /// <summary>
        /// 作用目标数量
        /// </summary>
        public virtual int TargetCount { get; } = 0;

        /// <summary>
        /// 作用范围
        /// </summary>
        public virtual double TargetRange { get; } = 0;

        /// <summary>
        /// 魔法类型
        /// </summary>
        public virtual MagicType MagicType { get; } = MagicType.None;

        /// <summary>
        /// 效果描述
        /// </summary>
        public virtual string Description { get; } = "";

        /// <summary>
        /// 等级 通常跟随技能的等级，不需要修改
        /// </summary>
        public int Level
        {
            get
            {
                return Math.Max(0, _Level);
            }
            set
            {
                _Level = Math.Max(0, value);
            }
        }

        /// <summary>
        /// 触发效果
        /// </summary>
        /// <param name="args"></param>
        public abstract void Trigger(Dictionary<string, object> args);

        public override bool Equals(IBaseEntity? other)
        {
            return other is Effect c && c.Name == Name;
        }

        /// <summary>
        /// 等级
        /// </summary>
        private int _Level = 1;
    }
}
