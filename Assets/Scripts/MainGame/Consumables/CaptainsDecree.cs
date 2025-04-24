using UnityEngine;


[CreateAssetMenu(fileName = "CaptainsDecree", menuName = "Consumable/CaptainsDecree", order = 1)]
public class CaptainsDecree : Consumable
{
    public override void UseConsumable(FleetManager userFleet){
        base.RemoveConsumable(userFleet);
       userFleet.GainFlagshipActionPoint();   
    }
}