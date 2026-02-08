using System;
using System.Collections.Generic;
using Godot;

namespace Lawfare.scripts.board.factions;

public class Allegiances
{
    private readonly HashSet<Faction> _allegiances;

    public Allegiances()
    {
        _allegiances = new HashSet<Faction>();
    }

    public Allegiances(Faction[] factions)
    {
        _allegiances = new HashSet<Faction>(factions);
        Primary = factions.Length > 0
            ? factions[0]
            : null;
    }

    public IReadOnlyCollection<Faction> All => _allegiances;
    public Faction Primary { get; private set; }

    public event Action<Allegiances> OnChange;
    public event Action<Faction> OnAdd;
    public event Action<Faction> OnRemove;

    public void Add(Faction faction)
    {
        _allegiances.Add(faction);
        if (Primary == null) Primary = faction;
        OnChange?.Invoke(this);
        OnAdd?.Invoke(faction);
    }

    public void Remove(Faction faction)
    {
        if (Primary == faction)
        {
            GD.PushWarning("Trying to remove primary allegiance. Set a new primary before removing.");
            return;
        }

        _allegiances.Remove(faction);
        OnChange?.Invoke(this);
        OnRemove?.Invoke(faction);
    }

    public bool Contains(Faction faction)
    {
        return _allegiances.Contains(faction);
    }

    public bool Allowed(Faction faction)
    {
        return _allegiances.Count == 0 || _allegiances.Contains(faction);
    }

    public void ClearNonPrimaries()
    {
        var toRemove = new List<Faction>();
        foreach (var faction in _allegiances)
            if (faction != Primary)
                toRemove.Add(faction);

        foreach (var faction in toRemove)
        {
            _allegiances.Remove(faction);
            OnRemove?.Invoke(faction);
        }

        if (toRemove.Count > 0) OnChange?.Invoke(this);
    }
}