using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Lawfare.scripts.subject.quantities;

public partial class Quantities : GodotObject, IAvailable
{
    private readonly Dictionary<Property, int> _quantities;

    public Quantities()
    {
        _quantities = new Dictionary<Property, int>();
    }

    public Quantities(IQuantity[] quantities)
    {
        var entries =
            quantities.Select(quantity => new KeyValuePair<Property, int>(quantity.Property, quantity.Amount));
        _quantities = new Dictionary<Property, int>(entries);
    }

    public IReadOnlyList<IQuantity> All => _quantities
        .Select(kv => new Quantity { Property = kv.Key, Amount = kv.Value })
        .ToList();

    public int Available(Property property)
    {
        return Get(property);
    }

    public event Action<Quantities> OnChange;

    public IReadOnlyCollection<IQuantity> GetOfCategory(Category category)
    {
        return _quantities
            .Where(kv => kv.Key.Category == category)
            .Select(kv => new Quantity { Property = kv.Key, Amount = kv.Value })
            .ToList();
    }

    public void Set(Property property, int amount)
    {
        _quantities[property] = amount;
        OnChange?.Invoke(this);
    }

    public int Get(Property property)
    {
        return _quantities.ContainsKey(property) ? _quantities[property] : 0;
    }
    
    public int StageAdd(Property property, int amount)
    {
        if (!_quantities.ContainsKey(property))
        {
            _quantities[property] = 0;
        }
        
        return _quantities[property] + amount;
    }

    public int Add(Property property, int amount)
    {
        if (!_quantities.ContainsKey(property)) _quantities[property] = 0;

        var oldAmount = _quantities[property];
        var newAmount = Math.Max(_quantities[property] + amount, property.Minimum);
        ;
        _quantities[property] = newAmount;
        OnChange?.Invoke(this);
        return newAmount;
    }

    public Quantities Clone()
    {
        var quantitiesArray = _quantities
            .Select(kv => new Quantity { Property = kv.Key, Amount = kv.Value })
            .ToArray<IQuantity>();
        return new Quantities(quantitiesArray);
    }
}