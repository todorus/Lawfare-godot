using Godot;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.board.factions;

[GlobalClass]
public partial class Faction : Resource
{
    // [Export]
    // public Planner[] Planners = [];

    public readonly Controlled Controlled = new();

    [Export] public Texture2D Icon;

    [Export] public string Label;

    public void Add(ISubject subject)
    {
        Controlled.Add(subject);
    }

    public void Remove(ISubject subject)
    {
        Controlled.Remove(subject);
    }
}