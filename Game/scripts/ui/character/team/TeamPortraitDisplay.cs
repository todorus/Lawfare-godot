using System.Collections.Generic;
using Godot;
using Lawfare.scripts.characters.lawyers;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.ui.character.portrait;

namespace Lawfare.scripts.ui.character.team;

public partial class TeamPortraitDisplay : Container
{
    
    [Export]
    PackedScene _characterScene;
    
    [Export]
    private bool _mirror = false;
    
    [Signal]
    public delegate void CharacterClickedEventHandler(Lawyer character);
    
    private List<PortraitDisplay> _displays = new();
    private List<CharacterObserver> _characterObservers = new();

    public void SetActiveAction(GameEventDto dto)
    {
        foreach (var characterObserver in _characterObservers)
        {
            characterObserver.UpdateCanTarget(dto.GameEvent);
        }
    }
    
    public void SetTeam(Team team)
    {
        Team = team;
    }
    
    public Team Team
    {
        set
        {
            this.ClearChildren();
            _characterObservers.Clear();
            
            foreach (var display in _displays)
            {
                display.CharacterClicked -= OnCharacterClicked;   
            }
            _displays.Clear();
            
            if (value == null) return;
            
            foreach (var member in value.Members)
            {
                var display = _characterScene.Instantiate<PortraitDisplay>();
                display.CharacterClicked += OnCharacterClicked;
                var characterObserver = display.CharacterObserver;
                characterObserver.Character = member;
                characterObserver.Mirror = _mirror;
                AddChild(display);
                _characterObservers.Add(characterObserver);
                _displays.Add(display);
            }
        }
    }

    private void OnCharacterClicked(Lawyer character)
    {
        EmitSignalCharacterClicked(character);
    }
}