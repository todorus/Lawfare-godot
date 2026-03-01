using Godot;
using Lawfare.scripts.subject.quantities;

namespace Lawfare.scripts.ui.character;

public partial class PropertyObserver : Control
{
    [Signal]
    public delegate void MeterVisibleEventHandler(bool visible);
    
    [Signal] 
    public delegate void MeterFullEventHandler(bool active);
    
    [Signal]
    public delegate void MeterProgressEventHandler(float progress);
    
    [Export]
    private Property _property;
    
    
    public void SetQuantities(Quantities quantities)
    {
        Quantities = quantities;
    }

    private Quantities _quantities;
    private Quantities Quantities
    {
        get => _quantities;
        set 
        {
            if (_quantities != null)
            {
                _quantities.OnChange -= OnQuantitiesChanged;   
            }
            _quantities = value;
            if (_quantities != null)
            {
                _quantities.OnChange += OnQuantitiesChanged;
            }
            
            OnQuantitiesChanged(_quantities);
        }
    }

    private void OnQuantitiesChanged(Quantities quantities)
    {
        var hasProperty = quantities?.Has(_property) ?? false;
        EmitSignalMeterVisible(hasProperty);
        if(!hasProperty) return;
        
        var quantity = quantities.Get(_property);
        
        EmitSignalMeterFull(quantity.IsMax);
        EmitSignalMeterProgress(quantity.Progress);
    }
}