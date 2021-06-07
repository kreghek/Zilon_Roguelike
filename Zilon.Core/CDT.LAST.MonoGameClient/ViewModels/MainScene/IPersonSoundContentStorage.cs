using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using Zilon.Core.Persons;
using Zilon.Core.Tactics.ActorInteractionEvents;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    /// <summary>
    /// Storage of sounds used to voice persons in the game.
    /// </summary>
    internal interface IPersonSoundContentStorage
    {
        /// <summary>
        /// The sound played when person hit a target successfuly.
        /// </summary>
        SoundEffect GetActHitSound(ActDescription actDescription, IPerson targetPerson);

        /// <summary>
        /// The sound played when visualization of a act starts.
        /// </summary>
        SoundEffect GetActStartSound(ActDescription actDescription);

        /// <summary>
        /// The sound of specified person death.
        /// </summary>
        SoundEffect GetDeathEffect(IPerson person);

        void LoadContent(ContentManager contentManager);
    }
}