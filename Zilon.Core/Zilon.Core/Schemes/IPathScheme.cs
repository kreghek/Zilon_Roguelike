namespace Zilon.Core.Schemes
{
    public interface IPathScheme: IScheme
    {
        string MapSid { get; set; }
        string Sid1 { get; set; }
        string Sid2 { get; set; }
    }
}