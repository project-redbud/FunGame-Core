using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Base;

namespace Milimoe.FunGame.Core.Api.EntityFactory
{
    internal class CharacterFactory : IFactory<Character>
    {
        public Type EntityType => typeof(Character);

        public Character Create() => Character.GetInstance();
    }
}
