using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReGoap.Core;

namespace Zilon.Core.WorldGeneration.AgentMemories
{
    public class ReGoapMemoryAdvanced<T, W> : ReGoapMemory<T, W>
    {
        private IReGoapSensor<T, W>[] sensors;

        public float SensorsUpdateDelay = 0.3f;
        private float sensorsUpdateCooldown;

        #region UnityFunctions
        public override void Awake()
        {
            base.Awake();
            sensors = new IReGoapSensor<T, W>[0];// GetComponents<IReGoapSensor<T, W>>();
            foreach (var sensor in sensors)
            {
                sensor.Init(this);
            }
        }

        protected virtual void Update()
        {
            //if (Time.time > sensorsUpdateCooldown)
            {
                //sensorsUpdateCooldown = Time.time + SensorsUpdateDelay;

                foreach (var sensor in sensors)
                {
                    sensor.UpdateSensor();
                }
            }
        }
        #endregion
    }
}
