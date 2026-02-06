using System.Collections.Generic;
using Godot;
using Lawfare.scripts.context;
using Lawfare.scripts.logic.cards;
using Lawfare.scripts.logic.@event;
using Team = Lawfare.scripts.characters.lawyers.Team;

namespace Lawfare.scripts.ui.character;

public partial class TeamDisplay : Container
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
            if (value == null) return;
            
            foreach (var member in value.Members)
            {
                var characterObserver = _characterScene.Instantiate<CharacterObserver>();
                characterObserver.Character = member;
                characterObserver.Mirror = _mirror;
                characterObserver.CharacterClicked += OnCharacterClicked;
                AddChild(characterObserver);
                _characterObservers.Add(characterObserver);
            }
        }
    }
    
    public void OnCharacterClicked(GodotObject character)
    {
        EmitSignalCharacterClicked(character);
    }
}