﻿using System;

using Zilon.Core.Props;

namespace Zilon.Core.StaticObjectModules
{
    /// <summary>
    /// Реализация сундука с фиксированным лутом.
    /// </summary>
    public class FixedPropChest : ChestBase
    {
        public FixedPropChest(IProp[] props) : base(new ChestStore())
        {
            if (props is null)
            {
                throw new ArgumentNullException(nameof(props));
            }

            foreach (var prop in props)
            {
                Content.Add(prop);
            }
        }

        public override bool IsMapBlock => true;
    }
}