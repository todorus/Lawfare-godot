using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.targets;

[GlobalClass]
public abstract partial class TargetSelector : Resource
{
    // TODO add an ordering mechanism, when we want partial completion of targets
    public abstract ISubject[] Select(GameEvent gameEvent);
}