namespace Zilon.Core.Tactics
{
    public sealed class DamageEfficientCalc
    {
        public int ArmorAbsorbtion { get; set; }

        public int ActApRank { get; internal set; }
        public int? ArmorRank { get; internal set; }
        public int FactArmorSaveRoll { get; internal set; }
        public int SuccessArmorSaveRoll { get; internal set; }
        public int ActEfficientArmorBlocked { get; internal set; }

        public int ResultEfficient => ActEfficientArmorBlocked;

        public bool TargetSuccessfullUsedArmor { get; internal set; }
    }
}
