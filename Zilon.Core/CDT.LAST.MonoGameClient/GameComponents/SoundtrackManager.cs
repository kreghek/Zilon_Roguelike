using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace CDT.LAST.MonoGameClient.GameComponents
{
    internal class SoundtrackManagerComponent : GameComponent
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
        private bool _backgroundTrackStarted;

        private string? _state;
        private Song? _titleSong;
        public bool IsInitialized { get; private set; }

        public void Initialize(Song song)
        {
            _titleSong = song;
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
                            if (_titleSong is not null)
                            {
                                MediaPlayer.IsRepeating = true;
                                MediaPlayer.Volume = 0.75f;
                                MediaPlayer.Play(_titleSong, TimeSpan.Zero);
                            }
                        }
                    }

                    break;
            }
        }
    }
}