using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Текущее состояние перка персонажа.
    /// </summary>
    public class Perk: IPerk
    {
        /// <summary>
        /// Схема перка.
        /// </summary>
        public PerkScheme Scheme { get; set; }

        /// <summary>
        /// Схема уровеня перка, которая прокачивается.
        /// </summary>
        public PerkLevelSubScheme TargetLevelScheme { get; set; }

        /// <summary>
        /// Достигнутые уровени перка.
        /// </summary>
        public PerkLevelSubScheme[] ArchievedLevelSchemes { get; set; }

        /// <summary>
        /// Аккумулированные работы по перку.
        /// Засчитываются в зависимости от области действия работ перка.
        /// </summary>
        public PerkJob[] CurrentLevelJobs { get; set; }

        /// <summary>
        /// Текущий уровень перка. Иначе - индекс схемы уровня.
        /// </summary>
        public int? CurrentLevel { get; set; }

        /// <summary>
        /// Подуровень. Или уровень внутри схемы уровня перка.
        /// </summary>
        public int CurrentSubLevel { get; set; }

        /// <summary>
        /// Признак того, что целевой уровень перка проплачен.
        /// </summary>
        public bool IsLevelPaid { get; internal set; }

        public void AddProgress(IJobProgress jobProgress)
        {
            switch (jobProgress)
            {
                case DefeatActorJobProgress: 
            }

            foreach (var job in CurrentLevelJobs)
            {
                switch(job.)
            }
        }
    }
}
