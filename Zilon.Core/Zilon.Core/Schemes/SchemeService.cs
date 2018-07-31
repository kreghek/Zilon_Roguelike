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

        public SchemeService(ISchemeLocator schemeLocator)
        {
            _handlerDict = new Dictionary<Type, object>();

            InitHandler<MapScheme>(schemeLocator);
            InitHandler<LocationScheme>(schemeLocator);
            InitHandler<PathScheme>(schemeLocator);
            InitHandler<PropScheme>(schemeLocator);
            InitHandler<TacticalActScheme>(schemeLocator);
            InitHandler<PersonScheme>(schemeLocator);
            InitHandler<DropTableScheme>(schemeLocator);
            InitHandler<PerkScheme>(schemeLocator);
            InitHandler<MonsterScheme>(schemeLocator);
            InitHandler<DropTableModificatorScheme>(schemeLocator);
        }

        private void InitHandler<T>(ISchemeLocator schemeLocator) where T : class, IScheme
        {
            var handler = new SchemeServiceHandler<T>(schemeLocator);
            _handlerDict.Add(typeof(T), handler);
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