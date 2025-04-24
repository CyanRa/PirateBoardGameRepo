using UnityEngine;

[CreateAssetMenu(fileName = "StormCalling", menuName = "Consumable/StormCalling", order = 1)]
public class StormCalling : Consumable
{       
    public override void UseConsumable(FleetManager userFleet){
        base.RemoveConsumable(userFleet);  
        userFleet.immuneToStorm = true;
        userFleet.StartCoroutine(userFleet.WaitForStormSelect());    
         
    }
}