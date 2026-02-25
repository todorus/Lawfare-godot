using Godot;

namespace Lawfare.scripts.ui.initiative;

public partial class SlotDisplay : Control
{
    [Signal]
    public delegate void BackgroundImageEventHandler(Texture2D texture);
    
    [Signal]
    public delegate void CurrentSlotChangedEventHandler(bool isCurrent);
    
    [Export] 
    private Texture2D _texture;
    [Export] 
    private Texture2D _staggeredTexture;
    
    public bool IsCurrent
    {
        set => EmitSignalCurrentSlotChanged(value);
    }
    
    public bool IsStaggered
    {
        set
        {
            var texture = value ? _staggeredTexture : _texture;
            EmitSignalBackgroundImage(texture);
        }
    }
}