using Godot;
using Lawfare.scripts.subject.quantities;

namespace Lawfare.scripts.ui.quantities;

public partial class PropertyObserver : Control
{
    [Signal]
    public delegate void IconChangedEventHandler(Texture2D icon);
    [Signal]
    public delegate void AmountChangedEventHandler(string amount);
    [Signal]
    public delegate void RatioChangedEventHandler(float ratio);
    
    [Signal]
    public delegate void OnLayoutDirectionChangedEventHandler(Window.LayoutDirection layoutDirection);
    
    [Signal]
    public delegate void OnOppositeLayoutDirectionChangedEventHandler(Window.LayoutDirection layoutDirection);
    
    private Property _property;

    [Export]
    public Property Property
    {
        get => _property;
        set
        {
            _property = value;
            EmitSignalIconChanged(Property.Icon);
        }
    }

    public void SetMirror(bool value)
    {
        EmitSignalOnLayoutDirectionChanged(value ? Window.LayoutDirection.Rtl : Window.LayoutDirection.Ltr);
        EmitSignalOnOppositeLayoutDirectionChanged(value ? Window.LayoutDirection.Ltr : Window.LayoutDirection.Rtl);
    }

    public void SetQuantity(Quantity quantity)
    {
        if (quantity.Property != _property) return;
        UpdateDisplay(quantity);
    }

    private void UpdateDisplay(IQuantity quantity)
    {
        var amount = quantity?.Amount ?? 0;
        EmitSignalAmountChanged(amount.ToString());
        var ratio = quantity != null ? (float) amount / _property.Maximum : 0f;
        EmitSignalRatioChanged(ratio);
    }
}