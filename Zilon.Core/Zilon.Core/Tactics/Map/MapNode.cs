namespace Zilon.Core.Tactics.Map
{
    using Zilon.Core.Math;

    public class MapNode
    {
        public int Id { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Coordinates { get; set; }
        
        public Vector3 GetCubeCoords()
        {
            var x = Coordinates.X - (Coordinates.Y - (Coordinates.Y % 2)) / 2;
            var z = Coordinates.Y;
            var y = -x - z;
            
            return new Vector3(x, y, z);
        }
    }
}
