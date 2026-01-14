namespace Milimoe.FunGame.Core.Entity
{
    public class Team(string name, IEnumerable<Character> charaters)
    {
        public Guid Id { get; set; } = Guid.Empty;
        public string Name { get; set; } = name;
        public List<Character> Members { get; } = [.. charaters];
        public int Score { get; set; } = 0;
        public bool IsWinner { get; set; } = false;
        public int Count => Members.Count;

        public List<Character> GetActiveCharacters()
        {
            return [.. Members.Where(c => c.HP > 0)];
        }

        public List<Character> GetTeammates(Character character)
        {
            return [.. Members.Where(c => c != character)];
        }

        public List<Character> GetActiveTeammates(Character character)
        {
            return [.. Members.Where(c => c.HP > 0 && c != character)];
        }

        public bool IsOnThisTeam(Character character)
        {
            if (character.Master != null)
            {
                character = character.Master;
            }
            return Members.Contains(character);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
