using Godot;

namespace Lawfare.scripts.ui.character;

public partial class GrayscaleControl : Node
{
    [Export] 
    private Control _control;
    
    private ShaderMaterial _shaderMaterial;
    
    public override void _Ready()
    {
        _shaderMaterial = (ShaderMaterial) _control.Material;
    }

    public void SetGrayscale(float grayscale)
    {
        _shaderMaterial.SetShaderParameter("grayscale", grayscale);
    }
    
    public void SetEnabled(bool enabled)
    {
        SetGrayscale(enabled ? 1f : 0f);
    }
    
    public void SetDisabled(bool disabled)
    {
        SetEnabled(!disabled);
    }
}