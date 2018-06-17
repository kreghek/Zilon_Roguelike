using System;
using System.Linq;
using UnityEngine;
using Zilon.Core;
using Zilon.Core.Tactics.Spatial;

public class MapNodeVM : MonoBehaviour
{
    public GameObject[] Walls;
    
    public HexNode Node { get; set; }
    public HexNode[] Neighbors { get; set; }
    public IEdge[] Edges { get; set; }

    public event EventHandler OnSelect;


    public void Start()
    {
        var neighborCubePositions = new[]
        {
            new CubeCoords(-1, +1, 0), new CubeCoords(-1, 0, +1), new CubeCoords(0, -1, +1),
            new CubeCoords(+1, -1, 0),new CubeCoords(+1, 0, -1),new CubeCoords(0, +1, -1)
        };

        for (var i=0; i < 6; i++)
        {
            var wallObj = Walls[i];
            var cubeOffsetPosition = neighborCubePositions[i];
 
            var checkedCubeCoords = Node.CubeCoords + cubeOffsetPosition;

            var neighbor = Neighbors.SingleOrDefault(x => x.CubeCoords == checkedCubeCoords);

            var hasWall = neighbor == null;
            
            wallObj.SetActive(hasWall);
        }
        
        
    }

    public void OnMouseDown()
    {
        OnSelect?.Invoke(this, new EventArgs());
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