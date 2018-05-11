namespace Zilon.Core.Tactics
{
    public enum SquadTurnState
    {
        None = 0,
		Waiting,
		Acted,

		CantWaiting = Waiting | Acted
    }
}
