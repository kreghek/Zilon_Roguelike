using System.Collections.Generic;

using Zenject;

using Zilon.Core.Commands;

namespace Assets.Zilon.Scripts.Services
{
    public class SpecialCommandManager
    {
        private readonly DiContainer _diContainer;
        private readonly Dictionary<int, EquipCommand> _equipCommandDict;

        public SpecialCommandManager(DiContainer diContainer)
        {
            _diContainer = diContainer;

            _equipCommandDict = new Dictionary<int, EquipCommand>();
        }

        public EquipCommand GetEquipCommand(int slotIndex)
        {
            if (_equipCommandDict.TryGetValue(slotIndex, out var equipCommand))
            {
                return equipCommand;
            }

            equipCommand = _diContainer.ResolveId<ICommand>("equip-command") as EquipCommand;
            _equipCommandDict[slotIndex] = equipCommand;
            equipCommand.SlotIndex = slotIndex;

            return equipCommand;
        }
    }
}
