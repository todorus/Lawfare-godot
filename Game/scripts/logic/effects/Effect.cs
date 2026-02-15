using Godot;
using Lawfare.scripts.logic.@event;

namespace Lawfare.scripts.logic.effects;

[GlobalClass]
public abstract partial class Effect : Resource
{
    public bool Applies(GameEvent gameEvent) => true;

    public abstract ChangeGroup[] Stage(GameEvent gameEvent);
}