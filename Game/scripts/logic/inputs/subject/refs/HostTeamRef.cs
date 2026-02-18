using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.inputs.subject.refs;

[GlobalClass]
public partial class HostTeamRef : SubjectRef
{
    protected override ISubject GetSubjectValue(GameEvent gameEvent)
    {
        return gameEvent.Context.GetTeam(gameEvent.Host);
    }
}