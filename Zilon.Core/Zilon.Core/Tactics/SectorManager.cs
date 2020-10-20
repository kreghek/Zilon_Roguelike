using System;
using System.Threading.Tasks;

using Zilon.Core.MapGenerators;
using Zilon.Core.Players;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Реализация менеджера сектора.
    /// </summary>
    /// <seealso cref="ISectorManager" />
    public class SectorManager : ISectorManager
    {
        private readonly ISectorGenerator _generator;
        private readonly HumanPlayer _humanPlayer;

        public SectorManager(
            ISectorGenerator generator,
            HumanPlayer humanPlayer)
        {
            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
            _humanPlayer = humanPlayer ?? throw new ArgumentNullException(nameof(humanPlayer));
        }

        /// <summary>
        /// Текущий сектор.
        /// </summary>
        public ISector CurrentSector { get; private set; }

        /// <summary>
        /// Создаёт текущий сектор по указанному генератору и настройкам.
        /// </summary>
        public async Task CreateSectorAsync()
        {
            CurrentSector = await _generator.GenerateAsync(_humanPlayer.SectorNode).ConfigureAwait(false);
        }
    }
}