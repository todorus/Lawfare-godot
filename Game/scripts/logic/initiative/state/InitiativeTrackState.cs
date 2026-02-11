using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Lawfare.scripts.logic.initiative.state;

public class InitiativeTrackState
{
    // Slots are absolute delays from "now".
    // Slot.Delay is always >= 0.
    // Ordering of Slots is by Delay ascending.
    public InitiativeSlotState[] Slots = Array.Empty<InitiativeSlotState>();

    public InitiativeTrackState Clone()
    {
        var clone = new InitiativeTrackState
        {
            Slots = Slots.Select(s => s.Clone()).ToArray()
        };
        return clone;
    }
}