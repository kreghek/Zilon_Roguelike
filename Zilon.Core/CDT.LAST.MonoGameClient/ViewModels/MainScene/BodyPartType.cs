namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    /// <summary>
    /// The part of person body visualization available on the client.
    /// </summary>
    public enum BodyPartType
    {
        Undefined = 0,

        Head,

        Chest,

        ArmLeft,

        /// <summary>
        /// Used when:
        /// - Idle mode.
        /// - No things in the left hand.
        /// - One handed thing in the hand.
        /// </summary>
        ArmRightSimple,

        /// <summary>
        /// Used to bear some things like rifle.
        /// </summary>
        ArmRightRifle,

        /// <summary>
        /// Used to bear two-handed weapon like staffs, grate swords or war hammers.
        /// </summary>
        ArmRightTwoHanded,

        LegsIdle,

        /// <summary>
        /// Used to visualize combat agressive stance of a person.
        /// </summary>
        LegsCombat
    }
}