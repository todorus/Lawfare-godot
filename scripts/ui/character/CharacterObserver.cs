using Godot;
using Lawfare.scripts.characters;

namespace Lawfare.scripts.ui.character;

public partial class CharacterObserver : Node
{
    [Signal]
    public delegate void ImageChangeEventHandler(Texture2D texture);
    [Signal]
    public delegate void LabelChangedEventHandler(string label);

    [Export] 
    private Lawyer _startingCharacter;

    public ICharacter Character
    {
        set
        {
            EmitSignalImageChange(value?.Image);
            EmitSignalLabelChanged(value?.Label);
        }
    }

    public override void _Ready()
    {
        base._Ready();
        Character = _startingCharacter;
    }
}