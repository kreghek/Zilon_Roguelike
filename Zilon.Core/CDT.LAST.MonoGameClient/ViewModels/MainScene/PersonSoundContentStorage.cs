using System;
using System.Diagnostics;
using System.Linq;

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using Zilon.Core.Persons;
using Zilon.Core.Tactics.ActorInteractionEvents;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    /// <summary>
    /// Base implementation of the sound storage of person.
    /// </summary>
    internal sealed class PersonSoundContentStorage : IPersonSoundContentStorage
    {
        private SoundEffect? _hunterDeathEffect;
        private SoundEffect? _hunterStartHitEffect;
        private SoundEffect? _punchStartHitEffect;
        private SoundEffect? _swordHitEffect;
        private SoundEffect? _swordStartHitEffect;

        public SoundEffect GetActHitSound(ActDescription actDescription, IPerson targetPerson)
        {
            return _swordHitEffect;
        }

        /// <inheritdoc />
        public SoundEffect GetActStartSound(ActDescription actDescription)
        {
            if (actDescription.Tags.Contains("slash"))
            {
                return _swordStartHitEffect;
            }

            if (actDescription.Tags.Contains("bite"))
            {
                return _hunterStartHitEffect;
            }

            if (actDescription.Tags.Contains("punch"))
            {
                return _punchStartHitEffect;
            }

            Debug.Fail("All acts must have audio effect.");
            // Return default audio if act is unknown.
            return _punchStartHitEffect;
        }

        /// <inheritdoc />
        public SoundEffect GetDeathEffect(IPerson person)
        {
            return _hunterDeathEffect;
        }

        /// <inheritdoc />
        public void LoadContent(ContentManager contentManager)
        {
            _swordStartHitEffect = contentManager.Load<SoundEffect>("Audio/SwordStartHitEffect");
            _hunterDeathEffect = contentManager.Load<SoundEffect>("Audio/HunterDeath");
            _swordHitEffect = contentManager.Load<SoundEffect>("Audio/SwordHitEffect");
            _hunterStartHitEffect = contentManager.Load<SoundEffect>("Audio/HunterHitEffect");
            _punchStartHitEffect = contentManager.Load<SoundEffect>("Audio/PunchStartHitEffect");
        }
    }
}