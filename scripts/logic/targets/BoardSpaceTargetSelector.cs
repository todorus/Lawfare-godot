using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.targets;

[GlobalClass]
public partial class BoardSpaceTargetSelector : TargetSelector
{
    public override ISubject[] Select(GameEvent gameEvent)
    {
        return [gameEvent.Space];
    }
}