using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Zilon.Core.Client;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    internal class PersonConditionsPanel
    {
        private readonly Texture2D _conditionBackgroundTexture;
        private readonly Texture2D _conditionIconTexture;
        private readonly Texture2D _conditionHintBackgroundTexture;
        private readonly SpriteFont _hintTitleFont;
        private readonly ISectorUiState _uiState;
        private IPersonEffect? _selectedCondition;

        public PersonConditionsPanel(Game game, ISectorUiState uiState)
        {
            _uiState = uiState;

            _conditionBackgroundTexture = game.Content.Load<Texture2D>("Sprites/ui/PersonConditions/ConditionIconBackground");
            _conditionIconTexture = game.Content.Load<Texture2D>("Sprites/ui/PersonConditions/HungerLesserConditionIcon");
            _conditionHintBackgroundTexture = game.Content.Load<Texture2D>("Sprites/ui/PersonConditions/ConditionHintBackground");
            _hintTitleFont = game.Content.Load<SpriteFont>("Fonts/HintTitle");
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
                spriteBatch.Draw(_conditionBackgroundTexture, new Rectangle(x, 0, 16, 16), Color.Yellow);
                spriteBatch.Draw(_conditionIconTexture, new Vector2(x, 0), Color.DarkSlateGray);
                effectIndex++;
            }

            DrawHintIfSelected(spriteBatch, effectsModule);
        }

        private void DrawHintIfSelected(SpriteBatch spriteBatch, IEffectsModule effectsModule)
        {
            if (_selectedCondition != null)
            {
                var effectTitle = _selectedCondition.ToString();
                var titleTextSizeVector = _hintTitleFont.MeasureString(effectTitle);
                var selectedEffectIndex = effectsModule.Items.ToList().IndexOf(_selectedCondition);
                var hintXPosition = selectedEffectIndex * 18;

                var hintRectangle = new Rectangle(hintXPosition, 18,
                    (int)titleTextSizeVector.X + 4, (int)titleTextSizeVector.Y + 4);

                spriteBatch.Draw(_conditionHintBackgroundTexture, hintRectangle, Color.DarkSlateGray);

                spriteBatch.DrawString(_hintTitleFont, effectTitle, new Vector2(hintRectangle.Left + 2, hintRectangle.Top + 2), Color.Wheat);
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
                _selectedCondition = effectUnderMouse.Effect;
            }
            else
            {
                _selectedCondition = null;
            }
        }
    }
}