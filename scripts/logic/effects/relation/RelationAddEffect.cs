using System;
using Godot;
using Lawfare.scripts.logic.effects.property.amounts;
using Lawfare.scripts.logic.effects.relation.faction;
using Lawfare.scripts.logic.@event;
using Lawfare.scripts.subject;
using Lawfare.scripts.subject.relations;

namespace Lawfare.scripts.logic.effects.relation;

[GlobalClass]
public partial class RelationAddEffect : RelationEffect
{
    
    [Export]
    public AmountProvider AmountProvider;

    [Export]
    public FactionProvider FactionProvider;
    
    protected override IDiff[] StageInternal(GameEvent gameEvent, ISubject subject)
    {
        var faction = FactionProvider.GetFaction(gameEvent, subject);
        var amount = AmountProvider.GetAmount(gameEvent, subject);
        // return [new RelationAddChange(subject, Property, faction, amount)];
        
        if (subject?.Relations == null)
        {
            var zeroRelation = new Relation
            {
                Faction = faction,
                Property = Property,
                Amount = 0
            };
            return [new RelationAddDiff(subject, zeroRelation, zeroRelation)];
        }

        var original = new Relation
        {
            Property = Property,
            Faction = faction,
            Amount = subject.Relations.Get(Property, faction)
        };
            
        subject.Relations.Add(Property, faction, amount);
            
        var updated = new Relation
        {
            Property = Property,
            Faction = faction,
            Amount = subject.Relations.Get(Property, faction)
        };
            
        return [new RelationAddDiff(subject, original, updated)];
    }
        
    public class RelationAddDiff(ISubject subject, Relation original, Relation updated) : IDiff<Relation>
    {
        public ISubject Subject { get; } = subject;
        public Relation Original { get; } = original;
        public Relation Updated { get; } = updated;

        public bool CanMerge(IDiff<Relation> other) => CanMerge(other);

        public bool CanMerge(IDiff other)
        {
            return other is RelationAddDiff otherDiff &&
                   otherDiff.Subject == Subject &&
                   otherDiff.Original.Property == Original.Property &&
                   otherDiff.Original.Faction == Original.Faction;
        }

        public IDiff<Relation> Merge(IDiff<Relation> other) => (IDiff<Relation>) Merge((IDiff) other);
        
        public IDiff Merge(IDiff other)
        {
            if(other is not RelationAddDiff otherDiff)
            {
                throw new ArgumentException("Trying to merge incompatible diff types");
            }
            
            return new RelationAddDiff(
                Subject,
                Original,
                otherDiff.Updated
            );
        }
        
        public IDiff Apply()
        {
            var actualValue = Subject.Relations.Add(Updated.Property, Updated.Faction, Updated.Amount);
            var newUpdated = new Relation
            {
                Property = Updated.Property,
                Faction = Updated.Faction,
                Amount = actualValue
            };
            return new RelationAddDiff(Subject, Original, newUpdated);
        }
    }
}