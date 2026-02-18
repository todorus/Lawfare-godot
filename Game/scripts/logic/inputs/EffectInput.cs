using Godot;
using Lawfare.scripts.logic.@event;

namespace Lawfare.scripts.logic.inputs;

[GlobalClass]
public abstract partial class EffectInput : Resource
{
    public abstract object GetValue(GameEvent gameEvent);
}