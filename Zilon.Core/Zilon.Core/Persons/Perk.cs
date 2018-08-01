using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    public class Perk
    {
        public PerkScheme Scheme { get; set; }
        public PerkLevelSubScheme TargetLevelScheme { get; set; }
        public PerkLevelSubScheme ArchievedLevelScheme { get; set; }
        public PerkJob[] DoneLevelJobs { get; set; }
        public int? CurrentLevel { get; set; }
        public int CurrentSubLevel { get; set; }
        public bool IsLevelPaid { get; internal set; }
    }
}
