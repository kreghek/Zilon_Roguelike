using System;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using Zilon.Core.Persons;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    /// <summary>
    /// Base implementation of the sound storage of person.
    /// </summary>
    internal sealed class PersonSoundContentStorage : IPersonSoundContentStorage
    {
        private IDictionary<string, SoundEffect>? _actStartDict;
        private IDictionary<ConsumeEffectType, SoundEffect>? _consumableDict;
        private SoundEffect? _defaultStartHitEffect;
        private SoundEffect? _hunterDeathEffect;
        private SoundEffect? _swordHitEffect;

        public SoundEffect GetActHitSound(ActDescription actDescription, IPerson targetPerson)
        {
            return _swordHitEffect ?? throw new InvalidOperationException("All content must be loaded early.");
        }

        /// <inheritdoc />
        public SoundEffect GetActStartSound(ActDescription actDescription)
        {
            if (_actStartDict is null)
            {
                throw new InvalidOperationException(
                    "Dictionary of act sound effect must be initialized in storage loading.");
            }

            foreach (var tag in actDescription.Tags)
            {
                if (_actStartDict.TryGetValue(tag, out var soundEffect))
                {
                    return soundEffect;
                }
            }

            Debug.Fail("All acts must have audio effect.");
            // Return default audio if act is unknown.
            return _defaultStartHitEffect ?? throw new InvalidOperationException("All content must be loaded early.");
        }

        /// <inheritdoc />
        public SoundEffect? GetConsumePropSound(ConsumeEffectType consumeEffectType)
        {
            if (_consumableDict is null)
            {
                throw new InvalidOperationException(
                    "Dictionary of consumable sound effect must be initialized in storage loading.");
            }

            if (_consumableDict.TryGetValue(consumeEffectType, out var soundEffect))
            {
                return soundEffect;
            }

            return null;
        }

        /// <inheritdoc />
        public SoundEffect GetDeathEffect(IPerson person)
        {
            return _hunterDeathEffect ?? throw new InvalidOperationException("All content must be loaded early.");
        }

        /// <inheritdoc />
        public void LoadContent(ContentManager contentManager)
        {
            _hunterDeathEffect = contentManager.Load<SoundEffect>("Audio/HunterDeath");
            _swordHitEffect = contentManager.Load<SoundEffect>("Audio/SwordHitEffect");

            _actStartDict = new Dictionary<string, SoundEffect>
            {
                ["bite"] = contentManager.Load<SoundEffect>("Audio/HunterHitEffect"),
                ["punch"] = contentManager.Load<SoundEffect>("Audio/PunchStartHitEffect"),
                ["slash"] = contentManager.Load<SoundEffect>("Audio/SwordStartHitEffect")
            };
            _defaultStartHitEffect = _actStartDict["punch"];

            _consumableDict = new Dictionary<ConsumeEffectType, SoundEffect>
            {
                [ConsumeEffectType.Use] = contentManager.Load<SoundEffect>("Audio/UseConsumable"),
                [ConsumeEffectType.Eat] = contentManager.Load<SoundEffect>("Audio/EatConsumable"),
                [ConsumeEffectType.Drink] = contentManager.Load<SoundEffect>("Audio/DrinkConsumable"),
                [ConsumeEffectType.Heal] = contentManager.Load<SoundEffect>("Audio/HealConsumable")
            };
        }
    }
}