using System;
using System.Threading.Tasks;

using Zilon.Core.MapGenerators;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Spec.Mocks
{
    public class FuncMapFactory : IMapFactory
    {
        private Func<IMap> _factoryFunc;

        public Task<IMap> CreateAsync(object options)
        {
            if (_factoryFunc == null)
            {
                throw new InvalidOperationException("Не задана фабричная функция.");
            }

            var map = _factoryFunc();

            return Task.FromResult(map);
        }

        public void SetFunc(Func<IMap> factoryFunc)
        {
            _factoryFunc = factoryFunc;
        }
    }
}
