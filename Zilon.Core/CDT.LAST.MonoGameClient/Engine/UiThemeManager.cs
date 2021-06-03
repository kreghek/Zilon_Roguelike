using System;

namespace CDT.LAST.MonoGameClient.Engine
{
    internal static class UiThemeManager
    {
        private static IUiSoundStorage? _soundStorage;

        public static IUiSoundStorage? SoundStorage
        {
            get => _soundStorage;
            set
            {
                if (_soundStorage?.ContentWasLoaded == false)
                {
                    throw new InvalidOperationException("Sound Storage must load content before assigning in Ui theme manager.");
                }

                _soundStorage = value;
            }
        }
    }
}