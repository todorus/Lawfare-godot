using Godot;
using Lawfare.scripts.subject.quantities;

namespace Lawfare.scripts.ui.quantities;

public partial class PropertyObserver : Node
{
    [Signal]
    public delegate void IconChangedEventHandler(Texture2D icon);
    [Signal]
    public delegate void AmountChangedEventHandler(string amount);
    [Signal]
    public delegate void RatioChangedEventHandler(float ratio);

    [Export] 
    private Property _property;

    public void SetQuantity(Quantity quantity)
    {
        if (quantity.Property != _property) return;
        UpdateDisplay(quantity);
    }

    private void UpdateDisplay(IQuantity quantity)
    {
        EmitSignalIconChanged(quantity?.Property.Icon);
        var amount = quantity?.Amount ?? 0;
        EmitSignalAmountChanged(amount.ToString());
        var ratio = quantity != null ? (float) amount / _property.Maximum : 0f;
        EmitSignalRatioChanged(ratio);
    }
}