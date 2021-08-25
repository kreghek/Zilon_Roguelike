﻿using System;
using System.Collections.Generic;

namespace Zilon.Core.World
{
    public sealed class ResourceDepositData : IResourceDepositData
    {
        public ResourceDepositData(IEnumerable<ResourceDepositDataItem> items)
        {
            Items = items ?? throw new ArgumentNullException(nameof(items));
        }

        public IEnumerable<ResourceDepositDataItem> Items { get; }
    }
}