using System.Collections.Generic;
using System.Linq;
using Godot;
using Lawfare.scripts.@case;
using Lawfare.scripts.characters;

namespace Lawfare.scripts.ui.testimony; 

public partial class TestimonialsDisplay : Container
{
    private List<TestimonyDisplay> _testimonyDisplays = new();

    public void SetTestimonies(Testimony[] testimonies)
    {
        Testimonies = testimonies;
    }
    
    private List<Witness> _canElicit;

    public void SetCanElicit(Witness[] witnesses)
    {
        _canElicit = witnesses.ToList();
        UpdateEnabled();
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
            UpdateEnabled();
        }
    }

    private void UpdateEnabled()
    {
        var defs = _canElicit?.Select(witness => witness.Definition).ToArray() ?? [];
        foreach (var testimonyDisplay in _testimonyDisplays)
        {
            testimonyDisplay.Disabled = !defs.Contains(testimonyDisplay.Testimony.Witness);
        }
    }
}