using Godot;

namespace Lawfare.scripts.ui.initiative;

public partial class SlotDisplay : Control
{
    [Export] 
    private Texture2D _texture;
    [Export] 
    private Texture2D _staggeredTexture;
    
    [Signal]
    public delegate void BackgroundImageEventHandler(Texture2D texture);
    
    public bool IsStaggered
    {
        set
        {
            var texture = value ? _staggeredTexture : _texture;
            EmitSignalBackgroundImage(texture);
        }
    }
}