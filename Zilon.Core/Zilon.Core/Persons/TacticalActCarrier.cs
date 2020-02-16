using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Базовая реализация объекта для хранения сведений о тактических действиях персонажа.
    /// </summary>
    public class TacticalActCarrier : ITacticalActCarrier
    {
        private readonly IDice _dice;

        public TacticalActCarrier(IDice dice)
        {
            _dice = dice;
        }

        public ITacticalAct[] Acts { get; set; }

        public void UpdateActs()
        {
            foreach(var act in Acts)
            {
                act.IsAvailable = _dice.RollD6() >= 4;
            }
        }
    }
}
