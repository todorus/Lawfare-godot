using Godot;
using Lawfare.scripts.@case;

namespace Lawfare.scripts.ui.testimony; 

public partial class TestimonialsDisplay : Container
{

    public void SetTestimonies(Testimony[] testimonies)
    {
        this.Testimonies = testimonies;
    }
    
    private Testimony[] Testimonies
    {
        set 
        {
            this.ClearChildren();
            foreach (var testimony in value)
            {
                var testimonyDisplay = new Button();
                testimonyDisplay.Text = testimony.Statement.Text;
                testimonyDisplay.Disabled = true;
                AddChild(testimonyDisplay);
            }
        }
    }
}