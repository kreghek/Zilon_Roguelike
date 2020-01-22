using System.Collections.Generic;

using Zenject;

using Zilon.Core.Commands;

namespace Assets.Zilon.Scripts.Services
{
    public class SpecialCommandManager
    {
        private readonly DiContainer _diContainer;
        private readonly Dictionary<int, ICommand<SectorCommandContext>> _equipCommandDict;

        public SpecialCommandManager(DiContainer diContainer)
        {
            _diContainer = diContainer;

            _equipCommandDict = new Dictionary<int, ICommand<SectorCommandContext>>();
        }

        public ICommand<SectorCommandContext> GetEquipCommand(int slotIndex)
        {
            if (_equipCommandDict.TryGetValue(slotIndex, out var equipWrapperCommand))
            {
                return equipWrapperCommand;
            }

            equipWrapperCommand = _diContainer.ResolveId<ICommand<SectorCommandContext>>("equip-command");
            var castedWrapper = (UpdateGlobeCommand<SectorCommandContext>)equipWrapperCommand;
            _equipCommandDict[slotIndex] = equipWrapperCommand;

            var undelyingEquipCommand = castedWrapper.UnderlyingCommand as EquipCommand;

            undelyingEquipCommand.SlotIndex = slotIndex;

            return equipWrapperCommand;
        }
    }
}
