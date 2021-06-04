using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    [ExcludeFromCodeCoverage]
    public record ActDescription
    {
        public ActDescription(IEnumerable<string> tags)
        {
            Tags = tags ?? throw new ArgumentNullException(nameof(tags));
        }

        public IEnumerable<string> Tags { get; }

        public static ActDescription CreateFromActStats(ITacticalActStatsSubScheme actStatScheme)
        {
            var notEmptyTagsOrNull = actStatScheme.Tags?
                .Where(x => x != null)?
                .Select(x => x!)?
                .ToArray();

            var usedActDescription = new ActDescription(notEmptyTagsOrNull ?? Array.Empty<string>());
            return usedActDescription;
        }
    }
}