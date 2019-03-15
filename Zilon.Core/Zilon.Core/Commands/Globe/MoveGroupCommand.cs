using System;

using Zilon.Core.Client;

namespace Zilon.Core.Commands.Globe
{
    public class MoveGroupCommand : ICommand
    {
        private readonly IGlobeUiState _globeUiState;

        public MoveGroupCommand(IGlobeUiState globeUiState)
        {
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
