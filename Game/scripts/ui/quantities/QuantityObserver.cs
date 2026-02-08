using Godot;
using Lawfare.scripts.subject.quantities;

namespace Lawfare.scripts.ui.quantities;

public partial class QuantityObserver : Control
{
    [Signal]
    public delegate void IconChangedEventHandler(Texture2D icon);
    [Signal]
    public delegate void AmountChangedEventHandler(string amount);
    
    private IQuantity _quantity;

    public IQuantity Quantity
    {
        get => _quantity;
        set
        {
            _quantity = value;
            UpdateDisplay(value);
        }
    }

    private void UpdateDisplay(IQuantity quantity)
    {
        EmitSignalIconChanged(quantity?.Property.Icon);
        var amount = quantity?.Amount ?? 0;
        EmitSignalAmountChanged(amount.ToString());
    }
}