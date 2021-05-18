
using Microsoft.Xna.Framework;

namespace CDT.LIV.MonoGameClient.ViewModels.MainScene
{
    public interface IActorStateEngine
    { 
        public bool IsComplete { get; }
        public void Update(GameTime gameTime);
    }
}
