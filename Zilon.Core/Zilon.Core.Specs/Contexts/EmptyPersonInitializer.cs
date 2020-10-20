﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Zilon.Core.Persons;
using Zilon.Core.World;

namespace Zilon.Core.Specs.Contexts
{
    class EmptyPersonInitializer : IPersonInitializer
    {
        public Task<IEnumerable<IPerson>> CreateStartPersonsAsync()
        {
            return Task.FromResult<IEnumerable<IPerson>>(Array.Empty<IPerson>());
        }
    }
}