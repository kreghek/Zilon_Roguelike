using System;

using Zilon.Core.MapGenerators;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Spec.Mocks
{
    public class FuncMapFactory : IMapFactory
    {
        private Func<IMap> _factoryFunc;

        public IMap Create()
        {
            if (_factoryFunc == null)
            {
                throw new InvalidOperationException("Не задана фабричная функция.");
            }

            return _factoryFunc();
        }

        public void SetFunc(Func<IMap> factoryFunc)
        {
            _factoryFunc = factoryFunc;
        }
    }
}
