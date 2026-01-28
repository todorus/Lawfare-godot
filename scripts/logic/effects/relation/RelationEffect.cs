using Godot;
using Lawfare.scripts.subject.quantities;

namespace Lawfare.scripts.logic.effects.relation;

[GlobalClass]
public abstract partial class RelationEffect : Effect
{
    [Export] 
    public Property Property;
    
    
}