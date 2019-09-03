using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zilon.Core.WorldGeneration
{
    public class ReGoapAgentAdvanced<T, W> : ReGoapAgent<T, W>
    {
        #region UnityFunctions
        protected virtual void Update()
        {
            possibleGoalsDirty = true;

            if (currentActionState == null)
            {
                if (!IsPlanning)
                    CalculateNewGoal();
                return;
            }
        }
        #endregion
    }
}
