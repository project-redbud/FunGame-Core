using Milimoe.FunGame.Core.Interface.Base;

namespace Milimoe.FunGame.Core.Entity
{
    public class Team(string name, IEnumerable<Character> charaters)
    {
        public Guid Id { get; set; } = Guid.Empty;
        public string Name { get; set; } = name;
        public List<Character> Members { get; } = new(charaters);
        public int Score { get; set; } = 0;
        public bool IsWinner { get; set; } = false;
        public int Count => Members.Count;

        public List<Character> GetActiveCharacters(IGamingQueue queue)
        {
            return [.. Members.Where(queue.Queue.Contains)];
        }

        public List<Character> GetTeammates(Character character)
        {
            return [.. Members.Where(c => c != character)];
        }

        public List<Character> GetActiveTeammates(IGamingQueue queue, Character character)
        {
            return [.. Members.Where(c => queue.Queue.Contains(c) && c != character)];
        }

        public bool IsOnThisTeam(Character character)
        {
            return Members.Contains(character);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
