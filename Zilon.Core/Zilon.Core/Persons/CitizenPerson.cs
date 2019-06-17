using System;
using System.Linq;

using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Персонаж для торговцев в секторе.
    /// </summary>
    public class CitizenPerson : IPerson
    {
        private readonly IDropTableScheme _goodsDropTable;
        private readonly IDropResolver _dropResolver;

        public int Id { get; set; }
        public IEquipmentCarrier EquipmentCarrier => null;

        public ITacticalActCarrier TacticalActCarrier { get; }

        public IEvolutionData EvolutionData => throw new NotSupportedException("Для монстров не поддерживается развитие");

        public ICombatStats CombatStats { get; }

        public IPropStore Inventory => throw new NotSupportedException("Для монстров не поддерживается инвентарь.");

        public ISurvivalData Survival { get; }

        public EffectCollection Effects { get; }

        public IMonsterScheme Scheme { get; }

        public CitizenPerson(IDropTableScheme goodsDropTable, IDropResolver dropResolver)
        {
            _goodsDropTable = goodsDropTable;
            _dropResolver = dropResolver;
        }

        /// <summary>
        /// Сделка с торговцем.
        /// </summary>
        /// <returns>
        /// Возвращает товар, который отдаёт торговец.
        /// </returns>
        public IProp Offer()
        {
            var goods = _dropResolver.Resolve(new[] { _goodsDropTable });
            return goods.Single();
        }

        public override string ToString()
        {
            return $"{Scheme?.Name?.En}";
        }
    }
}