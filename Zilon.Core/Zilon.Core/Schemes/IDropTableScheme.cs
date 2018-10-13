namespace Zilon.Core.Schemes
{
    public interface IDropTableScheme: IScheme
    {
        DropTableRecordSubScheme[] Records { get; }
        int Rolls { get; }
    }
}