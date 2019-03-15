using System;

using Zilon.Core.Client;
using Zilon.Core.World;

namespace Zilon.Core.Commands.Globe
{
    public class MoveGroupCommand : ICommand
    {
        private readonly IWorldManager _worldManager;
        private readonly IGlobeUiState _globeUiState;

        public MoveGroupCommand(IWorldManager worldManager,
            IGlobeUiState globeUiState)
        {
            _worldManager = worldManager;
            _globeUiState = globeUiState;
        }

        public bool CanExecute()
        {
            return true;
        }

        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
