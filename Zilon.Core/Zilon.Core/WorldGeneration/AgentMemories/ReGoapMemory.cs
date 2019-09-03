using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReGoap.Core;

namespace Zilon.Core.WorldGeneration.AgentMemories
{
    public class ReGoapMemory<T, W> : IReGoapMemory<T, W>
    {
        protected ReGoapState<T, W> state;

        #region UnityFunctions
        public virtual void Awake()
        {
            state = ReGoapState<T, W>.Instantiate();
        }

        protected virtual void OnDestroy()
        {
            state.Recycle();
        }

        protected virtual void Start()
        {
        }
        #endregion

        public virtual ReGoapState<T, W> GetWorldState()
        {
            return state;
        }
    }
}
