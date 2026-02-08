using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Lawfare.scripts.board.factions;
using Lawfare.scripts.subject.quantities;

namespace Lawfare.scripts.subject.relations;

public partial class Relations : GodotObject, IAvailable
{
    private readonly Dictionary<(quantities.Property Property, board.factions.Faction Faction), int> _relations;

    public Relations()
    {
        _relations = new Dictionary<(Property, Faction), int>();
    }

    public Relations(IRelation[] relations)
    {
        var entries = relations.Select(r =>
            new KeyValuePair<(Property, Faction), int>((r.Property, r.Faction), r.Amount)
        );

        _relations = new Dictionary<(Property, Faction), int>(entries);
    }

    public IReadOnlyList<IRelation> All => _relations
        .Select(kv => new Relation
        {
            Property = kv.Key.Property,
            Faction = kv.Key.Faction,
            Amount = kv.Value
        })
        .ToList();

    public event Action<Relations> OnChange;

    public int Available(Property property)
    {
        return _relations
            .Where(kv => kv.Key.Property == property)
            .Sum(kv => kv.Value);
    }

    public int Get(Property property, Faction faction)
    {
        return _relations.TryGetValue((property, faction), out var value) ? value : 0;
    }

    public IReadOnlyCollection<IRelation> GetOfFaction(Faction faction)
    {
        return _relations
            .Where(kv => kv.Key.Faction == faction)
            .Select(kv => new Relation
            {
                Property = kv.Key.Property,
                Faction = kv.Key.Faction,
                Amount = kv.Value
            })
            .ToList();
    }

    public IReadOnlyCollection<IRelation> GetOfProperty(Property property)
    {
        return _relations
            .Where(kv => kv.Key.Property == property)
            .Select(kv => new Relation
            {
                Property = kv.Key.Property,
                Faction = kv.Key.Faction,
                Amount = kv.Value
            })
            .ToList();
    }

    public void Set(Property property, Faction faction, int amount)
    {
        _relations[(property, faction)] = amount;
        OnChange?.Invoke(this);
    }

    public int Add(Property property, Faction faction, int amount)
    {
        var key = (property, faction);

        if (!_relations.ContainsKey(key))
            _relations[key] = 0;

        var oldAmount = _relations[key];
        var newAmount = Math.Max(oldAmount + amount, property.Minimum);

        _relations[key] = newAmount;
        OnChange?.Invoke(this);

        return newAmount - oldAmount;
    }

    public Relations Clone()
    {
        var relationsArray = _relations
            .Select(kv => new Relation
            {
                Property = kv.Key.Property,
                Faction = kv.Key.Faction,
                Amount = kv.Value
            })
            .ToArray<IRelation>();

        return new Relations(relationsArray);
    }
}
