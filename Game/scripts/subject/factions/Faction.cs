using Godot;
using Lawfare.scripts.info;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.board.factions;

[GlobalClass]
public partial class Faction : Resource, IInfo
{
    // [Export]
    // public Planner[] Planners = [];

    public readonly Controlled Controlled = new();

    [Export] 
    public Texture2D Icon { get; private set; }

    [Export] 
    public string Label { get; private set; }

    public void Add(ISubject subject)
    {
        Controlled.Add(subject);
    }

    public void Remove(ISubject subject)
    {
        Controlled.Remove(subject);
    }
}