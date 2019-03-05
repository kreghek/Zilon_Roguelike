namespace Zilon.Core.Props
{
    public interface IEquipmentDurableServiceRandomSource
    {
        int RollTurnResist(Equipment equipment);

        int RollUseResist(Equipment equipment);
    }
}
