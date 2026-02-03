using Godot;

namespace Lawfare.scripts.ui.toggles;

public partial class AnimationLoopToggle : Node
{
    private AnimationPlayer _parent;
    
    public void SetPlaying(bool isPlaying)
    {
        if (isPlaying)
        {
            _parent.Play();
        }
        else
        {
            _parent.Stop();
        }
    }
    
    public override void _Ready()
    {
        _parent = GetParent<AnimationPlayer>();
        _parent.Stop();
    }
    
}