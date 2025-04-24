using UnityEngine;

[CreateAssetMenu(fileName = "Passage", menuName = "Consumable/Passage", order = 1)]
public class Passage : Consumable
{
    public override void UseConsumable(FleetManager userFleet){
        base.RemoveConsumable(userFleet); 
        userFleet.StartCoroutine(userFleet.WaitForMapPieceSelect());             
    }
}