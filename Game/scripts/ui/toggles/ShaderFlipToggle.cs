using Godot;

namespace Lawfare.scripts.ui.toggles;

public partial class ShaderFlipToggle : Node
{
    private ShaderMaterial _shaderMaterial;
    
    private bool _isFlipped;

    public bool IsFlipped
    {
        get => _isFlipped;
        set
        {
            _isFlipped = value;
            if (_shaderMaterial == null) return;
            _shaderMaterial.SetShaderParameter("flip_h", value);
        }
    }

    public void SetFlipped(bool isFlipped)
    {
        IsFlipped = isFlipped;
    }
    
    public override void _Ready()
    {
        var parent = GetParent();
        
        if (parent is Sprite2D sprite2D)
        {
            _shaderMaterial = sprite2D.Material as ShaderMaterial;
        }
        if (parent is Control ctrl)
        {
            _shaderMaterial = ctrl.Material as ShaderMaterial;
        }
        
        IsFlipped = _isFlipped;
    }
    
}