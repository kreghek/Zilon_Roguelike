using System;

using Zilon.Core.Schemes;
using Zilon.Core.Tactics;

namespace Zilon.Core.StaticObjectModules
{
    public class PropDepositModule : IPropDepositModule
    {
        private readonly IDropResolver _dropResolver;
        private readonly IDropTableScheme _dropTableScheme;

        private readonly int _exhaustingValue;
        private readonly IPropContainer _propContainer;
        private readonly string[] _toolTags;
        private int _exhaustingCounter;

        public PropDepositModule(IPropContainer propContainer,
            IDropTableScheme dropTableScheme,
            IDropResolver dropResolver,
            string[] toolTags,
            int exhaustingValue,
            DepositMiningDifficulty depositMiningDifficulty)
        {
            _propContainer = propContainer ?? throw new ArgumentNullException(nameof(propContainer));
            _dropTableScheme = dropTableScheme ?? throw new ArgumentNullException(nameof(dropTableScheme));
            _dropResolver = dropResolver ?? throw new ArgumentNullException(nameof(dropResolver));
            _toolTags = toolTags ?? throw new ArgumentNullException(nameof(toolTags));

            _exhaustingValue = exhaustingValue;
            _exhaustingCounter = exhaustingValue;
            Difficulty = depositMiningDifficulty;
        }

        private void DoMined()
        {
            var eventArgs = new EventArgs();
            Mined?.Invoke(this, eventArgs);
        }

        /// <inheritdoc />
        public event EventHandler Mined;

        /// <inheritdoc />
        public string[] GetToolTags()
        {
            return _toolTags;
        }

        /// <inheritdoc />
        public bool IsExhausted => Stock <= 0;

        /// <inheritdoc />
        public bool IsActive { get; set; }

        /// <inheritdoc />
        public string Key => nameof(IPropDepositModule);

        public DepositMiningDifficulty Difficulty { get; }
        public float Stock => (float)_exhaustingCounter / _exhaustingValue;

        /// <inheritdoc />
        public void Mine()
        {
            if (_exhaustingCounter <= 0)
            {
                throw new InvalidOperationException("Попытка выполнить добычу в исчерпанных залежах");
            }

            var props = _dropResolver.Resolve(new[] { _dropTableScheme });
            foreach (var prop in props)
            {
                _propContainer.Content.Add(prop);
                _propContainer.IsActive = true;
            }

            _exhaustingCounter--;

            DoMined();
        }
    }
}