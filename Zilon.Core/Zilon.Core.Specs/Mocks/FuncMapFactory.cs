using System;
using System.Threading.Tasks;
using Zilon.Core.MapGenerators;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Specs.Mocks
{
    public class FuncMapFactory : IMapFactory
    {
        private Func<Task<ISectorMap>> _factoryFuncAsync;

        public async Task<ISectorMap> CreateAsync(ISectorMapFactoryOptions generationOptions)
        {
            if (_factoryFuncAsync == null)
            {
                throw new InvalidOperationException("Не задана фабричная функция.");
            }

            //TODO Объяснить, почему тут нужно использовать ConfigureAwait(false)
            // Это рекомендация Codacy.
            // Но есть статья https://habr.com/ru/company/clrium/blog/463587/,
            // в которой объясняется, что не всё так просто.
            // Нужно чёткое понимание, зачем здесь ConfigureAwait(false) и
            // к какому результату это приводит по сравнению с простым await.
            var map = await _factoryFuncAsync().ConfigureAwait(false);

            return map;
        }

        public void SetFunc(Func<Task<ISectorMap>> factoryFunc)
        {
            _factoryFuncAsync = factoryFunc;
        }
    }
}