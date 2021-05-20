﻿using System;
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

        private void CheckAndDestroy()
        {
            var minedProps = _containerModule.Content.CalcActualItems();
            if (_depositModule.IsExhausted && !minedProps.Any())
            {
                Destroy();
            }
        }

        private void ContainerModule_ItemsRemoved(object sender, PropStoreEventArgs e)
        {
            CheckAndDestroy();
        }

        private void DepositModule_Mined(object sender, EventArgs e)
        {
            CheckAndDestroy();
        }

        private void DoDestroyed()
        {
            Destroyed?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc />
        public string Key => nameof(ILifetimeModule);

        /// <inheritdoc />
        public bool IsActive { get; set; }

        public bool IsParentStaticObjectDestroyed { get; private set; }

        /// <inheritdoc />
        public event EventHandler? Destroyed;

        public void Destroy()
        {
            _depositModule.Mined -= DepositModule_Mined;
            _containerModule.ItemsRemoved -= ContainerModule_ItemsRemoved;
            _staticObjectManager.Remove(_parentStaticObject);
            IsParentStaticObjectDestroyed = true;
            DoDestroyed();
        }
    }
}