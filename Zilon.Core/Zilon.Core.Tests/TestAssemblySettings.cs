using NUnit.Framework;

[assembly: Parallelizable(ParallelScope.All)]
[assembly: LevelOfParallelism(100)]
namespace Zilon.Core.Tests
{
}