using Godot;
using Lawfare.scripts.logic.effects;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject.quantities;

namespace Lawfare.scripts.logic.cards;

[GlobalClass]
public partial class Action : Resource, IAction
{
    [Export]
    public string Label { get; private set; }
    
    [Export]
    public Skill[] DicePools { get; private set; } = [];
    
    public bool Applies(GameEvent gameEvent)
    {
        return false;
    }

    public ChangeGroup[] Stage(GameEvent gameEvent)
    {
        throw new System.NotImplementedException();
    }
}