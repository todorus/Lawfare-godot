using System.Collections.Generic;

namespace Lawfare.scripts.logic.initiative.state;

public sealed class InitiativeSlotState
{
    public int Delay { get; init; }
    public List<IHasInitiative> Row { get; init; } = new();
}