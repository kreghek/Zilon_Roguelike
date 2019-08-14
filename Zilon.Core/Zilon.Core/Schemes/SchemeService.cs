using System;
using System.Collections.Generic;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Класс для работы со схемами игрового мира.
    /// </summary>
    public sealed class SchemeService : ISchemeService
    {
        private readonly Dictionary<Type, object> _handlerDict;
        private readonly ISchemeServiceHandlerFactory _schemeServiceHandlerFactory;

        /// <summary>
        /// Конструирует экземпляр сервиса схем <see cref="SchemeService"/>.
        /// </summary>
        /// <param name="schemeServiceHandlerFactory">Фабрика для создания обработчиков схем.</param>
        public SchemeService(ISchemeServiceHandlerFactory schemeServiceHandlerFactory)
        {
            _schemeServiceHandlerFactory = schemeServiceHandlerFactory;

            _handlerDict = new Dictionary<Type, object>();

            InitHandler<ILocationScheme, LocationScheme>();
            InitHandler<IPropScheme, PropScheme>();
            InitHandler<ITacticalActScheme, TacticalActScheme>();
            InitHandler<IPersonScheme, PersonScheme>();
            InitHandler<IDropTableScheme, DropTableScheme>();
            InitHandler<IPerkScheme, PerkScheme>();
            InitHandler<IMonsterScheme, MonsterScheme>();
            InitHandler<IDropTableModificatorScheme, DropTableModificatorScheme>();
        }

        private void InitHandler<TScheme, TSchemeImpl>() where TScheme : class, IScheme
            where TSchemeImpl : class, TScheme
        {
            var handler = _schemeServiceHandlerFactory.Create<TSchemeImpl>();
            _handlerDict.Add(typeof(TScheme), handler);
            handler.LoadSchemes();
        }

        /// <summary>Извлечь схему по идентификатору.</summary>
        /// <typeparam name="TScheme">Тип схемы.</typeparam>
        /// <param name="sid">Идентификатор схемы.</param>
        /// <returns>Возвращает экземпляр схемы.</returns>
        /// <exception cref="System.ArgumentNullException">sid</exception>
        /// <exception cref="System.ArgumentException">Тип схемы должен быть интерфейсом, унаследованным от IScheme</exception>
        public TScheme GetScheme<TScheme>(string sid) where TScheme : class, IScheme
        {
            if (sid == null)
            {
                throw new ArgumentNullException(nameof(sid));
            }

            var schemeType = typeof(TScheme);
            if (!schemeType.IsInterface)
            {
                throw new ArgumentException("Тип схемы должен быть интерфейсом, унаследованным от IScheme");
            }

            var handler = GetHandler<TScheme>();
            var scheme = handler.Get(sid);
            return scheme;
        }

        /// <summary>Извлечь все схемы укаканного типа.</summary>
        /// <typeparam name="TScheme">Тип схемы.</typeparam>
        /// <returns>Возвращает набор схем указанного типа.</returns>
        /// <exception cref="System.ArgumentException">Тип схемы должен быть интерфейсом, унаследованным от {typeof(IScheme).Name}</exception>
        public TScheme[] GetSchemes<TScheme>()
            where TScheme : class, IScheme
        {
            var schemeType = typeof(TScheme);
            if (!schemeType.IsInterface)
            {
                throw new ArgumentException($"Тип схемы должен быть интерфейсом, унаследованным от {typeof(IScheme).Name}");
            }

            var handler = GetHandler<TScheme>();
            var allSchemes = handler.GetAll();
            return allSchemes;
        }

        private ISchemeServiceHandler<TScheme> GetHandler<TScheme>()
            where TScheme : class, IScheme
        {
            if (!_handlerDict.TryGetValue(typeof(TScheme), out object handlerObj))
            {
                throw new ArgumentException("Указан неизвестный тип схемы.");
            }

            var handler = (ISchemeServiceHandler<TScheme>)handlerObj;
            return handler;
        }
    }
}