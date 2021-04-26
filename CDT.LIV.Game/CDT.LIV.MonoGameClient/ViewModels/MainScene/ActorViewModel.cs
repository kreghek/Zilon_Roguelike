
using Zilon.Core.Client;
using Zilon.Core.Tactics;

namespace CDT.LIV.MonoGameClient.ViewModels.MainScene
{
    internal class ActorViewModel : IActorViewModel
    {
        public IActor Actor { get; set; }
        public object Item => Actor;
    }
}
