using System.Collections.Generic;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.GameObjectVisualization
{
    /// <summary>
    /// Storage of content to persons visualization.
    /// Responsible for humanoids, monsters and other persons.
    /// </summary>
    public interface IPersonVisualizationContentStorage
    {
        IEnumerable<AnimalPart> GetAnimalParts(string sid);
        IEnumerable<BodyPart> GetBodyParts(string sid);
        IEnumerable<HandPart> GetHandParts(string sid);
        IEnumerable<HeadPart> GetHeadParts(string sid);

        IEnumerable<BodyPart> GetHumanOutlinedParts();
        IEnumerable<BodyPart> GetHumanParts();

        Texture2D GetMonographicTexture(string sid);
        void LoadContent(ContentManager content);
    }
}