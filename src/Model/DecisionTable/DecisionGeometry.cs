using System.Collections.Concurrent;
using System.Reflection;
using FunGame.Core.Entity;
using FunGame.Core.Library.Constant;
using FunGame.Core.Model.Framework;

namespace FunGame.Core.Model.DecisionTable
{
    internal static class DecisionGeometry
    {
        private static readonly ConcurrentDictionary<(Type Implementation, Type Owner, string Method), bool> DefaultMethods = new();

        internal static bool UsesDefaultMethod(object instance, Type owner, string method) =>
            DefaultMethods.GetOrAdd((instance.GetType(), owner, method), key =>
            {
                MethodInfo definition = key.Owner.GetMethod(key.Method)!;
                Type[] parameters = [.. definition.GetParameters().Select(p => p.ParameterType)];
                return key.Implementation.GetMethod(key.Method, parameters)?.DeclaringType == key.Owner;
            });

        internal static bool UsesDefaultCastRange(GameMap map) => UsesDefaultMethod(map, typeof(GameMap), nameof(GameMap.GetGridsByRange));

        internal static bool CoversPlane(GameMap map, Skill skill) =>
            skill.CastAnywhere && UsesDefaultCastRange(map) && skill.CastRange >= (long)map.Length + map.Width - 2;

        internal static bool CanAnchorArea(GameMap map, Skill skill)
        {
            // 自定义投影可能不对称，必须保留它的完整落点查询。
            if (skill.AllowSelectNoCharacterGrid || !UsesDefaultMethod(skill, typeof(Skill), nameof(Skill.SelectNonDirectionalTargets)) ||
                !UsesDefaultMethod(skill, typeof(Skill), nameof(Skill.SelectNonDirectionalTargetsFrom)) ||
                !UsesDefaultMethod(skill, typeof(Skill), nameof(Skill.SelectTargetsByRange)) ||
                !UsesDefaultMethod(skill, typeof(Skill), nameof(Skill.SelectTargetsByRangeFrom))) return false;
            string? method = skill.SkillRangeType switch
            {
                SkillRangeType.Diamond => nameof(GameMap.GetGridsByRange),
                SkillRangeType.Circle => nameof(GameMap.GetGridsByCircleRange),
                SkillRangeType.Square => nameof(GameMap.GetGridsBySquareRange),
                _ => null
            };
            return method != null && UsesDefaultMethod(map, typeof(GameMap), method);
        }
    }
}
