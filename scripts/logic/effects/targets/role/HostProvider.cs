using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.effects.targets.role;

[GlobalClass]
public partial class HostProvider : SubjectProvider
{
    public override ISubject[] GetSubjects(GameEvent gameEvent)
    {
        if (gameEvent.Host == null) return [];
        return [gameEvent.Host];
    }
}