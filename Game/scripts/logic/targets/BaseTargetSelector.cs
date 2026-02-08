using System.Linq;
using Godot;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;
using SubjectCondition = Lawfare.scripts.logic.conditions.subject.SubjectCondition;

namespace Lawfare.scripts.logic.targets;

[GlobalClass]
public abstract partial class BaseTargetSelector : TargetSelector
{
    protected abstract SubjectCondition[] SubjectConditions { get; }

    public override ISubject[] Select(GameEvent gameEvent)
    {
        return gameEvent.Subjects
            .Where(subject => SubjectConditions.All(condition => condition.Evaluate(gameEvent, subject)))
            .ToArray();
    }
}