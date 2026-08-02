using FunGame.Core.Entity;

namespace FunGame.Core.Api
{
    public class OpenSkillAdapter
    {
        public static void Adaptation<T>(EntityModuleConfig<T> config) where T : BaseEntity
        {
            foreach (string key in config.Keys)
            {
                if (config[key] is Skill prev)
                {
                    Skill next = prev.Copy();
                    if (next is T t) config[key] = t;
                }
            }
        }
    }
}
