using System;

namespace Zilon.Core.StaticObjectModules
{
    /// <summary>
    /// Модуль, указывающий, что статический объект может находится в секторе какие-то определённое время.
    /// Это нужно:
    /// - Залежам, которые исчезают, когда исчерпаны.
    /// - Кустам, которые могут быть срезаны.
    /// - Деревьям, которые могут быть убраны.
    /// - Лужам, которые исчерпытваются.
    /// </summary>
    public interface ILifetimeModule : IStaticObjectModule
    {
        event EventHandler Destroyed;
    }
}
