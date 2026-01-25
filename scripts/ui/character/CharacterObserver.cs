using Godot;
using Lawfare.scripts.characters;

namespace Lawfare.scripts.ui.character;

public partial class CharacterObserver : Node
{
    [Signal]
    public delegate void ImageChangeEventHandler(Texture2D texture);
    [Signal]
    public delegate void LabelChangedEventHandler(string label);
    
    [Signal]
    public delegate void MirrorChangedEventHandler(bool mirror);

    public bool Mirror
    {
        set
        {
            EmitSignalMirrorChanged(value);
        }
    }

    public ICharacter Character
    {
        set
        {
            EmitSignalImageChange(value?.Image);
            EmitSignalLabelChanged(value?.Label);
        }
    }
}