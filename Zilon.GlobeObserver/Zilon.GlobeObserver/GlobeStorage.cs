using System;
using System.Collections.Generic;
using System.Text;

namespace Zilon.GlobeObserver
{
    public class GlobeStorage : IGlobeStorage
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="globeState"></param>
        /// <param name="name"> Наименование мира. </param>
        public void Save(GlobeState globeState, string name)
        {
            var terrain = globeState.Terrain;

        }
    }
}
