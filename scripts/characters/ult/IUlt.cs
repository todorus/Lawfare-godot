using Lawfare.scripts.context;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.characters.ult;

public interface IUlt
{
    public void Charge(int amount);
    public float Progress { get; }
    public bool Active { get; }

    public void Update(ISubject host, IContext context);
}