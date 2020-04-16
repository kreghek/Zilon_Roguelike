namespace Zilon.Core.StaticObjectModules
{
    public interface IStaticObjectModule
    {
        string Key { get; }
        bool IsActive { get; set; }
    }
}
