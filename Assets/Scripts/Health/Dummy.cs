public sealed class Dummy : Health
{
    public override void Die(DamageArgs args) { }

    private void OnDrawGizmos()
    {
        foreach (var zone in damageZones)
        {
            zone.DrawGizmos();
        }
    }
}
