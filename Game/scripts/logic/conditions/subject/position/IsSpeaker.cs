using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.conditions.subject.position;

[GlobalClass]
public partial class IsSpeaker : SubjectCondition
{
    public override bool Evaluate(GameEvent gameEventData, ISubject subject)
    {
        var context = gameEventData.Context;
        var team = context.GetTeam(subject);
        if(team == null) return false;
        
        var speaker = context.GetSpeaker(team);
        return speaker == subject;
    }
}