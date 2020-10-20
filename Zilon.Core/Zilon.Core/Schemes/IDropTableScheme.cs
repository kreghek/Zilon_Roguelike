namespace Zilon.Core.Schemes
{
    public interface IDropTableScheme : IScheme
    {
        IDropTableRecordSubScheme[] Records { get; }
        int Rolls { get; }
    }
}