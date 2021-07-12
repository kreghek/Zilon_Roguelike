using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Zilon.Core.Players;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.Ui
{
    internal class PersonMarkersPanel
    {
        private readonly SectorViewModelContext _sectorViewModelContext;
        private readonly IList<ActorViewModel> _visibleActors;

        public PersonMarkersPanel(SectorViewModelContext sectorViewModelContext, IPlayer player)
        {
            _sectorViewModelContext = sectorViewModelContext;

            _visibleActors = new List<ActorViewModel>();
        }

        public void Update()
        {
            _visibleActors.Clear();

            var actorViewModels = _sectorViewModelContext.GameObjects.OfType<ActorViewModel>().ToArray();
            foreach (var actorViewModel in actorViewModels)
            { 
                actorViewModel
            }
        }
    }
}
