using System.Collections.Generic;
using Godot;
using Lawfare.scripts.characters.lawyers;
using Lawfare.scripts.logic.@event;

namespace Lawfare.scripts.ui.character.team;

public partial class TeamPortraitDisplay : Control
{
    
    [Export]
    PackedScene _characterScene;
    
    [Export]
    private bool _mirror = false;
    
    [Signal]
    public delegate void CharacterClickedEventHandler(Lawyer character);
    
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
            foreach (var display in _characterObservers)
            {
                display.CharacterClicked -= OnCharacterClicked;   
            }
            _characterObservers.Clear();
            this.ClearChildren();
            
            if (value == null) return;
            
            foreach (var member in value.Members)
            {
                var characterObserver = _characterScene.Instantiate<CharacterObserver>();
                characterObserver.CharacterClicked += OnCharacterClicked;
                characterObserver.Character = member;
                characterObserver.Mirror = _mirror;
                AddChild(characterObserver);
                _characterObservers.Add(characterObserver);
            }
        }
    }

    private void OnCharacterClicked(GodotObject character)
    {
        EmitSignalCharacterClicked(character as Lawyer);
    }
}