namespace Zilon.Core.Schemes
{
    public interface IPersonScheme: IScheme
    {
        string DefaultAct { get; set; }
        int Hp { get; set; }
        PersonSlotSubScheme[] Slots { get; set; }
    }
}