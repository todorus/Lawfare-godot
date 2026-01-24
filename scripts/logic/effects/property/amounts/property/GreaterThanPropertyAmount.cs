using System;
using Godot;

namespace Lawfare.scripts.logic.effects.property.amounts.property;

[GlobalClass]
public partial class GreaterThanPropertyAmount : PropertyAmountProvider
{
    [Export] private int Amount;

    protected override int CountProperty(int propertyValue)
    {
        return Math.Max(propertyValue - Amount, 0);
    }
}