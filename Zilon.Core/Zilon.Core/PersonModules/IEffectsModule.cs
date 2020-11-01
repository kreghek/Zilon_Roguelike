using Zilon.Core.Persons;

namespace Zilon.Core.PersonModules
{
    public interface IEffectsModule : IPersonModule
    {
        IEnumerable<IPersonEffect> Items { get; }

        event EventHandler<EffectEventArgs> Added;
        event EventHandler<EffectEventArgs> Changed;
        event EventHandler<EffectEventArgs> Removed;

        void Add(IPersonEffect effect);
        void Remove(IPersonEffect effect);
    }
}