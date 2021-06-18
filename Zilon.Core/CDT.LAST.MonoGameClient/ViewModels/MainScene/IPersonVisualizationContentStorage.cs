using System.Collections.Generic;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    /// <summary>
    /// Storage of content to persons visualization.
    /// Responsible for humanoids, monsters and other persons.
    /// </summary>
    public interface IPersonVisualizationContentStorage
    {
        IEnumerable<BodyPart> GetBodyParts(string sid);
        IEnumerable<HandPart> GetHandParts(string sid);
        IEnumerable<HeadPart> GetHeadParts(string sid);
        IEnumerable<BodyPart> GetHumanParts();
        void LoadContent(ContentManager content);
    }

    public interface IGameObjectVisualizationContentStorage
    {
        Texture2D GetConsumingEffectTexture();
        void LoadContent(ContentManager content);
    }

    public sealed class GameObjectVisualizationContentStorage : IGameObjectVisualizationContentStorage
    {
        private Texture2D? _cunsumingEffectTexture;

        public Texture2D GetConsumingEffectTexture()
        {
            return _cunsumingEffectTexture;
        }

        public void LoadContent(ContentManager content)
        {
            _cunsumingEffectTexture = content.Load<Texture2D>("Sprites/effects/ConsumingEffects");
        }
    }
}