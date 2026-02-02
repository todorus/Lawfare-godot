using Godot;
using Lawfare.scripts.@case;

namespace Lawfare.scripts.ui.testimony;

public partial class TestimonyDisplay : Button
{
    private Testimony _testimony;
    public Testimony Testimony 
    {
        get => _testimony;
        set
        {
            _testimony = value;
            Text = _testimony.Statement.Text;
        }
    }
}