using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Zilon.Core.Client;
using Zilon.Core.PersonModules;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    internal class PersonEffectPanel
    {
        private readonly Texture2D _effectBackgroundTexture;
        private readonly Texture2D _effectIconTexture;
        private readonly ISectorUiState _uiState;

        public PersonEffectPanel(Game game, ISectorUiState uiState)
        {
            _uiState = uiState;

            _effectBackgroundTexture = game.Content.Load<Texture2D>("Sprites/ui/Effects/EffectBackground");
            _effectIconTexture = game.Content.Load<Texture2D>("Sprites/ui/Effects/HungerLesserEffect");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var person = _uiState.ActiveActor?.Actor?.Person;
            if (person is null)
            {
                return;
            }

            var effectsModule = person.GetModule<IEffectsModule>();

            var effectIndex = 0;
            foreach (var effect in effectsModule.Items)
            {
                var x = effectIndex * 18;
                spriteBatch.Draw(_effectBackgroundTexture, new Rectangle(x, 0, 16, 16), Color.Yellow);
                spriteBatch.Draw(_effectIconTexture, new Vector2(x, 0), Color.DarkSlateGray);
                effectIndex++;
            }
        }
    }
}