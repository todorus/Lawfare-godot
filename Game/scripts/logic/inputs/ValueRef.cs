using Godot;
using Lawfare.scripts.context;
using Lawfare.scripts.logic.@event;

namespace Lawfare.scripts.logic.inputs;

public abstract partial class ValueRef : Resource
{
    public abstract object GetValue(GameEvent gameEvent);
}