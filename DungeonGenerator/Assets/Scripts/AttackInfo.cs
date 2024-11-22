using UnityEngine;

public class AttackInfo
{
    public int baseDmg;
    public int baseDmgMultiplier;
    public int dmgIncrement;
    public int postDmgMultiplier;

    public AttackInfo(int baseDmg) : this(baseDmg, 1, 0, 1)
    {
        this.baseDmg = baseDmg;
    }

    public AttackInfo(int baseDmg, int baseDmgMultiplier, int dmgIncrement, int postDmgMultiplier)
    {
        this.baseDmg = baseDmg;
        this.baseDmgMultiplier = baseDmgMultiplier;
        this.dmgIncrement = dmgIncrement;
        this.postDmgMultiplier = postDmgMultiplier;
    }

    public int TotalDamage()
    {
        return (baseDmg * baseDmgMultiplier + dmgIncrement) * postDmgMultiplier;
    }
}
