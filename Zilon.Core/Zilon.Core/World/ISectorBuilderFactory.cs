namespace Zilon.Core.World
{
    public interface ISectorBuilderFactory
    {
        ISectorBuilder GetBuilder(ProvinceNode provinceNode);
    }
}
