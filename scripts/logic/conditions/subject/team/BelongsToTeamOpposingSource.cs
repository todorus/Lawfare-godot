using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.conditions.subject.team;

[GlobalClass]
public partial class BelongsToTeamOpposingSource : SubjectCondition
{
    public override bool Evaluate(GameEvent gameEventData, ISubject subject)
    {
        var context = gameEventData.Context;
        var opposingSourceTeam = context.GetTeam(gameEventData.Source);
        var subjectTeam = context.GetTeam(subject);
        return opposingSourceTeam == subjectTeam;
    }
}