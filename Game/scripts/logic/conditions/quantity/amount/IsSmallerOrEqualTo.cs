using Godot;

namespace Lawfare.scripts.logic.conditions.quantity.amount;

[GlobalClass]
public partial class IsSmallerOrEqualTo : QuantityAmountCondition
{
    protected override bool Evaluate(int threshold, int amount)
    {
        return amount <= threshold;
    }
}