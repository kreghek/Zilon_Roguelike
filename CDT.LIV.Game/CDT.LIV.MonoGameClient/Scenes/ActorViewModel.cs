
using Zilon.Core.Client;
using Zilon.Core.Tactics;

namespace CDT.LIV.MonoGameClient.Scenes
{
    internal class ActorViewModel : IActorViewModel
    {
        public IActor Actor { get; set; }
        public object Item => Actor;
    }
}
