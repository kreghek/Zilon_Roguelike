using JetBrains.Annotations;

namespace Zilon.Core.Schemes
{
    public interface IPersonScheme: IScheme
    {
        [NotNull]
        string DefaultAct { get; set; }
        
        int Hp { get; set; }
        
        [NotNull, ItemNotNull]
        PersonSlotSubScheme[] Slots { get; set; }
    }
}