using System;
using System.Linq;
using Godot;

namespace Lawfare.scripts.logic.initiative.state;

public class InitiativeSlotState
{
    public IHasInitiative? Occupant;
    public bool IsStaggered;

    public InitiativeSlotState Clone() => new InitiativeSlotState
    {
        Occupant = Occupant,
        IsStaggered = IsStaggered
    };
}