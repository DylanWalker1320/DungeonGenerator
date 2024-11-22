using UnityEngine;

public class SpeedIncreaseComponent : WeaponComponent
{
    Player player;
    const float SPEED_CHANGE = 2.0f;
    public SpeedIncreaseComponent(Player player) :
        base(new WeaponComponentInfo("SpeedUp", "Increase speed by 2"))
    {
        this.player = player;
    }

    public override void Apply()
    {
        player.IncreaseSpeed(SPEED_CHANGE);
    }
    public override void Unapply()
    {
        player.IncreaseSpeed(-SPEED_CHANGE);
    }
}
