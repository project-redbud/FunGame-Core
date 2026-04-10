namespace Milimoe.FunGame.Core.Api.Utility
{
    public static class LINQExtension
    {
        /// <summary>
        /// 分页查询：返回指定页码的数据
        /// </summary>
        /// <typeparam name="T">集合元素的类型</typeparam>
        /// <param name="source">要分页的源集合</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页显示的元素数量</param>
        /// <returns>当前页的数据</returns>
        public static IEnumerable<T> GetPage<T>(this IEnumerable<T> source, int page, int pageSize)
        {
            return source.Skip((page - 1) * pageSize).Take(pageSize);
        }

        /// <summary>
        /// 计算总页数
        /// </summary>
        /// <typeparam name="T">集合元素的类型</typeparam>
        /// <param name="source">源集合</param>
        /// <param name="pageSize">每页显示的元素数量（必须大于 0）</param>
        /// <returns>总页数（至少为 1）</returns>
        public static int MaxPage<T>(this IEnumerable<T> source, int pageSize)
        {
            if (pageSize <= 0) pageSize = 1;
            int count = source.Count();
            if (count == 0) return 1;
            return (int)Math.Ceiling((double)count / pageSize);
        }
    }
}
