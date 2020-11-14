namespace Zilon.Core.Schemes
{
    public class PropBulletSubScheme : SubSchemeBase, IPropBulletSubScheme
    {
        [JsonProperty] public string Caliber { get; private set; }
    }
}