using Godot;
using Lawfare.scripts.characters;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;
using Lawfare.scripts.subject.quantities;
using Lawfare.scripts.subject.relations;

namespace Lawfare.scripts.ui.character;

public partial class CharacterObserver : Control
{
    [Signal]
    public delegate void CharacterChangeEventHandler(GodotObject character);
    
    [Signal]
    public delegate void ImageChangeEventHandler(Texture2D texture);
    [Signal]
    public delegate void PortraitChangeEventHandler(Texture2D texture);
    
    [Signal]
    public delegate void LabelChangedEventHandler(string label);
    
    [Signal]
    public delegate void QuantitiesChangedEventHandler(Quantities quantities);
    
    [Signal]
    public delegate void RelationsChangedEventHandler(Relations relations);
    
    [Signal]
    public delegate void MirrorChangedEventHandler(bool mirror);
    
    [Signal]
    public delegate void OnLayoutDirectionChangedEventHandler(Window.LayoutDirection layoutDirection);
    
    [Signal]
    public delegate void OnOppositeLayoutDirectionChangedEventHandler(Window.LayoutDirection layoutDirection);
    
    [Signal]
    public delegate void OnFillModeChangedEventHandler(TextureProgressBar.FillModeEnum fillModeEnum);
    
    [Signal]
    public delegate void OnOppositeFillModeChangedEventHandler(TextureProgressBar.FillModeEnum fillModeEnum);
    
    [Signal]
    public delegate void CharacterClickedEventHandler(GodotObject character);
    
    [Signal]
    public delegate void TargetableChangedEventHandler(bool targetable);
    
    public void OnClicked(GodotObject character) => EmitSignalCharacterClicked(character);
    
    public void SetMirror(bool mirror) => Mirror = mirror;

    private bool _mirror = false;
    [Export]
    public virtual bool Mirror
    {
        get => _mirror;
        set
        {
            _mirror = value;
            EmitSignalMirrorChanged(value);
            EmitSignalOnLayoutDirectionChanged(value ? Window.LayoutDirection.Rtl : Window.LayoutDirection.Ltr);
            EmitSignalOnOppositeLayoutDirectionChanged(value ? Window.LayoutDirection.Ltr : Window.LayoutDirection.Rtl);
            
            EmitSignalOnFillModeChanged(value ? TextureProgressBar.FillModeEnum.RightToLeft : TextureProgressBar.FillModeEnum.LeftToRight);
            EmitSignalOnOppositeFillModeChanged(value ? TextureProgressBar.FillModeEnum.LeftToRight : TextureProgressBar.FillModeEnum.RightToLeft);
        }
    }
    
    public void SetCharacter(GodotObject character)
    {
        Character = character as ICharacter;
    }
    
    protected ICharacter _character;

    public virtual ICharacter Character
    {
        get => _character;
        set
        {
            _character = value;
            EmitSignalImageChange(value?.Image);
            EmitSignalPortraitChange(value?.Portrait);
            EmitSignalLabelChanged(value?.Label);
            EmitSignalQuantitiesChanged(value?.Quantities);
            EmitSignalRelationsChanged(value?.Relations);
            
            EmitSignalCharacterChange(value as GodotObject);
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