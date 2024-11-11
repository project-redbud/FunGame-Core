using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;

namespace Milimoe.FunGame.Core.Api.OpenEntityAdapter
{
    public class OpenItemAdapter
    {
        public static void Adaptation<T>(EntityModuleConfig<T> config) where T : BaseEntity
        {
            foreach (string key in config.Keys)
            {
                if (config[key] is Item prev)
                {
                    Item next = prev.Copy();
                    if (next is T t)
                    {
                        next.SetLevel(1);
                        config[key] = t;
                    }
                }
            }
        }
    }
}
