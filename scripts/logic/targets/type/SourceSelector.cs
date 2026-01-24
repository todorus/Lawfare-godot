using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.targets.type;

[GlobalClass]
public partial class SourceSelector : TargetSelector
{
    public override ISubject[] Select(GameEvent gameEvent)
    {
        return gameEvent.Source != null
            ? [gameEvent.Source]
            : [];
    }
}