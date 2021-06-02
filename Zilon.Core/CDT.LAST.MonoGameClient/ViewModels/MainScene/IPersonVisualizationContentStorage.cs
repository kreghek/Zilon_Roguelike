using System.Collections.Generic;

using Microsoft.Xna.Framework.Content;

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
        IEnumerable<BodyPart> GetHumanParts();
        void LoadContent(ContentManager content);
    }
}