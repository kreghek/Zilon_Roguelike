using System;

namespace Zilon.Core.StaticObjectModules
{
    public sealed class DepositDurabilityModule : IDurabilityModule
    {
        private readonly IPropDepositModule _propDepositModule;
        private readonly ILifetimeModule _lifetimeModule;
        private readonly int _damagePerMineUnit;
        private int _mineDamageCounter;

        public DepositDurabilityModule(IPropDepositModule propDepositModule, ILifetimeModule lifetimeModule, int damagePerMineUnit)
        {
            IsActive = true;
            _propDepositModule = propDepositModule;
            _lifetimeModule = lifetimeModule;
            _damagePerMineUnit = damagePerMineUnit;
            _mineDamageCounter = _damagePerMineUnit;
        }

        public float Value { get => _propDepositModule.Stock; }
        public string Key { get => nameof(IDurabilityModule); }
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
