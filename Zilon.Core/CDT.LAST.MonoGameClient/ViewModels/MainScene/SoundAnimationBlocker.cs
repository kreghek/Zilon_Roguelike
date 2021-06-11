using System;

using Microsoft.Xna.Framework.Audio;

using Zilon.Core.Client.Sector;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    /// <summary>
    /// The blocker with reference to a looped sound effect. Sound effect will be stoped when blocker will be released.
    /// </summary>
    internal class SoundAnimationBlocker : ICommandBlocker
    {
        private readonly SoundEffectInstance _soundEffect;

        public SoundAnimationBlocker(SoundEffectInstance soundEffect)
        {
            _soundEffect = soundEffect;
        }

        public string? DebugName { get; set; }

        public override string? ToString()
        {
            return DebugName;
        }

        private void DoRelease()
        {
            Released?.Invoke(this, new EventArgs());
        }

        public event EventHandler? Released;

        public void Release()
        {
            _soundEffect.Stop();

            if (!_soundEffect.IsDisposed)
            {
                _soundEffect.Dispose();
            }

            DoRelease();
        }
    }
}