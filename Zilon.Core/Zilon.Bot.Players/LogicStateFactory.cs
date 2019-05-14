using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zilon.Bot.Players
{
    public sealed class LogicStateFactory : ILogicStateFactory
    {
        public LogicStateFactory()
        {
        }

        public ILogicState CreateLogic<T>()
        {
            throw new NotImplementedException();
        }

        public ILogicStateTrigger CreateTrigger<T>()
        {
            throw new NotImplementedException();
        }
    }
}
