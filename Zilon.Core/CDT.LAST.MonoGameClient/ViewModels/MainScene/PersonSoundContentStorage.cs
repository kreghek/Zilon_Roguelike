using System;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using Zilon.Core.Common;
using Zilon.Core.Persons;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    /// <summary>
    /// Base implementation of the sound storage of person.
    /// </summary>
    internal sealed class PersonSoundContentStorage : IPersonSoundContentStorage
    {
        private Dictionary<PersonActivityEffectType, SoundEffect>? _activityDict;
        private IDictionary<string, SoundEffect>? _actStartDict;
        private IDictionary<ConsumeEffectType, SoundEffect>? _consumableDict;
        private Dictionary<string, SoundEffect>? _deathDict;
        private SoundEffect? _defaultStartHitEffect;

        private IDictionary<string, SoundEffect>? _equipDict;
        private Dictionary<string, SoundEffect>? _impactDict;
        private SoundEffect? _swordHitEffect;
        private SoundEffect? _unequipSound;

        /// <inheritdoc />
        public SoundEffect GetActHitSound(ActDescription actDescription, IPerson targetPerson)
        {
            return _swordHitEffect ?? throw new InvalidOperationException("All content must be loaded early.");
        }

        /// <inheritdoc />
        public SoundEffect? GetActivitySound(PersonActivityEffectType personActivityType)
        {
            if (_activityDict is null)
            {
                throw new InvalidOperationException(
                    "Dictionary of person activity sound effects must be initialized in storage loading.");
            }

            if (_activityDict.TryGetValue(personActivityType, out var soundEffect))
            {
                return soundEffect;
            }

            Debug.Fail("All acts must have audio effect.");
            // Return default audio if act is unknown.
            return null;
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
            if (_deathDict is null)
            {
                throw new InvalidOperationException(
                    "Dictionary of death sound effect must be initialized in storage loading.");
            }

            var key = "monster";
            if (person is HumanPerson)
            {
                key = "human";
            }

            if (_deathDict.TryGetValue(key, out var soundEffect))
            {
                return soundEffect;
            }

            Debug.Fail("All person must have sound effects.");

            return _deathDict["human"];
        }

        /// <inheritdoc />
        public SoundEffect GetImpactEffect(IPerson person)
        {
            if (_impactDict is null)
            {
                throw new InvalidOperationException(
                    "Dictionary of death sound effect must be initialized in storage loading.");
            }

            var key = "monster";
            if (person is HumanPerson)
            {
                key = "human";
            }

            if (_impactDict.TryGetValue(key, out var soundEffect))
            {
                return soundEffect;
            }

            Debug.Fail("All person must have sound effects.");

            return _impactDict["human"];
        }

        public SoundEffect? GetEquipSound(string[] tags, bool direction)
        {
            if (_equipDict is null)
            {
                throw new InvalidOperationException(
                    "Dictionary of equip sound effect must be initialized in storage loading.");
            }

            if (direction)
            {
                foreach (var tag in tags)
                {
                    if (_equipDict.TryGetValue(tag, out var soundEffect))
                    {
                        return soundEffect;
                    }
                }

                Debug.Fail("All types of equipment must have audio effect.");
                // Return default audio if act is unknown.
                return null;
            }

            Debug.Assert(_unequipSound != null);
            return _unequipSound;
        }

        /// <inheritdoc />
        public void LoadContent(ContentManager contentManager)
        {
            _deathDict = new Dictionary<string, SoundEffect>
            {
                ["human"] = contentManager.Load<SoundEffect>("Audio/HumanDeath"),
                ["monster"] = contentManager.Load<SoundEffect>("Audio/HunterDeath")
            };

            _impactDict = new Dictionary<string, SoundEffect>
            {
                ["human"] = contentManager.Load<SoundEffect>("Audio/HumanImpact"),
                ["monster"] = contentManager.Load<SoundEffect>("Audio/HunterImpact")
            };

            _swordHitEffect = contentManager.Load<SoundEffect>("Audio/SwordHitEffect");

            _actStartDict = new Dictionary<string, SoundEffect>
            {
                ["bite"] = contentManager.Load<SoundEffect>("Audio/HunterHitEffect"),
                ["punch"] = contentManager.Load<SoundEffect>("Audio/PunchStartHitEffect"),
                ["slash"] = contentManager.Load<SoundEffect>("Audio/SwordStartHitEffect"),
                ["pierce"] = contentManager.Load<SoundEffect>("Audio/SpearPierceEffect"),
                ["swing"] = contentManager.Load<SoundEffect>("Audio/CombatStaffSwing")
            };
            _defaultStartHitEffect = _actStartDict["punch"];

            _consumableDict = new Dictionary<ConsumeEffectType, SoundEffect>
            {
                [ConsumeEffectType.UseCommon] = contentManager.Load<SoundEffect>("Audio/UseConsumable"),
                [ConsumeEffectType.Eat] = contentManager.Load<SoundEffect>("Audio/EatConsumable"),
                [ConsumeEffectType.Drink] = contentManager.Load<SoundEffect>("Audio/DrinkConsumable"),
                [ConsumeEffectType.Heal] = contentManager.Load<SoundEffect>("Audio/HealConsumable")
            };

            _equipDict = new Dictionary<string, SoundEffect>
            {
                [PropTags.Equipment.Weapon] = contentManager.Load<SoundEffect>("Audio/EquipWeapon"),
                [PropTags.Equipment.Shield] = contentManager.Load<SoundEffect>("Audio/EquipWeapon"),
                [PropTags.Equipment.Ranged] = contentManager.Load<SoundEffect>("Audio/EquipRifle"),
                [PropTags.Equipment.Armor] = contentManager.Load<SoundEffect>("Audio/EquipClothes")
            };

            _activityDict = new Dictionary<PersonActivityEffectType, SoundEffect>
            {
                [PersonActivityEffectType.Move] = contentManager.Load<SoundEffect>("Audio/Step"),
                [PersonActivityEffectType.Transit] = contentManager.Load<SoundEffect>("Audio/Transition"),
                [PersonActivityEffectType.TransferProp] = contentManager.Load<SoundEffect>("Audio/TransferProp")
            };

            _unequipSound = contentManager.Load<SoundEffect>("Audio/Unequip");
        }
    }
}