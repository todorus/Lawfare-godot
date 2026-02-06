using System.Collections.Generic;
using System.Linq;
using Godot;
using Lawfare.scripts.characters;
using Lawfare.scripts.context;
using Lawfare.scripts.logic.cards;
using Lawfare.scripts.logic.@event;

namespace Lawfare.scripts.ui.character;

public partial class WitnessDisplay : Container
{
    [Signal]
    public delegate void CharacterClickedEventHandler(GodotObject character);
    
    [Export]
    PackedScene _characterScene;
    
    [Export]
    private bool _mirror = false;
    
    private List<CharacterObserver> _characterObservers = new();
    public void SetActiveAction(GameEventDto dto)
    {
        foreach (var characterObserver in _characterObservers)
        {
            characterObserver.UpdateCanTarget(dto.GameEvent);
        }
    }
    
    public void SetWitnesses(Witness[] witnesses)
    {
        Witnesses = witnesses;
    }
    
    public Witness[] Witnesses
    {
        set
        {
            this.ClearChildren();
            _characterObservers.Clear();
            if (value == null) return;
            
            foreach (var witness in value)
            {
                var characterObserver = _characterScene.Instantiate<CharacterObserver>();
                characterObserver.Character = witness;
                characterObserver.Mirror = _mirror;
                characterObserver.CharacterClicked += EmitSignalCharacterClicked;
                AddChild(characterObserver);
                _characterObservers.Add(characterObserver);
            }
        }
    }
}