using System;

using Zilon.Core.Tactics;

namespace Zilon.Core.StaticObjectModules
{
    public class LifetimeModule : ILifetimeModule
    {
        private readonly IStaticObjectManager _staticObjectManager;
        private readonly IStaticObject _parentStaticObject;

        public LifetimeModule(IStaticObjectManager staticObjectManager, IStaticObject parentStaticObject)
        {
            _staticObjectManager = staticObjectManager ?? throw new ArgumentNullException(nameof(staticObjectManager));
            _parentStaticObject = parentStaticObject;
        }

        /// <inheritdoc/>
        public string Key { get => nameof(ILifetimeModule); }

        /// <inheritdoc/>
        public bool IsActive { get; set; }

        /// <inheritdoc/>
        public event EventHandler Destroyed;

        public void Destroy()
        {
            _staticObjectManager.Remove(_parentStaticObject);
            DoDestroyed();
        }

        private void DoDestroyed()
        {
            Destroyed?.Invoke(this, EventArgs.Empty);
        }
    }
}