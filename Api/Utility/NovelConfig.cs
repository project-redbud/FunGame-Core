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
        /// 从指定路径加载配置文件，并根据其文件名，转换为本框架所需的文件<para/>
        /// 需要注意：<paramref name="checkConflict"/> 用于检查加载的文件名是否在配置文件目录中已经存在<para/>
        /// 如果不使用此检查，使用 <see cref="SaveConfig"/> 时可能会覆盖原有文件（程序目录/<see cref="RootPath"/>(通常是 novels)/<paramref name="novelName"/>/[所选的文件名].json）
        /// </summary>
        /// <param name="path"></param>
        /// <param name="novelName"></param>
        /// <param name="checkConflict"></param>
        /// <param name="predicates"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException" />
        /// <exception cref="InvalidOperationException" />
        /// <exception cref="InvalidDataException" />
        public static NovelConfig LoadFrom(string path, string novelName, bool checkConflict = true, Dictionary<string, Func<bool>>? predicates = null)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"找不到文件：{path}");
            }

            string fileName = Path.GetFileNameWithoutExtension(path);
            string dpath = $@"{AppDomain.CurrentDomain.BaseDirectory}{RootPath}/{novelName}";
            string fpath = $@"{dpath}/{fileName}.json";

            if (checkConflict && File.Exists(fpath))
            {
                throw new InvalidOperationException($"文件 {fileName}.json 已存在，请先重命名。");
            }

            // 确保目录存在
            ExistsDirectoryAndCreate(novelName);

            // 复制文件内容
            string json = File.ReadAllText(path, General.DefaultEncoding);
            if (NetworkUtility.JsonDeserialize<Dictionary<string, NovelNode>>(json) is null)
            {
                throw new InvalidDataException($"文件 {path} 内容为空或格式不正确。");
            }
            File.WriteAllText(fpath, json, General.DefaultEncoding);

            // 从新文件加载配置
            NovelConfig config = new(novelName, fileName);
            config.LoadConfig(predicates);

            return config;
        }

        /// <summary>
        /// 从配置文件中读取配置。
        /// </summary>
        /// <param name="predicates">传入定义好的条件字典</param>
        public void LoadConfig(Dictionary<string, Func<bool>>? predicates = null)
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
                    if (obj.Values.TryGetValue(nameof(NovelNode.Previous), out object? value) && value is string prevKey && dict.Values.FirstOrDefault(n => n.Key == prevKey) is NovelNode prev)
                    {
                        obj.Previous = prev;
                    }
                    if (obj.Values.TryGetValue(nameof(NovelNode.NextNodes), out value) && value is List<string> nextKeys)
                    {
                        foreach (string nextKey in nextKeys)
                        {
                            if (dict.TryGetValue(nextKey, out NovelNode? node) && node != null)
                            {
                                obj.NextNodes.Add(node);
                            }
                        }
                    }
                    if (predicates != null)
                    {
                        if (obj.Values.TryGetValue(nameof(NovelNode.AndPredicates), out object? value2) && value2 is List<string> aps)
                        {
                            foreach (string ap in aps)
                            {
                                if (predicates.TryGetValue(ap, out Func<bool>? value3) && value3 != null)
                                {
                                    obj.AndPredicates[ap] = value3;
                                }
                            }
                        }
                        if (obj.Values.TryGetValue(nameof(NovelNode.OrPredicates), out value2) && value2 is List<string> ops)
                        {
                            foreach (string op in ops)
                            {
                                if (predicates.TryGetValue(op, out Func<bool>? value3) && value3 != null)
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
                        if (predicates != null)
                        {
                            if (option.Values.TryGetValue(nameof(NovelNode.AndPredicates), out object? value3) && value3 is List<string> aps)
                            {
                                foreach (string ap in aps)
                                {
                                    if (predicates.TryGetValue(ap, out Func<bool>? value4) && value4 != null)
                                    {
                                        option.AndPredicates[ap] = value4;
                                    }
                                }
                            }
                            if (option.Values.TryGetValue(nameof(NovelNode.OrPredicates), out value3) && value3 is List<string> ops)
                            {
                                foreach (string op in ops)
                                {
                                    if (predicates.TryGetValue(op, out Func<bool>? value4) && value4 != null)
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

        /// <summary>
        /// 检查配置文件目录是否存在
        /// </summary>
        /// <param name="novelName"></param>
        /// <returns></returns>
        public static bool ExistsDirectory(string novelName)
        {
            string dpath = $@"{AppDomain.CurrentDomain.BaseDirectory}{RootPath}/{novelName}";
            return Directory.Exists(dpath);
        }

        /// <summary>
        /// 检查配置文件目录是否存在，不存在则创建
        /// </summary>
        /// <param name="novelName"></param>
        /// <returns></returns>
        public static bool ExistsDirectoryAndCreate(string novelName)
        {
            string dpath = $@"{AppDomain.CurrentDomain.BaseDirectory}{RootPath}/{novelName}";
            bool result = Directory.Exists(dpath);
            if (!result)
            {
                Directory.CreateDirectory(dpath);
            }
            return result;
        }

        /// <summary>
        /// 检查配置文件目录中是否存在指定文件
        /// </summary>
        /// <param name="novelName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool ExistsFile(string novelName, string fileName)
        {
            string dpath = $@"{AppDomain.CurrentDomain.BaseDirectory}{RootPath}/{novelName}";
            string fpath = $@"{dpath}/{fileName}.json";
            return File.Exists(fpath);
        }
    }
}
