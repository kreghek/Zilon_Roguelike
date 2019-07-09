namespace Zilon.Core.PersonDialogs
{
    /// <summary>
    /// Переход между узлами диалога. Возможный ответ пользователя.
    /// </summary>
    public sealed class DialogTransition
    {
        public string Text;

        public DialogNode StartNode { get; set; }

        public DialogNode TargetNode { get; set; }
    }
}
