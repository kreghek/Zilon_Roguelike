using System;
using System.Collections.Generic;

using Zilon.Core.Schemes;

namespace Zilon.Core.MapGenerators
{
    public class SectorMapFactoryOptions : ISectorMapFactoryOptions
    {
        public SectorMapFactoryOptions(
            ISectorMapFactoryOptionsSubScheme optionsSubScheme,
            IEnumerable<RoomTransition> transitions)
        {
            OptionsSubScheme = optionsSubScheme ?? throw new ArgumentNullException(nameof(optionsSubScheme));
            Transitions = transitions ?? throw new ArgumentNullException(nameof(transitions));
        }

        public SectorMapFactoryOptions(ISectorMapFactoryOptionsSubScheme optionsSubScheme)
        {
            OptionsSubScheme = optionsSubScheme ?? throw new ArgumentNullException(nameof(optionsSubScheme));
            Transitions = Array.Empty<RoomTransition>();
        }

        public ISectorMapFactoryOptionsSubScheme OptionsSubScheme { get; set; }

        public IEnumerable<RoomTransition> Transitions { get; set; }
    }
}