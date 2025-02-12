namespace Milimoe.FunGame.Core.Model
{
    public class NovelNode
    {
        public string Key { get; set; } = "";
        public int Priority { get; set; } = 0;
        public NovelNode? Previous { get; set; } = null;
        public List<NovelNode> NextNodes { get; set; } = [];
        public NovelNode? Next => NextNodes.OrderByDescending(n => n.Priority).Where(n => n.ShowNode).FirstOrDefault();
        public List<NovelOption> Options { get; set; } = [];
        public List<NovelOption> AvailableOptions => [.. Options.Where(o => o.ShowOption)];
        public string Name { get; set; } = "";
        public string Name2 { get; set; } = "";
        public string Content { get; set; } = "";
        public string PortraitImagePath { get; set; } = "";
        public Dictionary<string, Func<bool>> AndPredicates { get; set; } = [];
        public Dictionary<string, Func<bool>> OrPredicates { get; set; } = [];
        public bool ShowNode
        {
            get
            {
                bool andResult = AndPredicates.Values.All(predicate => predicate());
                bool orResult = OrPredicates.Values.Any(predicate => predicate());
                return andResult && (OrPredicates.Count == 0 || orResult);
            }
        }
        internal Dictionary<string, object> Values { get; set; } = [];
    }

    public class NovelOption
    {
        public string Key { get; set; } = "";
        public string Name { get; set; } = "";
        public List<NovelNode> Targets { get; set; } = [];
        public Dictionary<string, Func<bool>> AndPredicates { get; set; } = [];
        public Dictionary<string, Func<bool>> OrPredicates { get; set; } = [];
        public bool ShowOption
        {
            get
            {
                bool andResult = AndPredicates.Values.All(predicate => predicate());
                bool orResult = OrPredicates.Values.Any(predicate => predicate());
                return andResult && (OrPredicates.Count == 0 || orResult);
            }
        }
        internal Dictionary<string, object> Values { get; set; } = [];
    }
}
