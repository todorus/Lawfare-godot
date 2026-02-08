using Godot;

namespace Lawfare.scripts.logic.effects.property.amounts.property;

[GlobalClass]
public partial class PropertyAmount : PropertyAmountProvider
{
    protected override int CountProperty(int propertyValue)
    {
        return propertyValue;
    }
}