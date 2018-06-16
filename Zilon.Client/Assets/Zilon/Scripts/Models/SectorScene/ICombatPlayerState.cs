namespace Assets.Zilon.Scripts.Models.CombatScene
{
    /// <summary>
    /// Состояние ввода игрока.
    /// </summary>
    /// <remarks>
    /// Испоьзуется командами для получения ввода игрока. Хранит состояние объектов боя.
    /// </remarks>
    interface ICombatPlayerState
    {
        /// <summary>
        /// Выбранный взвод.
        /// </summary>
        CombatSquadVM SelectedSquad { get; set; }

//        /// <summary>
//        /// Выбранная локация.
//        /// </summary>
//        CombatLocationVM SelectedNode { get; set; }
    }
}
