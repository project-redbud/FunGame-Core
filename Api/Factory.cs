using FunGame.Core.Entity;

namespace FunGame.Core.Api
{
    public class Factory
    {
        /// <summary>
        /// 支持动态扩展的工厂实例
        /// </summary>
        public static Factory OpenFactory { get; } = new();

        internal HashSet<EntityFactoryDelegate<Character>> CharacterFactories { get; } = [];
        internal HashSet<EntityFactoryDelegate<Inventory>> InventoryFactories { get; } = [];
        internal HashSet<EntityFactoryDelegate<Skill>> SkillFactories { get; } = [];
        internal HashSet<EntityFactoryDelegate<Effect>> EffectFactories { get; } = [];
        internal HashSet<EntityFactoryDelegate<Item>> ItemFactories { get; } = [];
        internal HashSet<EntityFactoryDelegate<Room>> RoomFactories { get; } = [];
        internal HashSet<EntityFactoryDelegate<User>> UserFactories { get; } = [];

        public delegate T? EntityFactoryDelegate<T>(long id, string name, Dictionary<string, object> args);

        /// <summary>
        /// 注册工厂方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="d"></param>
        public void RegisterFactory<T>(EntityFactoryDelegate<T> d)
        {
            if (typeof(T) == typeof(Character) && d is EntityFactoryDelegate<Character> character)
            {
                CharacterFactories.Add(character);
            }
            if (typeof(T) == typeof(Inventory) && d is EntityFactoryDelegate<Inventory> inventory)
            {
                InventoryFactories.Add(inventory);
            }
            if (typeof(T) == typeof(Skill) && d is EntityFactoryDelegate<Skill> skill)
            {
                SkillFactories.Add(skill);
            }
            if (typeof(T) == typeof(Effect) && d is EntityFactoryDelegate<Effect> effect)
            {
                EffectFactories.Add(effect);
            }
            if (typeof(T) == typeof(Item) && d is EntityFactoryDelegate<Item> item)
            {
                ItemFactories.Add(item);
            }
            if (typeof(T) == typeof(Room) && d is EntityFactoryDelegate<Room> room)
            {
                RoomFactories.Add(room);
            }
            if (typeof(T) == typeof(User) && d is EntityFactoryDelegate<User> user)
            {
                UserFactories.Add(user);
            }
        }

        /// <summary>
        /// 移除工厂方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="d"></param>
        public void UnRegisterFactory<T>(EntityFactoryDelegate<T> d)
        {
            if (typeof(T) == typeof(Character) && d is EntityFactoryDelegate<Character> character)
            {
                CharacterFactories.Remove(character);
            }
            if (typeof(T) == typeof(Inventory) && d is EntityFactoryDelegate<Inventory> inventory)
            {
                InventoryFactories.Remove(inventory);
            }
            if (typeof(T) == typeof(Skill) && d is EntityFactoryDelegate<Skill> skill)
            {
                SkillFactories.Remove(skill);
            }
            if (typeof(T) == typeof(Effect) && d is EntityFactoryDelegate<Effect> effect)
            {
                EffectFactories.Remove(effect);
            }
            if (typeof(T) == typeof(Item) && d is EntityFactoryDelegate<Item> item)
            {
                ItemFactories.Remove(item);
            }
            if (typeof(T) == typeof(Room) && d is EntityFactoryDelegate<Room> room)
            {
                RoomFactories.Remove(room);
            }
            if (typeof(T) == typeof(User) && d is EntityFactoryDelegate<User> user)
            {
                UserFactories.Remove(user);
            }
        }

