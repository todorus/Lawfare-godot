using Godot;

namespace Lawfare.scripts.subject.quantities;

[GlobalClass]
public partial class Quantity : Resource, IQuantity
{
    [Export] public Property Property { get; set; }

    [Export] public int Amount { get; set; }
    
    public static Quantity operator -(Quantity a, Quantity b)
    {
        var prop = a?.Property ?? b?.Property;
        var amount = (a?.Amount ?? 0) - (b?.Amount ?? 0);

        if (a == null || b == null)
            GD.PushWarning("Quantity.operator-: one or both operands are null");

        if (a?.Property != b?.Property)
            GD.PushWarning("Quantity.operator-: operand properties differ");

        return new Quantity
        {
            Property = prop,
            Amount = amount
        };
    }
}

public interface IQuantity
{
    public Property Property { get; }
    public int Amount { get; set; }
}