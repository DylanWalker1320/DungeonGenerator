using UnityEngine;

public class AttackCountComponent : WeaponComponent
{
    Player player;
    int attackCount = 0;
    const int attackCountTarget = 10;
    public AttackCountComponent(Player player) :
        base(new WeaponComponentInfo("AttackDouble", "Your 10th attack does double damage"))
    {
        this.player = player;
    }

    public override void Apply()
    {
        player.SubscribeToAttack(OnPlayerAttack);
    }
    public override void Unapply()
    {
        attackCount = 0;
        player.UnsubscribeToAttack(OnPlayerAttack);
    }

    private void OnPlayerAttack(AttackInfo atkInfo)
    {
        attackCount++;
        if (attackCount == attackCountTarget)
        {
            Debug.Log($"Component activated! Doing double damage!");
            atkInfo.postDmgMultiplier *= 2;
            attackCount = 0;
        }
    }
}
