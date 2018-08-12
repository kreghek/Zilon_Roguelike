namespace Zilon.Core.Persons
{
    /// <summary>
    /// Базовая реализация объекта для хранения сведений о тактических действиях персонажа.
    /// </summary>
    public class TacticalActCarrier : ITacticalActCarrier
    {
        public ITacticalAct[] Acts { get; set; }
    }
}
