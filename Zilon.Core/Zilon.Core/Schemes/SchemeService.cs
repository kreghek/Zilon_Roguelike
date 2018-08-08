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

            InitHandler<MapScheme>();
            InitHandler<LocationScheme>();
            InitHandler<PathScheme>();
            InitHandler<PropScheme>();
            InitHandler<TacticalActScheme>();
            InitHandler<PersonScheme>();
            InitHandler<DropTableScheme>();
            InitHandler<PerkScheme>();
            InitHandler<MonsterScheme>();
            InitHandler<DropTableModificatorScheme>();
        }

        private void InitHandler<TScheme>() where TScheme : class, IScheme
        {
            var handler = _schemeServiceHandlerFactory.Create<TScheme>();
            _handlerDict.Add(typeof(TScheme), handler);
            handler.LoadSchemes();
        }

        public TScheme GetScheme<TScheme>(string sid) where TScheme : class, IScheme
        {
            var handler = GetHandler<TScheme>();
            var scheme = handler.Get(sid);
            return scheme;
        }

        private SchemeServiceHandler<TScheme> GetHandler<TScheme>() 
            where TScheme : class, IScheme
        {
            if (!_handlerDict.TryGetValue(typeof(TScheme), out object handlerObj))
            {
                throw new ArgumentException("Указан неизвестный тип схемы.");
            }

            var handler = (SchemeServiceHandler<TScheme>)handlerObj;
            return handler;
        }

        public TScheme[] GetSchemes<TScheme>() where TScheme : class, IScheme
        {
            var handler = GetHandler<TScheme>();
            var scheme = handler.GetAll();
            return scheme;
        }
    }
}