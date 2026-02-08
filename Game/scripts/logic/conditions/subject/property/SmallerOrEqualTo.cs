using Godot;

namespace Lawfare.scripts.logic.conditions.subject.property;

[GlobalClass]
public partial class SmallerOrEqualTo : PropertyCondition
{
    protected override bool Compare(int value, int amount)
    {
        return value <= amount;
    }
}