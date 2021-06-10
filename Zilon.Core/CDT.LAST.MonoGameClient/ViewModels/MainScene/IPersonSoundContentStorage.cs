using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using Zilon.Core.Persons;

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

        //TODO Docs
        SoundEffect? GetActivitySound(PersonActivityEffectType personActivityType);

        /// <summary>
        /// The sound played when visualization of a act starts.
        /// </summary>
        SoundEffect GetActStartSound(ActDescription actDescription);

        /// <summary>
        /// The sound played when visualization of a consuming things from inventory starts.
        /// </summary>
        SoundEffect? GetConsumePropSound(ConsumeEffectType consumeEffectType);

        /// <summary>
        /// The sound of specified person death.
        /// </summary>
        SoundEffect GetDeathEffect(IPerson person);

        /// <summary>
        /// The sound played when visualization of some items are weared by a person.
        /// </summary>
        SoundEffect? GetEquipSound(string[] tags, bool direction);

        SoundEffect GetImpactEffect(IPerson person);

        void LoadContent(ContentManager contentManager);
    }
}