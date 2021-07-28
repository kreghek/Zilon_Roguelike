using System;

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

    internal sealed class UiSoundStorage : IUiSoundStorage
    {
        private SoundEffect? _alertEffect;
        private SoundEffect? _buttonClickSoundEffect;
        private SoundEffect? _buttonHoverSoundEffect;

        public bool ContentWasLoaded { get; private set; }

        public SoundEffect GetAlertEffect()
        {
            return _alertEffect ?? throw new InvalidOperationException("Sound must be loaded before using.");
        }

        public SoundEffect GetButtonClickEffect()
        {
            return _buttonClickSoundEffect ?? throw new InvalidOperationException("Sound must be loaded before using.");
        }

        public SoundEffect GetButtonHoverEffect()
        {
            return _buttonHoverSoundEffect ?? throw new InvalidOperationException("Sound must be loaded before using.");
        }

        public void LoadContent(ContentManager contentManager)
        {
            _buttonClickSoundEffect = contentManager.Load<SoundEffect>("Audio/ButtonClick");
            _buttonHoverSoundEffect = contentManager.Load<SoundEffect>("Audio/ButtonHover");
            _alertEffect = contentManager.Load<SoundEffect>("Audio/CriticalPersonConditionAlert");

            ContentWasLoaded = true;
        }
    }
}