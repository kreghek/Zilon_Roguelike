using System;

namespace Assets.Zilon.Scripts.Services.CombatScene
{
    interface IPersonCommandHandler
    {
        void LocationVM_OnSelect(object sender, EventArgs e);
        void SquadVM_OnSelect(object sender, EventArgs e);
    }
}