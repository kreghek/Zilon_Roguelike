namespace Zilon.Core.Schemes
{
    public class MonsterDefenceScheme : IMonsterDefenceScheme
    {
        public IMonsterDefenceItemSubScheme[] Defences { get; }
        public string Sid { get; set; }
        public bool Disabled { get; set; }
        public LocalizedStringSubScheme Name { get; set; }
        public LocalizedStringSubScheme Description { get; set; }
    }
}
