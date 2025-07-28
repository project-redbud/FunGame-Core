namespace Milimoe.FunGame.Core.Api.Utility
{
    public static class LINQExtension
    {
        public static IEnumerable<T> GetPage<T>(this IEnumerable<T> list, int showPage, int pageSize)
        {
            return [.. list.Skip((showPage - 1) * pageSize).Take(pageSize)];
        }

        public static int MaxPage<T>(this IEnumerable<T> list, int pageSize)
        {
            if (pageSize <= 0) pageSize = 1;
            int page = (int)Math.Ceiling((double)list.Count() / pageSize);
            return page > 0 ? page : 1;
        }
    }
}
