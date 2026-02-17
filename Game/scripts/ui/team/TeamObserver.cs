using Godot;
using Lawfare.scripts.characters.lawyers;
using Lawfare.scripts.subject.quantities;

namespace Lawfare.scripts.ui.team;

public partial class TeamObserver : Node
{
    [Signal]
    public delegate void QuantitiesChangedEventHandler(Quantities quantities);
    
    public void SetTeam(Team team)
    {
        if (team == null) return;
        EmitSignalQuantitiesChanged(team.Quantities);
    }
}