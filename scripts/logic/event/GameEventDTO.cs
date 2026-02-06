using Godot;

namespace Lawfare.scripts.logic.@event;

public partial class GameEventDto(GameEvent gameEvent) : GodotObject
{
    public readonly GameEvent GameEvent = gameEvent;
}