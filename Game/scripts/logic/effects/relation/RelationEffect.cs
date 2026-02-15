using Godot;
using Lawfare.scripts.subject.quantities;

namespace Lawfare.scripts.logic.effects.relation;

[GlobalClass]
public abstract partial class RelationEffect : EffectOld
{
    [Export] 
    public Property Property;
    
    
}