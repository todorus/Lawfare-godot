using Godot;
using Lawfare.scripts.logic.@event;

namespace Lawfare.scripts.logic.conditions.@event;

[GlobalClass]
public abstract partial class EventCondition : Resource
{
    public abstract bool Evaluate(GameEvent context);
}