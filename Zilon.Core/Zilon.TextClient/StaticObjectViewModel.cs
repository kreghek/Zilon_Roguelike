using Zilon.Core.Client;
using Zilon.Core.Tactics;

namespace Zilon.TextClient
{
    internal class StaticObjectViewModel : IContainerViewModel
    {
        public IStaticObject StaticObject { get; set; }
        public object Item => StaticObject;
    }
}