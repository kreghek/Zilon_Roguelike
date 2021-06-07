using System;

using Zilon.Core.Client;
using Zilon.Core.Props;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.Ui
{
    public class PropViewModel : IPropItemViewModel
    {
        public PropViewModel(IProp prop)
        {
            Prop = prop ?? throw new ArgumentNullException(nameof(prop));
        }

        public IProp Prop { get; }
    }
}