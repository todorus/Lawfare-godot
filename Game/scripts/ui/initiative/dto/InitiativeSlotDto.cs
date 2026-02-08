using Godot;

namespace Lawfare.scripts.logic.initiative.dto;

[GlobalClass]
public partial class InitiativeSlotDto : GodotObject
{
    public int Delay;

    /// <summary>
    /// Godot-side references for UI. You decide what entity maps to (e.g., Character, Card, etc.).
    /// </summary>
    public Godot.Collections.Array<GodotObject> Entities = new();
}