        /// <summary>
        /// 构造一个实体实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedInstanceClassException"></exception>
        public T GetInstance<T>(long id, string name, Dictionary<string, object> args)
        {
            if (typeof(T) == typeof(Character))
            {
                foreach (EntityFactoryDelegate<Character> d in CharacterFactories)
                {
                    try
                    {
                        if (d.Invoke(id, name, args) is T character)
                        {
                            return character;
                        }
                    }
                    catch
                    {
                        throw;
                    }
                }
                return (T)(object)new Character();
            }
            if (typeof(T) == typeof(Inventory))
            {
                foreach (EntityFactoryDelegate<Inventory> d in InventoryFactories)
                {
                    try
                    {
                        if (d.Invoke(id, name, args) is T inventory)
                        {
                            return inventory;
                        }
                    }
                    catch
                    {
                        throw;
                    }
                }
                return (T)(object)new Inventory(new User());
            }
            if (typeof(T) == typeof(Skill))
            {
                foreach (EntityFactoryDelegate<Skill> d in SkillFactories)
                {
                    try
                    {
                        if (d.Invoke(id, name, args) is T skill)
                        {
                            return skill;
                        }
                    }
                    catch
                    {
                        throw;
                    }
                }

                Skill openSkill = new OpenSkill(id, name, args);
                if (args.TryGetValue("values", out object? value) && value is Dictionary<string, object> dict)
                {
                    foreach (string key in dict.Keys)
                    {
                        openSkill.Values[key] = dict[key];
                    }
                }

                return (T)(object)openSkill;
            }
            if (typeof(T) == typeof(Effect))
            {
                foreach (EntityFactoryDelegate<Effect> d in EffectFactories)
                {
                    try
                    {
                        if (d.Invoke(id, name, args) is T effect)
                        {
                            return effect;
                        }
                    }
                    catch
                    {
                        throw;
                    }
                }
                return (T)(object)new Effect();
            }
            if (typeof(T) == typeof(Item))
            {
                foreach (EntityFactoryDelegate<Item> d in ItemFactories)
                {
                    try
                    {
                        if (d.Invoke(id, name, args) is T item)
                        {
                            return item;
                        }
                    }
                    catch
                    {
                        throw;
                    }
                }
                return (T)(object)new OpenItem(id, name, args);
            }
            if (typeof(T) == typeof(Room))
            {
                foreach (EntityFactoryDelegate<Room> d in RoomFactories)
                {
                    try
                    {
                        if (d.Invoke(id, name, args) is T room)
                        {
                            return room;
                        }
                    }
                    catch
                    {
                        throw;
                    }
                }
                return (T)(object)new Room();
            }
            if (typeof(T) == typeof(User))
            {
                foreach (EntityFactoryDelegate<User> d in UserFactories)
                {
                    try
                    {
                        if (d.Invoke(id, name, args) is T user)
                        {
                            return user;
                        }
                    }
                    catch
                    {
                        throw;
                    }
                }
                return (T)(object)new User();
            }
            throw new NotSupportedInstanceClassException();
        }

        /// <summary>
        /// 此方法使用 <see cref="EntityModuleConfig{T}"/> 取得一个实体字典
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="module_name"></param>
        /// <param name="file_name"></param>
        /// <returns></returns>
        public static Dictionary<string, T> GetGameModuleInstances<T>(string module_name, string file_name) where T : BaseEntity
        {
            EntityModuleConfig<T> config = new(module_name, file_name);
            config.LoadConfig();
            if (typeof(T) == typeof(Skill))
            {
                OpenSkillAdapter.Adaptation(config);
            }
            if (typeof(T) == typeof(Item))
            {
                OpenItemAdapter.Adaptation(config);
            }
            return config;
        }

        /// <summary>
        /// 使用 <see cref="EntityModuleConfig{T}"/> 构造一个实体字典并保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="module_name"></param>
        /// <param name="file_name"></param>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static void CreateGameModuleEntityConfig<T>(string module_name, string file_name, Dictionary<string, T> dict) where T : BaseEntity
        {
            EntityModuleConfig<T> config = new(module_name, file_name);
            foreach (string key in dict.Keys)
            {
                config[key] = dict[key];
            }
            config.SaveConfig();
        }
    }
}
