using System;
using System.Linq;

using UnityEngine;
using UnityEngine.EventSystems;

using Zilon.Core.Client;
using Zilon.Core.Common;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics.Spatial;

public class MapNodeVM : MonoBehaviour, IMapNodeViewModel
{
    public SpriteRenderer FloorSpriteRenderer;
    public SpriteRenderer FloorDecorRenderer;
    public SpriteRenderer InteriorObjectSpriteRenderer;
    public GameObject[] Walls;
    public GameObject[] NextNodeMarkers;
    public SpriteRenderer LeftBottomRenderer;
    public SpriteRenderer RightBottomRenderer;
    public bool IsExit;
    public GameObject ExitMarker;
    
    public HexNode Node { get; set; }
    public HexNode[] Neighbors { get; set; }

    public ILocationScheme LocaltionScheme { get; set; }

    public event EventHandler OnSelect;
    public event EventHandler MouseEnter;


    public void Start()
    {
        var neighborCubePositions = HexHelper.GetOffsetClockwise();

        var walls = GetWalls(LocaltionScheme.Sid);

        for (var i = 0; i < 6; i++)
        {
            var wallObj = Walls[i];
            var cubeOffsetPosition = neighborCubePositions[i];

            var checkedCubeCoords = Node.CubeCoords + cubeOffsetPosition;

            var neighbor = Neighbors.SingleOrDefault(x => x.CubeCoords == checkedCubeCoords);

            var hasWall = neighbor == null;

            wallObj.SetActive(hasWall);

            var wallSprite = walls.Walls[i];

            if (wallSprite != null)
            {
                wallObj.GetComponent<SpriteRenderer>().sprite = walls.Walls[i];
            }
            else
            {
                Destroy(wallObj.GetComponent<SpriteRenderer>());
            }

            // Скрываем маркеры неоткрытого следующего узла,
            // если есть стена.
            NextNodeMarkers[i].SetActive(!hasWall);
        }

        LeftBottomRenderer.color = walls.LeftBottomColor;
        RightBottomRenderer.color = walls.RightBottomColor;

        FloorSpriteRenderer.sprite = walls.Floor;

        ExitMarker.SetActive(IsExit);

        if (Node.IsObstacle)
        {
            var selectedInteriorObjectSpriteIndex = UnityEngine.Random.Range(0, walls.InteriorObjectSprites.Length);
            var selectedInteriorObjectSprite = walls.InteriorObjectSprites[selectedInteriorObjectSpriteIndex];
            InteriorObjectSpriteRenderer.sprite = selectedInteriorObjectSprite;
        }

        var hasFloorDecor = UnityEngine.Random.Range(0, 100) > 90;
        if (hasFloorDecor)
        {
            var decorIndex = UnityEngine.Random.Range(0, walls.FloorDecorSprites.Length);
            FloorDecorRenderer.sprite = walls.FloorDecorSprites[decorIndex];
        }

        transform.position = new Vector3(transform.position.x, transform.position.y, Node.OffsetY);
    }


    //TODO Указывать тип пола в схеме сектора.
    private static SectorWalls GetWalls(string locationSid)
    {
        var wallsPath = "Sector/Walls";
        string wallsSid;

        switch (locationSid)
        {
            case "rat-hole":
            case "rat-kingdom":
            case "genomass-cave":
            case "intro":
            default:
                wallsSid = "Cave";
                break;

            case "city":
                wallsSid = "City";
                break;

            case "forest":
                wallsSid = "Forest";
                break;

            case "crypt":
                wallsSid = "Crypt";
                break;

            case "demon-dungeon":
            case "demon-lair":
                wallsSid = "Demon";
                break;

            case "elder-place":
                wallsSid = "Elder";
                break;
        }

        return Resources.Load<SectorWalls>($"{wallsPath}/{wallsSid}");
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