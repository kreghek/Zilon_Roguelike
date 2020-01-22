using System;

using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.World
{
    /// <summary>
    /// Объект для хранения контекста обновления состояния мира.
    /// </summary>
    public sealed class GlobeIterationContext
    {
        public GlobeIterationContext(IActorTaskSource botTaskSource)
        {
            BotTaskSource = botTaskSource ?? throw new ArgumentNullException(nameof(botTaskSource));
        }

        /// <summary>
        /// Источник команд для ботов.
        /// </summary>
        public IActorTaskSource BotTaskSource { get; }
    }
}
