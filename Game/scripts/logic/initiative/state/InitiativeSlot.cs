using System.Collections.Generic;

namespace Lawfare.scripts.logic.initiative.state;

public sealed class InitiativeSlot(int delay, IReadOnlyList<IHasInitiative> row)
{
    public int Delay { get; } = delay;
    public IReadOnlyList<IHasInitiative> Row { get; } = row;
}