using Microsoft.Xna.Framework;

namespace CDT.LAST.MonoGameClient
{
    internal interface IScreen
    {
        IScreen? TargetScreen { get; set; }
        void Draw(GameTime gameTime);
        void Update(GameTime gameTime);
    }
}