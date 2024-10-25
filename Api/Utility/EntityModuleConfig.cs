using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.Core.Api.Utility
{
    /// <summary>
    /// 简易的实体模组配置文件生成器，适用范围：动态扩展技能和物品、保存玩家的存档<para/>
    /// 仅支持继承了 <see cref="BaseEntity"/> 的实体类型，每个 <see cref="EntityModuleConfig{T}"/> 仅保存一种实体类型的数据
    /// <para/>文件会保存为：程序目录/configs/<see cref="ModuleName"/>/<see cref="FileName"/>.json
    /// </summary>
    /// <remarks>
    /// 新建一个配置文件，文件会保存为：程序目录/configs/<paramref name="module_name"/>/<paramref name="file_name"/>.json
    /// </remarks>
    /// <param name="module_name"></param>
    /// <param name="file_name"></param>
    public class EntityModuleConfig<T>(string module_name, string file_name) : Dictionary<string, T> where T : BaseEntity
    {
        /// <summary>
        /// 模组的名称
        /// </summary>
        public string ModuleName { get; set; } = module_name;

        /// <summary>
        /// 配置文件的名称（后缀将是.json）
        /// </summary>
        public string FileName { get; set; } = file_name;

        /// <summary>
        /// 使用索引器给指定key赋值，不存在key会新增
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public new T this[string key]
        {
            get => base[key];
            set
            {
                if (value != null) Add(key, value);
            }
        }

        /// <summary>
        /// 获取指定key的value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T? Get(string key)
        {
            if (TryGetValue(key, out T? value) && value != null)
            {
                return value;
            }
            return null;
        }

        /// <summary>
        /// 添加一个配置，如果已存在key会覆盖
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public new void Add(string key, T value)
        {
            if (value != null)
            {
                if (TryGetValue(key, out _)) base[key] = value;
                else base.Add(key, value);
            }
        }

        /// <summary>
        /// 从配置文件中读取配置。
        /// </summary>
        public void LoadConfig()
        {
            string dpath = $@"{AppDomain.CurrentDomain.BaseDirectory}configs/{ModuleName}";
            string fpath = $@"{dpath}/{FileName}.json";
            if (Directory.Exists(dpath) && File.Exists(fpath))
            {
                string json = File.ReadAllText(fpath, General.DefaultEncoding);
                Dictionary<string, T> dict = NetworkUtility.JsonDeserialize<Dictionary<string, T>>(json) ?? [];
                Clear();
                foreach (string key in dict.Keys)
                {
                    T obj = dict[key];
                    base.Add(key, obj);
                }
            }
        }

        /// <summary>
        /// 将配置保存到配置文件。调用此方法会覆盖原有的.json，请注意备份
        /// </summary>
        public void SaveConfig()
        {
            string json = NetworkUtility.JsonSerialize((Dictionary<string, T>)this);
            string dpath = $@"{AppDomain.CurrentDomain.BaseDirectory}configs/{ModuleName}";
            string fpath = $@"{dpath}/{FileName}.json";
            if (!Directory.Exists(dpath))
            {
                Directory.CreateDirectory(dpath);
            }
            using StreamWriter writer = new(fpath, false, General.DefaultEncoding);
            writer.WriteLine(json);
            writer.Flush();
        }
    }
}
