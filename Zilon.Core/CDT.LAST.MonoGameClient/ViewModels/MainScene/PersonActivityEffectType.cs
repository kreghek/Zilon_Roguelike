namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    /// <summary>
    /// The type of activity that persons can perform in the world except the fighting.
    /// </summary>
    internal enum PersonActivityEffectType
    {
        Undefined = 0,

        /// <summary>
        /// Just moving.
        /// </summary>
        Move,

        /// <summary>
        /// Take some things from containers or put a things into containers.
        /// </summary>
        TransferProp,

        /// <summary>
        /// Moving between sectors (levels).
        /// Sounds like moving.
        /// </summary>
        Transit
    }
}