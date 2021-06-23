using Zilon.Core.Tactics;

namespace Zilon.Core.MapGenerators.StaticObjectFactories
{
    /// <summary>
    /// Сервис для сбора и предоставления всех возможных фабрик статических объектов.
    /// </summary>
    public interface IStaticObjectFactoryCollector
    {
        /// <summary>
        /// Select factory by purpose of static object.
        /// </summary>
        /// <returns>Factory to create static object of specified purpose.</returns>
        IStaticObjectFactory SelectFactoryByStaticObjectPurpose(PropContainerPurpose purpose);
    }
}