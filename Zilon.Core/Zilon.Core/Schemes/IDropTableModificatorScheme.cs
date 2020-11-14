namespace Zilon.Core.Schemes
{
    public interface IDropTableModificatorScheme : IScheme
    {
        string[] PropSids { get; set; }

        float WeightBonus { get; set; }
    }
}