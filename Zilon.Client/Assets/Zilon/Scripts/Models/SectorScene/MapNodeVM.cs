using System;
using System.Linq;

using UnityEngine;
using UnityEngine.EventSystems;

using Zilon.Core.Client;
using Zilon.Core.Common;
using Zilon.Core.Tactics.Spatial;

public class MapNodeVM : MonoBehaviour, IMapNodeViewModel
{
    public SpriteRenderer FloorSpriteRenderer;
    public SpriteRenderer FloorDecorRenderer;
    public SpriteRenderer InteriorObjectSpriteRenderer;
    public Sprite[] InteriorObjectSprites;
    public Sprite[] FloorDecorSprites;
    public GameObject[] Walls;
    public bool IsExit;
    public GameObject ExitMarker;
    
    public HexNode Node { get; set; }
    public HexNode[] Neighbors { get; set; }

    public event EventHandler OnSelect;
    public event EventHandler MouseEnter;


    public void Start()
    {
        var neighborCubePositions = HexHelper.GetOffsetClockwise();

        for (var i = 0; i < 6; i++)
        {
            var wallObj = Walls[i];
            var cubeOffsetPosition = neighborCubePositions[i];

            var checkedCubeCoords = Node.CubeCoords + cubeOffsetPosition;

            var neighbor = Neighbors.SingleOrDefault(x => x.CubeCoords == checkedCubeCoords);

            var hasWall = neighbor == null;

            wallObj.SetActive(hasWall);
        }

        ExitMarker.SetActive(IsExit);

        if (Node.IsObstacle)
        {
            var selectedInteriorObjectSpriteIndex = UnityEngine.Random.Range(0, InteriorObjectSprites.Length);
            var selectedInteriorObjectSprite = InteriorObjectSprites[selectedInteriorObjectSpriteIndex];
            InteriorObjectSpriteRenderer.sprite = selectedInteriorObjectSprite;
        }

        var hasFloorDecor = UnityEngine.Random.Range(0, 100) > 90;
        if (hasFloorDecor)
        {
            var decorIndex = UnityEngine.Random.Range(0, FloorDecorSprites.Length);
            FloorDecorRenderer.sprite = FloorDecorSprites[decorIndex];
        }
    }
    
    public void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        OnSelect?.Invoke(this, new EventArgs());
    }

    public void OnMouseEnter()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        MouseEnter?.Invoke(this, new EventArgs());
    }

    public override string ToString()
    {
        if (Node == null)
        {
            return string.Empty;
        }

        return $"Id: {Node.Id} Position: ({Node.OffsetX}, {Node.OffsetY})";
    }
}