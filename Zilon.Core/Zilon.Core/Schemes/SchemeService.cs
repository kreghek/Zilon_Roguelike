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

        public TScheme[] GetSchemes<TScheme>() where TScheme : class, IScheme
        {
            var schemeType = typeof(TScheme);
            if (!schemeType.IsInterface)
            {
                throw new ArgumentException("Тип схемы должен быть интерфейсом, унаследованным от IScheme");
            }

            var handler = GetHandler<TScheme>();
            var scheme = handler.GetAll();
            return scheme;
        }
    }
}