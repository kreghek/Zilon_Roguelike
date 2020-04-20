namespace Zilon.Core.MapGenerators.StaticObjectFactories
{
    /// <summary>
    /// Сервис для сбора и предоставления всех возможных фабрик статических объектов.
    /// </summary>
    public interface IStaticObjectFactoryCollector
    {
        /// <summary>
        /// Возвращает доступные в системе фабрики.
        /// </summary>
        /// <returns></returns>
        IStaticObjectFactory[] GetFactories();
    }
}
