namespace Zilon.Core.StaticObjectModules
{
    /// <summary>
    ///     Модуль, отвечающий за время жизни статического объекта.
    ///     Если он установлен для объекта, то вероятнее всего объект в секторе будет уничтожаться.
    /// </summary>
    public interface ILifetimeModule : IStaticObjectModule
    {
        void Destroy();

        event EventHandler Destroyed;
    }
}