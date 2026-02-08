using System.Collections.Generic;
using Godot;
using Lawfare.scripts.@case;
using Lawfare.scripts.subject.quantities;

namespace Lawfare.scripts.ui.testimony; 

public partial class TestimonialsDisplay : Container
{
    [Export] 
    private PropertyConfig _propertyConfig;
    
    private List<TestimonyDisplay> _testimonyDisplays = new();

    public void SetTestimonies(Testimony[] testimonies)
    {
        Testimonies = testimonies;
    }

    private void SetQuantities(Quantities quantities)
    {
        Quantities = quantities;
    }

    private Quantities _quantities;

    public Quantities Quantities
    {
        get => _quantities;
        set
        {
            if (_quantities != null)
            {
                _quantities.OnChange -= UpdateEnabled;   
            }
            _quantities = value;
            if (_quantities != null)
            {
                _quantities.OnChange += UpdateEnabled;
            }
            UpdateEnabled(_quantities);
        }
    }
    
    private Testimony[] Testimonies
    {
        set 
        {
            _testimonyDisplays.Clear();
            this.ClearChildren();
            foreach (var testimony in value)
            {
                var testimonyDisplay = new TestimonyDisplay();
                testimonyDisplay.Testimony = testimony;
                _testimonyDisplays.Add(testimonyDisplay);
                AddChild(testimonyDisplay);
            }
            UpdateEnabled(Quantities);
        }
    }

    private void UpdateEnabled(Quantities quantities)
    {
        var enabled = quantities != null && quantities.Get(_propertyConfig.Charge).IsMax;
        foreach (var testimonyDisplay in _testimonyDisplays)
        {
            testimonyDisplay.Disabled = !enabled;
        }
    }
}