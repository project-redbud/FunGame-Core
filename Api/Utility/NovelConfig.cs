using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Model;

namespace Milimoe.FunGame.Core.Api.Utility
{
    /// <summary>
    /// 视觉小说文本配置器<para/>
    /// <para/>文件会保存为：程序目录/<see cref="RootPath"/>(通常是 novels)/<see cref="NovelName"/>/<see cref="FileName"/>.json
    /// </summary>
    /// <remarks>
    /// 新建一个配置文件，文件会保存为：程序目录/<see cref="RootPath"/>(通常是 novels)/<paramref name="novel_name"/>/<paramref name="file_name"/>.json
    /// </remarks>
    /// <param name="novel_name"></param>
    /// <param name="file_name"></param>
    public class NovelConfig(string novel_name, string file_name) : Dictionary<string, NovelNode>
    {
        /// <summary>
        /// 配置文件存放的根目录
        /// </summary>
        public static string RootPath { get; set; } = "novels";

        /// <summary>
        /// 模组的名称
        /// </summary>
        public string NovelName { get; set; } = novel_name;

        /// <summary>
        /// 配置文件的名称（后缀将是.json）
        /// </summary>
        public string FileName { get; set; } = file_name;

        /// <summary>
        /// 使用索引器给指定key赋值，不存在key会新增
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public new NovelNode this[string key]
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
        public NovelNode? Get(string key)
        {
            if (TryGetValue(key, out NovelNode? value) && value != null)
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
        public new void Add(string key, NovelNode value)
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
        /// <param name="Predicates">传入定义好的条件字典</param>
        public void LoadConfig(Dictionary<string, Func<bool>>? Predicates = null)
        {
            string dpath = $@"{AppDomain.CurrentDomain.BaseDirectory}{RootPath}/{NovelName}";
            string fpath = $@"{dpath}/{FileName}.json";
            if (Directory.Exists(dpath) && File.Exists(fpath))
            {
                string json = File.ReadAllText(fpath, General.DefaultEncoding);
                Dictionary<string, NovelNode> dict = NetworkUtility.JsonDeserialize<Dictionary<string, NovelNode>>(json) ?? [];
                Clear();
                foreach (string key in dict.Keys)
                {
                    NovelNode obj = dict[key];
                    base.Add(key, obj);
                    if (obj.Values.TryGetValue(nameof(NovelNode.NextNodes), out object? value) && value is List<string> nextKeys)
                    {
                        foreach (string nextKey in nextKeys)
                        {
                            if (dict.TryGetValue(nextKey, out NovelNode? node) && node != null)
                            {
                                obj.NextNodes.Add(node);
                            }
                        }
                    }
                    if (Predicates != null)
                    {
                        if (obj.Values.TryGetValue(nameof(NovelNode.AndPredicates), out object? value2) && value2 is List<string> aps)
                        {
                            foreach (string ap in aps)
                            {
                                if (Predicates.TryGetValue(ap, out Func<bool>? value3) && value3 != null)
                                {
                                    obj.AndPredicates[ap] = value3;
                                }
                            }
                        }
                        if (obj.Values.TryGetValue(nameof(NovelNode.OrPredicates), out value2) && value2 is List<string> ops)
                        {
                            foreach (string op in ops)
                            {
                                if (Predicates.TryGetValue(op, out Func<bool>? value3) && value3 != null)
                                {
                                    obj.OrPredicates[op] = value3;
                                }
                            }
                        }
                    }
                    foreach (NovelOption option in obj.Options)
                    {
                        if (option.Values.TryGetValue(nameof(NovelOption.Targets), out object? value2) && value2 is List<string> targets)
                        {
                            foreach (string targetKey in targets)
                            {
                                if (dict.TryGetValue(targetKey, out NovelNode? node) && node != null)
                                {
                                    option.Targets.Add(node);
                                }
                            }
                        }
                        if (Predicates != null)
                        {
                            if (option.Values.TryGetValue(nameof(NovelNode.AndPredicates), out object? value3) && value3 is List<string> aps)
                            {
                                foreach (string ap in aps)
                                {
                                    if (Predicates.TryGetValue(ap, out Func<bool>? value4) && value4 != null)
                                    {
                                        option.AndPredicates[ap] = value4;
                                    }
                                }
                            }
                            if (option.Values.TryGetValue(nameof(NovelNode.OrPredicates), out value3) && value3 is List<string> ops)
                            {
                                foreach (string op in ops)
                                {
                                    if (Predicates.TryGetValue(op, out Func<bool>? value4) && value4 != null)
                                    {
                                        option.OrPredicates[op] = value4;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// 将配置保存到配置文件。调用此方法会覆盖原有的.json，请注意备份
        /// </summary>
        public void SaveConfig()
        {
            string json = NetworkUtility.JsonSerialize((Dictionary<string, NovelNode>)this);
            string dpath = $@"{AppDomain.CurrentDomain.BaseDirectory}{RootPath}/{NovelName}";
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
