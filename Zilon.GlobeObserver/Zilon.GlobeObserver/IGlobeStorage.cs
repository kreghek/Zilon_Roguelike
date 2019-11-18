using System;
using System.Collections.Generic;
using System.Text;

namespace Zilon.GlobeObserver
{
    public interface IGlobeStorage
    {
        void Save(GlobeState globeState, string name);
    }
}
