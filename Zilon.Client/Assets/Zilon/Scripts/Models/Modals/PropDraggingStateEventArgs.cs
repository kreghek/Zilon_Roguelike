using System;

namespace Assets.Zilon.Scripts.Models.Modals
{
    public sealed class PropDraggingStateEventArgs : EventArgs
    {
        public PropDraggingStateEventArgs(bool dragging)
        {
            Dragging = dragging;
        }

        public bool Dragging { get; }
    }
}
