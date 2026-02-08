using System.Collections.Generic;

namespace Lawfare.scripts.logic.initiative.state;

public sealed class InitiativeTrackState
{
    public bool IsStaging { get; init; }
    public IHasInitiative? Current { get; init; }
    public List<InitiativeSlotState> Slots { get; init; } = new();
}