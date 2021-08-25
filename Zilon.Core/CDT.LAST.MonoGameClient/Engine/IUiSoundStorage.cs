using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace CDT.LAST.MonoGameClient.Engine
{
    internal interface IUiSoundStorage
    {
        bool ContentWasLoaded { get; }

        SoundEffect GetAlertEffect();
        SoundEffect GetButtonClickEffect();
        SoundEffect GetButtonHoverEffect();

        void LoadContent(ContentManager contentManager);
    }
}