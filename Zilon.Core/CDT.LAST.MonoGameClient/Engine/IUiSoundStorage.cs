using System;

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace CDT.LAST.MonoGameClient.Engine
{
    internal interface IUiSoundStorage
    {
        bool ContentWasLoaded { get; }
        void LoadContent(ContentManager contentManager);
        SoundEffect GetButtonClickEffect();
        SoundEffect GetButtonHoverEffect();
    }

    internal sealed class UiSoundStorage: IUiSoundStorage
    {
        private SoundEffect? _buttonClickSoundEffect;
        private SoundEffect? _buttonHoverSoundEffect;

        public bool ContentWasLoaded { get; private set; }

        public SoundEffect GetButtonClickEffect() => _buttonClickSoundEffect ?? throw new InvalidOperationException("Sound must be loaded before using.");

        public SoundEffect GetButtonHoverEffect() => _buttonHoverSoundEffect ?? throw new InvalidOperationException("Sound must be loaded before using.");

        public void LoadContent(ContentManager contentManager)
        {
            _buttonClickSoundEffect = contentManager.Load<SoundEffect>("Audio/ButtonClick");
            _buttonHoverSoundEffect = contentManager.Load<SoundEffect>("Audio/ButtonHover");

            ContentWasLoaded = true;
        }
    }
}
