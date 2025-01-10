using Alteruna;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ShipSpawnerBehaviour : MonoBehaviour
{
    private int spawnIndex;
    public Spawner mySpawner;
    private Alteruna.Avatar myAvatar;
    public Transform spawnPoint;
  
   
    void Start()
    {
        spawnIndex = 0;
        mySpawner = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<Spawner>();
        spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint").GetComponent<Transform>();
        myAvatar = GetComponent<Alteruna.Avatar>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!myAvatar.IsMe){
            return;
        }

        if(Input.GetKeyDown(KeyCode.F) && myAvatar.GetComponent<FleetManager>().myShips.Count < 5){
            SpawnShip();
        }
    }
    
    public void SpawnShip(){       
       if(myAvatar.GetComponent<FleetManager>().myShips.Count < 5){           
            GameObject spawnedShip = mySpawner.Spawn(0, spawnPoint.position);
            spawnedShip.name = "Ship" + myAvatar.name + spawnIndex;          
            spawnedShip.transform.SetParent(myAvatar.transform);
            spawnedShip.transform.localScale += new Vector3(1,1,1);
            myAvatar.GetComponent<FleetManager>().GetColourID();
            int shipColourID = myAvatar.GetComponent<FleetManager>().fleetColourID;           
            myAvatar.GetComponent<FleetManager>().AddShipToFleet(spawnedShip, false);
            spawnIndex++;
       }      
    }
    
    public void SpawnFlagShip(){
       GameObject spawnedShip = mySpawner.Spawn(1, spawnPoint.position);
       spawnedShip.name = "FlagShip" + myAvatar.name;
       spawnedShip.transform.SetParent(myAvatar.transform);
       spawnedShip.transform.localScale += new Vector3(1,1,1);
       int shipColourID = myAvatar.GetComponent<FleetManager>().fleetColourID;
       spawnedShip.GetComponent<Ship>().ChangeShipColour(shipColourID);
       myAvatar.GetComponent<FleetManager>().AddShipToFleet(spawnedShip, true);
       spawnIndex++;       
    }
}
