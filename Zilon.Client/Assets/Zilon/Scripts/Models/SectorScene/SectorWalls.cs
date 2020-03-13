using UnityEngine;

public class SectorWalls : MonoBehaviour
{
    public Sprite LeftTopWall;
    public Sprite LeftWall;
    public Sprite LeftBottomWall;

    public Sprite RightTopWall;
    public Sprite RightWall;
    public Sprite RightBottomWall;

    public Color LeftBottomColor;
    public Color RightBottomColor;

    public Sprite Floor;

    public Sprite[] InteriorObjectSprites;
    public Sprite[] FloorDecorSprites;

    public Sprite[] Walls
    {
        get => new[] { LeftWall, LeftTopWall, RightTopWall, RightWall, RightBottomWall, LeftBottomWall };
    }
}
