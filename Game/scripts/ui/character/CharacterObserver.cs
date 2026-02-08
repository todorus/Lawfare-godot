using Godot;
using Lawfare.scripts.characters;
using Lawfare.scripts.context;
using Lawfare.scripts.logic.cards;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;
using Lawfare.scripts.subject.quantities;
using Lawfare.scripts.subject.relations;

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
    public delegate void TargetableChangedEventHandler(bool targetable);

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
        }
    }
    
    public void OnClicked()
    {
        EmitSignalCharacterClicked(_character as GodotObject);
    }

    public void UpdateCanTarget(GameEvent gameEvent)
    {
        var canTarget = gameEvent.Action?.CanTarget(gameEvent, _character as ISubject) ?? true;
        EmitSignalTargetableChanged(canTarget);
    }
}