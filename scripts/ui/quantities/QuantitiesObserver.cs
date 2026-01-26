using Godot;
using Lawfare.scripts.subject.quantities;

namespace Lawfare.scripts.ui.quantities;

public partial class QuantitiesObserver : Container
{
    [Export]
    private PackedScene _quantityScene;

    public void SetQuantities(Quantities quantities) => Quantities = quantities;
    
    private Quantities _quantities;
    public Quantities Quantities
    {
        set
        {
            if (_quantities != null)
            {
                _quantities.OnChange -= UpdateQuantitiesDisplay;
            }
            
            _quantities = value;
            if (value != null)
            {
                value.OnChange += UpdateQuantitiesDisplay;
            }
            UpdateQuantitiesDisplay(value);
        }
    }

    private void UpdateQuantitiesDisplay(Quantities value)
    {
        this.ClearChildren();
        if (value == null) return;

        foreach (var quantity in value.All)
        {
            var quantityDisplay = _quantityScene.Instantiate<QuantityObserver>();
            quantityDisplay.Quantity = quantity;
            AddChild(quantityDisplay);
        }
    }
}