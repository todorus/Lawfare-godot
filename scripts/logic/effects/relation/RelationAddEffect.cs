using System.Collections.Generic;
using Godot;
using Lawfare.scripts.board.factions;
using Lawfare.scripts.logic.effects.property.amounts;
using Lawfare.scripts.logic.effects.relation.faction;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.logic.modifiers;
using Lawfare.scripts.subject;
using Lawfare.scripts.subject.quantities;

namespace Lawfare.scripts.logic.effects.relation;

[GlobalClass]
public partial class RelationAddEffect : RelationEffect
{
    
    [Export]
    public AmountProvider AmountProvider;

    [Export]
    public FactionProvider FactionProvider;
    
    protected override IChange[] StageInternal(GameEvent gameEvent, ISubject subject)
    {
        var faction = FactionProvider.GetFaction(gameEvent, subject);
        var amount = AmountProvider.GetAmount(gameEvent, subject);
        return [new RelationAddChange(subject, Property, faction, amount)];
    }
    
    public class RelationAddChange(ISubject subject, Property property, Faction faction, int amount) : IChange
    {
        private ISubject _subject = subject;
        private Property _property = property;
        private Faction _faction = faction;
        private int _amount = amount;
        
        public IReadOnlyList<IModification> Modifications { get; } = [];
        public IChange Apply()
        {
            if (_subject?.Relations == null)
            {
                GD.PushWarning("RelationAddChange.Apply: Subject or Subject.Relations is null");
                return this;
            }

            var actualChange = _subject.Relations.Add(_property, _faction, _amount);
            return new RelationAddChange(_subject, _property, _faction, actualChange);
        }
    }
}