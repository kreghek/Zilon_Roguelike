using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

using Zilon.Core.Players;
using Zilon.Core.World;

using Color = UnityEngine.Color;

/// <summary>
/// Скрипт для работы с миникартой мира.
/// </summary>
public class GlobeMinimap : MonoBehaviour
{
    private const int CELL_SIZE = 8;

    public Image MinimapContent;

    [Inject] private IGlobeManager _worldManager;
    [Inject] private HumanPlayer _humanPlayer;

    private Texture2D _realmTexture;
    private Texture2D _branchesTexture;

    public void Start()
    {
        if (_realmTexture == null || _branchesTexture == null)
        {
            InitMapTextures();
        }
    }

    /// <summary>
    /// Инициализация текстур, содержащих карту. Формированно вызывается из клиентского кода
    /// когда объект Globe создан.
    /// </summary>
    /// <remarks>
    /// Это нужно, потому что сначала создаётся объект миникарты в момент создания модели глобальной карты,
    /// а только потом модель генерирует мир. После создания мира ещё раз вызывается этот метод.
    /// </remarks>
    public void InitMapTextures()
    {
        var globe = _worldManager.Globe;
        var terrain = globe.Terrain;

        var globeWidth = terrain.Cells.Length;
        var globeHeight = terrain.Cells[0].Length;

        var branchColors = new[] { Color.red, Color.blue, Color.green, Color.yellow,
                Color.black, Color.magenta, Color.cyan, Color.gray };

        _realmTexture = new Texture2D(globeWidth * CELL_SIZE, globeHeight * CELL_SIZE, TextureFormat.ARGB32, false);
        _branchesTexture = new Texture2D(globeWidth * CELL_SIZE, globeHeight * CELL_SIZE, TextureFormat.ARGB32, false);

        for (int y = 0; y < globeHeight; y++)
        {
            for (int x = 0; x < globeWidth; x++)
            {
                var cell = terrain.Cells[x][y];
                //if (globe.LocalitiesCells.TryGetValue(cell, out var locality))
                //{
                //    var branch = locality.Branches.Single(b => b.Value > 0);
                //    var owner = locality.Owner;

                //    var mainRealmColor = owner.Banner.MainColor;
                //    var realmColor = new Color(
                //        mainRealmColor.R / 255f,
                //        mainRealmColor.G / 255f,
                //        mainRealmColor.B / 255f);

                //    DrawBlock(_realmTexture, x, y, realmColor);
                //    DrawBlock(_branchesTexture, x, y, branchColors[(int)branch.Key]);
                //}
                //else
                //{
                //    ClearBlock(_realmTexture, x, y);
                //}
            }
        }

        if (_humanPlayer != null)
        {
            var playerCell = _humanPlayer.Terrain;
            var playerCellCoords = playerCell.Coords;
            DrawPlayer(_realmTexture, playerCellCoords.X, playerCellCoords.Y);
        }

        _realmTexture.filterMode = FilterMode.Trilinear;
        _realmTexture.Apply();

        _branchesTexture.filterMode = FilterMode.Trilinear;
        _branchesTexture.Apply();
    }

    /// <summary>
    /// Включает отображение политической карты. Это используется по умолчанию.
    /// </summary>
    public void ShowRealms()
    {
        var miniMap = Sprite.Create(_realmTexture, new Rect(0, 0, _realmTexture.width, _realmTexture.height), new Vector2(0, 0));
        MinimapContent.sprite = miniMap;
    }

    /// <summary>
    /// Включает отображение карты специализаций.
    /// </summary>
    public void ShowBranches()
    {
        var miniMap = Sprite.Create(_branchesTexture, new Rect(0, 0, _realmTexture.width, _realmTexture.height), new Vector2(0, 0));
        MinimapContent.sprite = miniMap;
    }

    private void DrawBlock(Texture2D texture, int x, int y, Color color)
    {
        for (var i = 0; i < CELL_SIZE; i++)
        {
            for (var j = 0; j < CELL_SIZE; j++)
            {
                texture.SetPixel(x * CELL_SIZE + i, y * CELL_SIZE + j, color);
            }
        }
    }

    private void ClearBlock(Texture2D texture, int x, int y)
    {
        for (var i = 0; i < CELL_SIZE; i++)
        {
            for (var j = 0; j < CELL_SIZE; j++)
            {
                texture.SetPixel(x * CELL_SIZE + i, y * CELL_SIZE + j, new Color(0, 0, 0, 0));
            }
        }
    }

    private void DrawPlayer(Texture2D texture, int x, int y)
    {
        for (var i = 0; i < CELL_SIZE; i++)
        {
            for (var j = 0; j < CELL_SIZE; j++)
            {
                var color = Color.green;
                if ((i + j * CELL_SIZE) % 2 == 0)
                {
                    color = Color.gray;
                }
                texture.SetPixel(x * CELL_SIZE + i, y * CELL_SIZE + j, color);
            }
        }
    }
}
