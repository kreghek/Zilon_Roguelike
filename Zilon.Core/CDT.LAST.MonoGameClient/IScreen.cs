
using Microsoft.Xna.Framework;

namespace CDT.LAST.MonoGameClient
{
    internal interface IScreen
    {
        void Draw(GameTime gameTime);
        void Update(GameTime gameTime);
        IScreen? TargetScreen { get; set; }
    }
}