using Lawfare.scripts.context;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.characters.ult;

public partial class DummyUlt : Ult
{
    public override void Charge(int amount) { }

    public override float Progress => 0f;
    public override bool Active => false;
    public override void Update(ISubject host, IContext context)
    {
        EmitOnChange();
    }
}