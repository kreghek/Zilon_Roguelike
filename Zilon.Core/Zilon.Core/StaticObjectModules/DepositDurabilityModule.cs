namespace Zilon.Core.StaticObjectModules
{
    public sealed class DepositDurabilityModule : IDurabilityModule
    {
        private readonly int _damagePerMineUnit;
        private readonly ILifetimeModule _lifetimeModule;
        private readonly IPropDepositModule _propDepositModule;
        private int _mineDamageCounter;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="propDepositModule"> Prop Deposit module from static object. </param>
        /// <param name="lifetimeModule"> Lifetime module from static object. </param>
        /// <param name="damagePerMineUnit">
        ///     Required damage to mine one time. Example, if this value is 10 and actor hit multiple
        ///     times sum on 10, this be like one mine action.
        /// </param>
        public DepositDurabilityModule(IPropDepositModule propDepositModule, ILifetimeModule lifetimeModule,
            int damagePerMineUnit)
        {
            IsActive = true;
            _propDepositModule = propDepositModule;
            _lifetimeModule = lifetimeModule;
            _damagePerMineUnit = damagePerMineUnit;
            _mineDamageCounter = _damagePerMineUnit;
        }

        public float Value => _propDepositModule.Stock;
        public string Key => nameof(IDurabilityModule);
        public bool IsActive { get; set; }

        public void TakeDamage(int damageValue)
        {
            if (_mineDamageCounter <= 0)
            {
                throw new InvalidOperationException("Попытка нанести урон залежам с нулевым счётчиком.");
            }

            _mineDamageCounter -= damageValue;
            if (_mineDamageCounter <= 0)
            {
                _propDepositModule.Mine();

                if (Value > 0)
                {
                    _mineDamageCounter = _damagePerMineUnit;
                }
                else
                {
                    _lifetimeModule.Destroy();
                }
            }
        }
    }
}