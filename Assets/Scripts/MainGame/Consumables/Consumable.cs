using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using Alteruna;
using NUnit.Framework;
using System.Linq;


public abstract class Consumable : ScriptableObject
{  
    public string consumableName = "";
    public string description = "";
    public string image = "MainGame/Interactables/Consumables";
    public int consumableIndex;
    public FleetManager userFleet;
    public abstract void UseConsumable(FleetManager userFleet);
    public virtual void RemoveConsumable(FleetManager _fleet){

        foreach(Consumable consumable in _fleet.myInventory.myConsumables.ToList()){
            if (consumable.consumableIndex == consumableIndex){
                _fleet.myInventory.myConsumables.Remove(consumable);
            }
        }
    }
}



[CreateAssetMenu(fileName = "TributeToTheOldGods", menuName = "Consumable/TributeToTheOldGods", order = 1)]
public class TributeToTheOldGods : Consumable
{
    public override void UseConsumable(FleetManager userFleet){
        Debug.Log("Tribute to The Old Gods");
    }
}





[CreateAssetMenu(fileName = "TalesOfTheFlagShip", menuName = "Consumable/TalesOfTheFlagShip", order = 1)]
public class TalesOfTheFlagship : Consumable
{    public override void UseConsumable(FleetManager userFleet){
       Debug.Log("Tales of The Flagship");
       
    }
}

[CreateAssetMenu(fileName = "PirateAlliance", menuName = "Consumable/PirateAlliance", order = 1)]
public class PirateAlliance : Consumable
{
    public override void UseConsumable(FleetManager userFleet){
       Debug.Log("Pirate Alliance");       
    }
}

