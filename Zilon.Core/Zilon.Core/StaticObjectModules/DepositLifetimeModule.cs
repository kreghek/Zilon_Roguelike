using System;
using System.Linq;
using Zilon.Core.Props;
using Zilon.Core.Tactics;

namespace Zilon.Core.StaticObjectModules
{
    /// <summary>
    /// Модуль времени жизни для залежей.
    /// </summary>
    public class DepositLifetimeModule : ILifetimeModule
    {
        private readonly IPropContainer _containerModule;
        private readonly IPropDepositModule _depositModule;
        private readonly IStaticObject _parentStaticObject;
        private readonly IStaticObjectManager _staticObjectManager;

        public DepositLifetimeModule(IStaticObjectManager staticObjectManager, IStaticObject parentStaticObject)
        {
            _staticObjectManager = staticObjectManager ?? throw new ArgumentNullException(nameof(staticObjectManager));
            _parentStaticObject = parentStaticObject ?? throw new ArgumentNullException(nameof(parentStaticObject));

            _depositModule = _parentStaticObject.GetModule<IPropDepositModule>();
            _containerModule = _parentStaticObject.GetModule<IPropContainer>();

            _depositModule.Mined += DepositModule_Mined;
            _containerModule.ItemsRemoved += ContainerModule_ItemsRemoved;
        }

        /// <inheritdoc/>
        public string Key => nameof(ILifetimeModule);

        /// <inheritdoc/>
        public bool IsActive { get; set; }

        /// <inheritdoc/>
        public event EventHandler Destroyed;

        public void Destroy()
        {
            _depositModule.Mined -= DepositModule_Mined;
            _containerModule.ItemsRemoved -= ContainerModule_ItemsRemoved;
            _staticObjectManager.Remove(_parentStaticObject);
            DoDestroyed();
        }

        private void DepositModule_Mined(object sender, EventArgs e)
        {
            CheckAndDestroy();
        }

        private void ContainerModule_ItemsRemoved(object sender, PropStoreEventArgs e)
        {
            CheckAndDestroy();
        }

        private void CheckAndDestroy()
        {
            if (_depositModule.IsExhausted && !_containerModule.Content.CalcActualItems().Any())
            {
                Destroy();
            }
        }

        private void DoDestroyed()
        {
            Destroyed?.Invoke(this, EventArgs.Empty);
        }
    }
}