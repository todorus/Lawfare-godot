using Godot;

namespace Lawfare.scripts.subject.quantities;

[GlobalClass]
public partial class Quantity : Resource, IQuantity
{
    [Export] public Property Property { get; set; }

    [Export] public int Amount { get; set; }
}

public interface IQuantity
{
    public Property Property { get; }
    public int Amount { get; set; }
}