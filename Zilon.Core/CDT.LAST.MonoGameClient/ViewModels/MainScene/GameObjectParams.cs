using CDT.LAST.MonoGameClient.Screens;
using CDT.LAST.MonoGameClient.ViewModels.MainScene.GameObjectVisualization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Zilon.Core.Client;
using Zilon.Core.Players;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    internal record GameObjectParams
    {
        public Camera? Camera { get; init; }
        public Game? Game { get; init; }
        public IGameObjectVisualizationContentStorage? GameObjectVisualizationContentStorage { get; init; }
        public IPersonSoundContentStorage? PersonSoundStorage { get; init; }
        public IPersonVisualizationContentStorage? PersonVisualizationContentStorage { get; init; }
        public IPlayer? Player { get; init; }
        public SectorViewModelContext? SectorViewModelContext { get; init; }
        public SpriteBatch? SpriteBatch { get; init; }
        public ISectorUiState? UiState { get; init; }
    }
}