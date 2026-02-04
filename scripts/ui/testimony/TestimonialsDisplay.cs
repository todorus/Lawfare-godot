using System.Collections.Generic;
using Godot;
using Lawfare.scripts.@case;
using Lawfare.scripts.characters.ult;

namespace Lawfare.scripts.ui.testimony; 

public partial class TestimonialsDisplay : Container
{
    private List<TestimonyDisplay> _testimonyDisplays = new();

    public void SetTestimonies(Testimony[] testimonies)
    {
        Testimonies = testimonies;
    }

    private void SetUlt(Ult ult)
    {
        Ult = ult;
    }

    private Ult _ult;

    public Ult Ult
    {
        get => _ult;
        set
        {
            if (_ult != null)
            {
                _ult.OnChange -= UpdateEnabled;   
            }
            _ult = value;
            if (_ult != null)
            {
                _ult.OnChange += UpdateEnabled;
            }
            UpdateEnabled(_ult);
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
            UpdateEnabled(Ult);
        }
    }

    private void UpdateEnabled(Ult ult)
    {
        var enabled = ult != null && ult.Active;
        foreach (var testimonyDisplay in _testimonyDisplays)
        {
            testimonyDisplay.Disabled = !enabled;
        }
    }
}