using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace CDT.LAST.MonoGameClient.GameComponents
{
    class SoundtrackManagerComponent : GameComponent
    {
        private SoundtrackManager? _soundtrackManager;

        public SoundtrackManagerComponent(Game game) : base(game)
        {
        }

        public void Initialize(SoundtrackManager soundtrackManager)
        {
            _soundtrackManager = soundtrackManager;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_soundtrackManager is not null)
            {
                _soundtrackManager.Update();
            }
        }
    }


    internal sealed class SoundtrackManager
    {
        public bool IsInitialized { get; private set; }
        private bool _backgroundTrackStarted;
        private Song? titleSong;

        private string? _state;

        public void Initialize(Song song)
        {
            titleSong = song;
            IsInitialized = true;
        }

        public void PlayBackgroundTrack()
        {
            _state = "title";
        }

        public void PlaySilence()
        {
            _state = null; // means silence.
        }

        public void Update()
        {
            if (!IsInitialized)
            {
                return;
            }

            switch (_state)
            {
                case null:
                    _backgroundTrackStarted = false;
                    MediaPlayer.Stop();
                    break;

                case "title":
                    if (!_backgroundTrackStarted)
                    {
                        _backgroundTrackStarted = true;
                        if (MediaPlayer.State != MediaState.Playing)
                        {
                            if (titleSong is not null)
                            {
                                MediaPlayer.IsRepeating = true;
                                MediaPlayer.Volume = 0.75f;
                                MediaPlayer.Play(titleSong, TimeSpan.Zero);
                            }
                        }
                    }
                    break;
            }

        }
    }
}
