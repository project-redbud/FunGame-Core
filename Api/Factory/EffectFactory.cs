﻿using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Base;

namespace Milimoe.FunGame.Core.Api.Factory
{
    internal class EffectFactory : IFactory<Effect>
    {
        public Type EntityType => typeof(Effect);

        public Effect Create()
        {
            return new();
        }
    }
}