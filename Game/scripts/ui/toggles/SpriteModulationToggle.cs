using Godot;

namespace Lawfare.scripts.ui.toggles;

public partial class SpriteModulationToggle : Node
{
    private Sprite3D _parent;
    
    [Export]
    private Color _passiveColor = Colors.White;
    
    [Export]
    private Color _activeColor = Colors.Gray;
    
    public void SetMode(bool active)
    {
        _parent.Modulate = active ? _activeColor : _passiveColor;
    }
    
    public override void _Ready()
    {
        _parent = GetParent<Sprite3D>();
    }
    
}