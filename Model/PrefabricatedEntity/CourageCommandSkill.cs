using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;

namespace FunGame.Core.Model.PrefabricatedEntity
{
    /// <summary>
    /// 继承此类以表示勇气指令技能
    /// </summary>
    public abstract class CourageCommandSkill(Character? character = null) : Skill(SkillType.Skill, character)
    {

    }
}
