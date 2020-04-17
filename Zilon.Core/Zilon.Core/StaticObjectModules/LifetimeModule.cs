using System;

namespace Zilon.Core.StaticObjectModules
{
    public sealed class LifetimeModule : ILifetimeModule
    {
        public LifetimeModule()
        {
            IsActive = true;
        }

        /// <inheritdoc/>
        public string Key { get => nameof(ILifetimeModule); }

        /// <inheritdoc/>
        public bool IsActive { get; set; }

        /// <inheritdoc/>
        public event EventHandler Destroyed;
    }
}
