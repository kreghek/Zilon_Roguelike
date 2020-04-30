using Zilon.Core.Persons;

namespace Zilon.Core.PersonModules
{
    /// <summary>
    /// Базовая реализация объекта для хранения сведений о тактических действиях персонажа.
    /// </summary>
    public sealed class CombatActModule : ICombatActModule
    {
        public CombatActModule()
        {
            IsActive = true;
        }

        public ITacticalAct[] Acts { get; set; }
        public string Key { get => nameof(ICombatActModule); }
        public bool IsActive { get; set; }
    }
}
