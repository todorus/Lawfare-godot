using Godot;
using Lawfare.scripts.logic.@event;

namespace Lawfare.scripts.logic.effects.group;

[GlobalClass]
public abstract partial class EffectGroup : Resource, IEffectGroup
{
    public bool Applies(GameEvent gameEvent) => true;

    public abstract ChangeGroup[] Stage(GameEvent gameEvent);
}