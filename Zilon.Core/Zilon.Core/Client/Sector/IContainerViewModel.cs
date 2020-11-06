using Zilon.Core.Tactics;

namespace Zilon.Core.Client
{
    public interface IContainerViewModel : ISelectableViewModel
    {
        IStaticObject StaticObject { get; set; }
    }
}