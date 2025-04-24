using UnityEngine;

[CreateAssetMenu(fileName = "GreekFire", menuName = "Consumable/GreekFire", order = 1)]
public class GreekFire : Consumable
{
    public override void UseConsumable(FleetManager userFleet){
        foreach(GameObject ship in userFleet.myShips){
            if(ship.GetComponent<Ship>().usingGreekFire == true){
                ship.GetComponent<Ship>().occupyingMapPiece.DestroyAllShips();
                base.RemoveConsumable(userFleet); 
                return;
            }
        }
       Debug.Log("Can only be used when attacked");      
    }
}