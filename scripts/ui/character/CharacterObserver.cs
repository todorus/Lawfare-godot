using System.Linq;
using Godot;
using Lawfare.scripts.characters;
using Lawfare.scripts.logic.cards;

namespace Lawfare.scripts.ui.character;

public partial class CharacterObserver : Node
{
    [Signal]
    public delegate void ImageChangeEventHandler(Texture2D texture);
    [Signal]
    public delegate void LabelChangedEventHandler(string label);
    
    [Signal]
    public delegate void MirrorChangedEventHandler(bool mirror);
    
    [Signal]
    public delegate void CharacterClickedEventHandler(GodotObject character);
    
    [Signal]
    public delegate void HandChangedEventHandler(Card[] cards);

    public bool Mirror
    {
        set
        {
            EmitSignalMirrorChanged(value);
        }
    }
    
    private ICharacter _character;

    public ICharacter Character
    {
        set
        {
            _character = value;
            EmitSignalImageChange(value?.Image);
            EmitSignalLabelChanged(value?.Label);
        }
    }
    
    public void OnClicked()
    {
        EmitSignalCharacterClicked(_character as GodotObject);
        EmitSignalHandChanged(_character.Actions.Select(action => new Card(action)).ToArray());
    }
}