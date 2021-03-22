using Zilon.Core.Client;
using Zilon.Core.Tactics;

namespace Zilon.TextClient
{
    internal class StaticObjectViewModel : IContainerViewModel
    {
        public object Item => StaticObject;
        public IStaticObject StaticObject { get; set; }
    }
}