using Godot;
using Lawfare.scripts.logic.effects;
using Lawfare.scripts.logic.@event;

namespace Lawfare.scripts.logic.conditions.diff;

[GlobalClass]
public abstract partial class DiffCondition : Resource
{
    public abstract bool Evaluate(GameEvent gameEventData, IDiff diff);
}