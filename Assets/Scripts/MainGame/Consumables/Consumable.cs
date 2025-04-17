using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using Alteruna;
using NUnit.Framework;


public abstract class Consumable : ScriptableObject
{  
    public string consumableName = "Storm Calling";
    public string description = " Your ships are immune to the storm this turn\n\n You may move the storm in any direction";
    public string image = "StormCallingImage";
    public int consumableIndex;
    public FleetManager userFleet;
    public abstract void UseConsumable(FleetManager userFleet);
}


[CreateAssetMenu(fileName = "StormCalling", menuName = "Consumable/StormCalling", order = 1)]
public class StormCalling : Consumable
{   
    
    public override void UseConsumable(FleetManager userFleet)
    {
        //Debug.Log("Waiting for fleet to select the storm to move");
        userFleet.myInventory.myConsumables.RemoveAt(1);
        userFleet.immuneToStorm = true;
        userFleet.StartCoroutine("WaitForStormSelect");       
    }
}
[CreateAssetMenu(fileName = "TributeToTheOldGods", menuName = "Consumable/TributeToTheOldGods", order = 1)]
public class TributeToTheOldGods : Consumable
{

    public override void UseConsumable(FleetManager userFleet)
    {
        Debug.Log("Tribute to The Old Gods");
    }
}

[CreateAssetMenu(fileName = "Passage", menuName = "Consumable/Passage", order = 1)]
public class Passage : Consumable
{
    public override void UseConsumable(FleetManager userFleet)
    {
        userFleet.myInventory.myConsumables.RemoveAt(2);
        userFleet.StartCoroutine(userFleet.WaitForMapPieceSelect());
        Debug.Log("PASSAGE USED");      
    }
}

[CreateAssetMenu(fileName = "GreekFire", menuName = "Consumable/GreekFire", order = 1)]
public class GreekFire : Consumable
{
    public override void UseConsumable(FleetManager userFleet)
    {
       Debug.Log("Greek Fire");      
    }
}

[CreateAssetMenu(fileName = "CaptainsDecree", menuName = "Consumable/CaptainsDecree", order = 1)]
public class CaptainsDecree : Consumable
{

    public override void UseConsumable(FleetManager userFleet)
    {
       Debug.Log("Captains Decree");       
    }
}


[CreateAssetMenu(fileName = "TalesOfTheFlagShip", menuName = "Consumable/TalesOfTheFlagShip", order = 1)]
public class TalesOfTheFlagship : Consumable
{    public override void UseConsumable(FleetManager userFleet)
    {
       Debug.Log("Tales of The Flagship");
       
    }
}

[CreateAssetMenu(fileName = "PirateAlliance", menuName = "Consumable/PirateAlliance", order = 1)]
public class PirateAlliance : Consumable
{
    public override void UseConsumable(FleetManager userFleet)
    {
       Debug.Log("Pirate Alliance");       
    }
}

[CreateAssetMenu(fileName = "Shipwright", menuName = "Consumable/Shipwright", order = 1)]
public class Shipwright : Consumable
{
    public override void UseConsumable(FleetManager userFleet)
    {
       
       Debug.Log("Shipwright");       
    }
}