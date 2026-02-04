using System.Linq;
using Godot;
using Lawfare.scripts.characters;
using Lawfare.scripts.characters.lawyers;
using Lawfare.scripts.logic.cards;
using Lawfare.scripts.subject.quantities;
using Lawfare.scripts.subject.relations;
using Ult = Lawfare.scripts.characters.ult.Ult;

namespace Lawfare.scripts.ui.character;

public partial class CharacterObserver : Node
{
    [Signal]
    public delegate void ImageChangeEventHandler(Texture2D texture);
    [Signal]
    public delegate void LabelChangedEventHandler(string label);
    
    [Signal]
    public delegate void QuantitiesChangedEventHandler(Quantities quantities);
    
    [Signal]
    public delegate void RelationsChangedEventHandler(Relations relations);
    
    [Signal]
    public delegate void MirrorChangedEventHandler(bool mirror);
    
    [Signal]
    public delegate void CharacterClickedEventHandler(GodotObject character);
    
    [Signal]
    public delegate void UltChangedEventHandler(Ult ult);

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
            EmitSignalQuantitiesChanged(value?.Quantities);
            EmitSignalRelationsChanged(value?.Relations);
            EmitSignalUltChanged(value?.Ult);
        }
    }
    
    public void OnClicked()
    {
        EmitSignalCharacterClicked(_character as GodotObject);
    }
}