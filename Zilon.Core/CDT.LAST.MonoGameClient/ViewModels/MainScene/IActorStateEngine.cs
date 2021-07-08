using Microsoft.Xna.Framework;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    public interface IActorStateEngine
    {
        public bool IsComplete { get; }
        void Cancel();
        public void Update(GameTime gameTime);
    }
}