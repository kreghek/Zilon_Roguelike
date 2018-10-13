namespace Zilon.Core.Schemes
{
    public interface IDropTableRecordSubScheme: ISubScheme
    {
        string Concept { get; set; }
        int MaxCount { get; set; }
        int MaxPower { get; set; }
        int MinCount { get; set; }
        int MinPower { get; set; }
        string SchemeSid { get; }
        int Weight { get; }
    }
}