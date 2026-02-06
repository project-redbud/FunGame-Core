using Milimoe.FunGame.Core.Entity;

namespace Milimoe.FunGame.Core.Model.PrefabricatedEntity
{
    /// <summary>
    /// 继承此类以表示勇气指令技能
    /// </summary>
    public class CourageCommandSkill(long id, string name, Dictionary<string, object> args, Character? character = null) : OpenSkill(id, name, args, character)
    {

    }
}
