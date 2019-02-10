using System;
using System.Threading.Tasks;

using Zilon.Core.MapGenerators;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Spec.Mocks
{
    public class FuncMapFactory : IMapFactory
    {
        private Func<Task<IMap>> _factoryFunc;

        public async Task<IMap> CreateAsync(object options)
        {
            if (_factoryFunc == null)
            {
                throw new InvalidOperationException("Не задана фабричная функция.");
            }

            var map = await _factoryFunc();

            return map;
        }

        public void SetFunc(Func<Task<IMap>> factoryFunc)
        {
            _factoryFunc = factoryFunc;
        }
    }
}
