using System.Linq;
using Godot;
using Lawfare.scripts.logic.conditions.subject;
using Lawfare.scripts.logic.@event;

namespace Lawfare.scripts.logic.conditions.@event.count;

[GlobalClass]
public abstract partial class SubjectCountCondition : EventCondition
{
    [Export] public SubjectCondition[] SubjectConditions = [];

    [Export] public int Value;

    public override bool Evaluate(GameEvent context)
    {
        var count = context.Subjects
            .Count(subject => SubjectConditions.All(condition => condition.Evaluate(context, subject)));
        return Compare(count);
    }

    protected abstract bool Compare(int value);
}