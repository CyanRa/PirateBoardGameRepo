using UnityEngine;

[CreateAssetMenu(fileName = "Shipwright", menuName = "Consumable/Shipwright", order = 1)]
public class Shipwright : Consumable
{
    public override void UseConsumable(FleetManager userFleet){
       base.RemoveConsumable(userFleet);
       userFleet.WaitForHarborSelect();
       Debug.Log("Shipwright");       
    }
}