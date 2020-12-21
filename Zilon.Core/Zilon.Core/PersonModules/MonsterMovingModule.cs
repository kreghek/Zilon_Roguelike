using System;

using Zilon.Core.Schemes;
using Zilon.Core.World;

namespace Zilon.Core.PersonModules
{
    /// <summary>
    /// Реализация модуля перемещения для моснтров.
    /// Зависит от схемы монстра.
    /// </summary>
    public sealed class MonsterMovingModule : IMovingModule
    {
        private int BASE_MOVE_SPEED = GlobeMetrics.OneIterationLength;
        private readonly IMonsterScheme _monsterScheme;

        public MonsterMovingModule(IMonsterScheme monsterScheme)
        {
            _monsterScheme = monsterScheme ?? throw new ArgumentNullException(nameof(monsterScheme));
        }

        public string Key => nameof(IMovingModule);
        public bool IsActive { get; set; }

        public int CalculateCost()
        {
            var moveSpeed = _monsterScheme.BaseMoveSpeed;
            if (moveSpeed == 0)
            {
                moveSpeed = BASE_MOVE_SPEED;
            }

            return moveSpeed;
        }
    }
}