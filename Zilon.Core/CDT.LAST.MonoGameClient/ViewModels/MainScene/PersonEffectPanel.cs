using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Zilon.Core.Client;
using Zilon.Core.PersonModules;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    internal class PersonEffectPanel
    {
        private readonly Texture2D _effectBackgroundTexture;
        private readonly Texture2D _effectIconTexture;
        private readonly SpriteFont _font;
        private readonly ISectorUiState _uiState;
        private string? _effectDescription;

        public PersonEffectPanel(Game game, ISectorUiState uiState)
        {
            _uiState = uiState;

            _effectBackgroundTexture = game.Content.Load<Texture2D>("Sprites/ui/Effects/EffectBackground");
            _effectIconTexture = game.Content.Load<Texture2D>("Sprites/ui/Effects/HungerLesserEffect");
            _font = game.Content.Load<SpriteFont>("Fonts/Main");
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

            if (_effectDescription != null)
            {
                spriteBatch.DrawString(_font, _effectDescription, new Vector2(0, 18), Color.Wheat);
            }
        }

        public void Update()
        {
            var person = _uiState.ActiveActor?.Actor?.Person;
            if (person is null)
            {
                return;
            }

            var effectsModule = person.GetModule<IEffectsModule>();

            var mouseState = Mouse.GetState();

            var effectRectangles = effectsModule.Items.Select((x, index) => new
            {
                UiRect = new Rectangle(index * 18 - 2, 0, 16, 16),
                Effect = x
            });
            var mouseRectangle = new Rectangle(mouseState.X, mouseState.Y, 1, 1);

            var effectUnderMouse = effectRectangles.FirstOrDefault(x => x.UiRect.Intersects(mouseRectangle));

            if (effectUnderMouse != null)
            {
                _effectDescription = effectUnderMouse.Effect.ToString();
            }
            else
            {
                _effectDescription = null;
            }
        }
    }
}