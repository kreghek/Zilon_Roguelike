namespace Zilon.Core.Schemes
{
    public interface ILocationScheme: IScheme
    {
        string MapSid { get; set; }
        float X { get; set; }
        float Y { get; set; }
    }
}