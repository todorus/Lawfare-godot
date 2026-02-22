using Godot;
using Lawfare.scripts.characters.lawyers;

namespace Lawfare.scripts.ui.character;

public partial class SpeakerObserver : CharacterObserver
{
    public void SetTeam(Team team)
    {
        Team = team;
    }

    public Team Team { get; set; }
    
    public void OnSpeakerChange(Team team, Lawyer speaker)
    {
        if(team != Team) return;
        Character = speaker;
    }
}