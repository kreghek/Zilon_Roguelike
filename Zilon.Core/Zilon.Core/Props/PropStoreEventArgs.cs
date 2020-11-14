namespace Zilon.Core.Props
{
    /// <summary>
    ///     Аргументы событий, связанных с инвентарём.
    /// </summary>
    public class PropStoreEventArgs
    {
        [ExcludeFromCodeCoverage]
        public PropStoreEventArgs([NotNull] [ItemNotNull] IEnumerable<IProp> props)
        {
            if (props == null)
            {
                throw new ArgumentNullException(nameof(props));
            }

            Props = props.ToArray();
        }

        [ExcludeFromCodeCoverage]
        public PropStoreEventArgs([NotNull] [ItemNotNull] params IProp[] props)
        {
            Props = props ?? throw new ArgumentNullException(nameof(props));
        }

        [PublicAPI] public IProp[] Props { get; }
    }
}