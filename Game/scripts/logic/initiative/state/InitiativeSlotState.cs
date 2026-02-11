using System;
using System.Linq;
using Godot;

namespace Lawfare.scripts.logic.initiative.state;

public class InitiativeSlotState
{
    public int Delay;
    public IHasInitiative[] Row = Array.Empty<IHasInitiative>();

    public InitiativeSlotState Clone()
    {
        return new InitiativeSlotState
        {
            Delay = Delay,
            Row = Row.ToArray()
        };
    }
}