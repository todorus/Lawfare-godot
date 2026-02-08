using System;
using System.Collections.Generic;
using Lawfare.scripts.logic.initiative.state;

namespace Lawfare.scripts.logic.initiative;

public sealed class InitiativeTrack : IInitiativeTrack
{
    public void Add(IHasInitiative entity, int delay = 0)
    {
        throw new NotImplementedException();
    }

    public bool Remove(IHasInitiative entity)
    {
        throw new NotImplementedException();
    }

    public void Move(int dt)
    {
        throw new NotImplementedException();
    }

    public void Stage(int dt)
    {
        throw new NotImplementedException();
    }

    public void ClearStaging()
    {
        throw new NotImplementedException();
    }

    public bool SetDelay(IHasInitiative entity, int newDelay)
    {
        throw new NotImplementedException();
    }

    public void CommitTurn(int cost)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyList<InitiativeSlot> Slots { get; }
    public IHasInitiative Current { get; }
    public IHasInitiative Next { get; }
    public int DelayToNext { get; }
    public bool IsStaging { get; }
    public event Action OnChange;
    public event Action<IReadOnlyList<InitiativeSlot>> SlotsChanged;
    public event Action<IHasInitiative> CurrentChanged;
    public event Action<IHasInitiative> NextChanged;
    public event Action<int> DelayToNextChanged;
    public event Action<int> Tick;
}