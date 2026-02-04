using System;
using Godot;
using Lawfare.scripts.context;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.characters.ult;

[GlobalClass]
public abstract partial class Ult : Resource, IUlt
{
    public event Action<Ult> OnChange;
    public abstract void Charge(int amount);
    public abstract float Progress { get; }
    public abstract bool Active { get; }
    public abstract void Update(ISubject host, IContext context);

    protected void EmitOnChange()
    {
        OnChange?.Invoke(this);
    }
}