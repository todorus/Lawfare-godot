using Godot;

namespace Lawfare.scripts.logic.initiative.dto;

[GlobalClass]
public partial class InitiativeTrackDto : GodotObject
{
    public bool IsStaging;
    public int DelayToNext;

    public GodotObject? Current;
    public GodotObject? Next;

    public Godot.Collections.Array<ui.initiative.dto.InitiativeSlotDto> Slots = new();
}