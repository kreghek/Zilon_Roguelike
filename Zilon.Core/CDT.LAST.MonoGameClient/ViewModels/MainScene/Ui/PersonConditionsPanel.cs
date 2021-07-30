using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using CDT.LAST.MonoGameClient.Engine;
using CDT.LAST.MonoGameClient.GameComponents;
using CDT.LAST.MonoGameClient.Resources;
using CDT.LAST.MonoGameClient.Screens;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Zilon.Core.Client;
using Zilon.Core.Diseases;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Persons.Survival;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.Ui
{
    internal class PersonConditionsPanel
    {
        private const int ICON_SIZE = 32;
        private const int ICON_SPACING = 2;
        private const double ALERT_VISIBLE_DURATION_SECONDS = 3;
        private const double ALERT_DELAY_DURATION_SECONDS = 3;

        private readonly IList<IPersonCondition> _alertedConditions;
        private readonly SoundEffectInstance _alertSoundEffect;
        private readonly Texture2D _alertTexture;

        private readonly int _screenX;
        private readonly int _screenY;
        private readonly SoundtrackManager _soundtrackManager;
        private readonly IUiContentStorage _uiContentStorage;
        private readonly IUiSoundStorage _uiSoundStorage;
        private readonly ISectorUiState _uiState;

        private double _alertCounter;

        private bool _isAlertEffectPlaying;
        private IPersonCondition? _selectedCondition;
        private int? _selectedConditionIconIndex;

        public PersonConditionsPanel(ISectorUiState uiState, int screenX, int screenY,
            IUiContentStorage uiContentStorage, IUiSoundStorage uiSoundStorage, SoundtrackManager soundtrackManager,
            GraphicsDevice graphicsDevice)
        {
            _uiState = uiState;
            _screenX = screenX;
            _screenY = screenY;
            _uiContentStorage = uiContentStorage;
            _uiSoundStorage = uiSoundStorage;
            _soundtrackManager = soundtrackManager;
            _alertSoundEffect = _uiSoundStorage.GetAlertEffect().CreateInstance();

            _alertTexture = CreateTexture(graphicsDevice, ICON_SIZE + ICON_SPACING * 2, ICON_SIZE + ICON_SPACING * 2,
                LastColors.Red);
            _alertedConditions = new List<IPersonCondition>();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var person = _uiState.ActiveActor?.Actor?.Person;
            if (person is null)
            {
                return;
            }

            var conditionsModule = person.GetModule<IConditionsModule>();

            var items = conditionsModule.Items;
            var itemsList = items.ToArray();
            for (var conditionIndex = 0; conditionIndex < itemsList.Length; conditionIndex++)
            {
                var condition = itemsList[conditionIndex];
                var iconX = conditionIndex * (ICON_SIZE + ICON_SPACING) + _screenX;

                var iconTextures = _uiContentStorage.GetConditionIconTextures(condition);

                DrawIconBackground(spriteBatch, iconX, iconTextures.Background);

                DrawIcon(spriteBatch, iconX, iconTextures.Icon);

                if (_alertedConditions.Contains(condition))
                {
                    spriteBatch.Draw(_alertTexture, new Vector2(iconX - ICON_SPACING, _screenY - ICON_SPACING),
                        Color.White);
                }
            }

            DrawHintIfSelected(spriteBatch);
        }

        public void Update(GameTime gameTime)
        {
            var person = _uiState.ActiveActor?.Actor?.Person;
            if (person is null)
            {
                return;
            }

            var conditionsModule = person.GetModule<IConditionsModule>();

            var mouseState = Mouse.GetState();

            var conditionRectangles = conditionsModule.Items.Select((x, index) => new
            {
                UiRect = new Rectangle(index * (ICON_SIZE + ICON_SPACING) - ICON_SPACING + _screenX, _screenY,
                    ICON_SIZE, ICON_SIZE),
                Condition = x,
                IconIndex = index
            });
            var mouseRectangle = new Rectangle(mouseState.X, mouseState.Y, 1, 1);

            var conditionUnderMouse = conditionRectangles.FirstOrDefault(x => x.UiRect.Intersects(mouseRectangle));

            if (conditionUnderMouse != null)
            {
                _selectedCondition = conditionUnderMouse.Condition;
                _selectedConditionIconIndex = conditionUnderMouse.IconIndex;
            }
            else
            {
                _selectedCondition = null;
                _selectedConditionIconIndex = null;
            }

            UpdateAlert(conditionsModule, gameTime);
        }

        private static Texture2D CreateTexture(GraphicsDevice device, int width, int height, Color color)
        {
            //initialize a texture
            var texture = new Texture2D(device, width, height);

            //the array holds the color for each pixel in the texture
            var data = new Color[width * height];
            for (var pixel = 0; pixel < data.Length; pixel++)
            {
                data[pixel] = color;
            }

            //set the color
            texture.SetData(data);

            return texture;
        }

        private void DrawHintIfSelected(SpriteBatch spriteBatch)
        {
            if (_selectedCondition != null && _selectedConditionIconIndex != null)
            {
                var personConditionTitle = GetConditionTitle(_selectedCondition);
                var hintFont = _uiContentStorage.GetHintTitleFont();
                var titleTextSizeVector = hintFont.MeasureString(personConditionTitle);
                var selectedConditionIndex = _selectedConditionIconIndex.Value;
                var hintXPosition = selectedConditionIndex * (ICON_SIZE + ICON_SPACING) + _screenX;

                const int Y_POSITION_UNDER_ICON_SEQUENCE = ICON_SIZE + ICON_SPACING;
                const int HINT_TEXT_SPACING = 8;
                var hintRectangle = new Rectangle(hintXPosition, Y_POSITION_UNDER_ICON_SEQUENCE + _screenY,
                    (int)titleTextSizeVector.X + HINT_TEXT_SPACING * 2,
                    (int)titleTextSizeVector.Y + HINT_TEXT_SPACING * 2);

                var conditionHintBackgroundTexture = _uiContentStorage.GetHintBackgroundTexture();
                spriteBatch.Draw(conditionHintBackgroundTexture, hintRectangle, Color.DarkSlateGray);

                spriteBatch.DrawString(hintFont, personConditionTitle,
                    new Vector2(hintRectangle.Left + HINT_TEXT_SPACING, hintRectangle.Top + HINT_TEXT_SPACING),
                    Color.Wheat);
            }
        }

        private void DrawIcon(SpriteBatch spriteBatch, int iconX, Texture2D iconTexture)
        {
            spriteBatch.Draw(iconTexture, new Vector2(iconX, _screenY), Color.White);
        }

        private void DrawIconBackground(SpriteBatch spriteBatch, int iconX, Texture2D backgroundTexture)
        {
            spriteBatch.Draw(backgroundTexture, new Vector2(iconX, _screenY), Color.White);
        }

        private static string GetConditionTitle(IPersonCondition personCondition)
        {
            switch (personCondition)
            {
                case SurvivalStatHazardCondition statEffect:
                    return GetSurvivalConditionTitle(statEffect);

                case DiseaseSymptomCondition diseaseSymptomCondition:
                    return GetSymptomTitle(diseaseSymptomCondition);

                default:
                    Debug.Fail(
                        $"All person conditions must have localized titles. Unknown person effect: {personCondition}.");
                    return string.Empty;
            }
        }

        private static string? GetSymptomTitle(DiseaseSymptomCondition diseaseSymptomCondition)
        {
            var diseasesTitles = diseaseSymptomCondition.Diseases.Select(x=>GetName(x));
            var fullDeseasesList = string.Join(",", diseasesTitles);
            return $"{diseaseSymptomCondition.Symptom.Name?.Ru ?? diseaseSymptomCondition.Symptom.Name?.En} ({fullDeseasesList})";
        }

        private static string GetName(IDisease disease)
        {
            var name =
                        $"{disease.Name.Secondary?.Ru} {disease.Name.PrimaryPrefix?.Ru}{disease.Name.Primary?.Ru} {disease.Name.Subject?.Ru}";
            return name;
        }

        private static string GetStatHazardConditionLevelClientString(SurvivalStatHazardCondition statCondition)
        {
            return statCondition.Level == SurvivalStatHazardLevel.Max
                ? "Critical"
                : statCondition.Level.ToString();
        }

        private static string GetStatHazardConditionTypeClientString(SurvivalStatType type)
        {
            switch (type)
            {
                case SurvivalStatType.Health:
                    return "Wound";

                case SurvivalStatType.Satiety:
                    return "Hunger";

                case SurvivalStatType.Hydration:
                    return "Thrist";

                case SurvivalStatType.Intoxication:
                    return "Intoxication";

                case SurvivalStatType.Energy:
                    return "Energy";

                case SurvivalStatType.Undefined:
                    Debug.Fail("Undefined condition.");
                    return "Empty";

                default:
                    Debug.Fail($"Unknown condition type {type}.");
                    return "Empty";
            }
        }

        private static string GetSurvivalConditionTitle(SurvivalStatHazardCondition statCondition)
        {
            var typeString = GetStatHazardConditionTypeClientString(statCondition.Type);
            var levelString = GetStatHazardConditionLevelClientString(statCondition);
            var conditionTitle = UiResources.ResourceManager.GetString($"{typeString}{levelString}ConditionTitle");
            if (string.IsNullOrWhiteSpace(conditionTitle))
            {
                Debug.Fail("All person conditions must have localized titles."
                           + $" Unknown person effect: {statCondition.Type} {statCondition.Level}.");
                return $"{typeString}{levelString}";
            }

            return conditionTitle;
        }

        private void UpdateAlert(IConditionsModule conditionsModule, GameTime gameTime)
        {
            var conditionRectangles = conditionsModule.Items.Select((x, index) => new
            {
                UiRect = new Rectangle(index * (ICON_SIZE + ICON_SPACING) - ICON_SPACING + _screenX, _screenY,
                    ICON_SIZE, ICON_SIZE),
                Condition = x,
                IconIndex = index
            });

            var criticalConditions = conditionRectangles
                .Where(x => x.Condition is SurvivalStatHazardCondition survivalStatHazardCondition &&
                            survivalStatHazardCondition.Level == SurvivalStatHazardLevel.Max);

            _alertedConditions.Clear();

            if (_alertCounter < ALERT_DELAY_DURATION_SECONDS + ALERT_VISIBLE_DURATION_SECONDS)
            {
                _alertCounter += gameTime.ElapsedGameTime.TotalSeconds;

                if (_alertCounter < ALERT_VISIBLE_DURATION_SECONDS)
                {
                    var t = _alertCounter / ALERT_VISIBLE_DURATION_SECONDS;
                    var visiblilitySin = Math.Sin(t * Math.PI * 2 * 3);
                    if (visiblilitySin > 0)
                    {
                        foreach (var criticalCondition in criticalConditions)
                        {
                            _alertedConditions.Add(criticalCondition.Condition);
                        }
                    }

                    if (visiblilitySin > 0 && _alertedConditions.Any() && _alertSoundEffect.State != SoundState.Playing)
                    {
                        _alertSoundEffect.Play();
                    }
                }
            }
            else
            {
                _alertCounter = 0;
            }

            if (criticalConditions.Any())
            {
                _soundtrackManager.PlaySilence();
            }
            else
            {
                _soundtrackManager.PlayBackgroundTrack();
            }
        }
    }
}