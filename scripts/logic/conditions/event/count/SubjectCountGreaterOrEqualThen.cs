using Godot;

namespace Lawfare.scripts.logic.conditions.@event.count;

[GlobalClass]
public partial class SubjectCountGreaterOrEqualThen : SubjectCountCondition
{
    protected override bool Compare(int value)
    {
        return Value >= value;
    }
}