using System;
using Alteruna;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ShipSpawnerBehaviour : AttributesSync
{
    private int spawnIndex;
    public Spawner mySpawner;
    private Alteruna.Avatar myAvatar;
    public Transform spawnPoint;
  
    [SynchronizableField] string _tempSpawnedShipName = "";
   
    void Start()
    {
        myAvatar = GetComponent<Alteruna.Avatar>();

        if(!myAvatar.IsMe)return;

        spawnIndex = 0;
        mySpawner = GameObject.Find("SpawnPool").GetComponent<Spawner>();
        
        
    }

    public void InitSpawnPoint(){
        spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint").GetComponent<Transform>();
    }
    
    public void SpawnShip(){       
       if(myAvatar.GetComponent<FleetManager>().myShips.Count < 5){           
            GameObject spawnedShip = mySpawner.Spawn(0, spawnPoint.position);
            spawnedShip.name = "Ship" + myAvatar.name + spawnIndex;  
            _tempSpawnedShipName = spawnedShip.name;        
            spawnedShip.transform.SetParent(myAvatar.transform);
            spawnedShip.transform.localScale += new Vector3(1,1,1);
            string shipColour = myAvatar.GetComponent<FleetManager>().fleetColour; 
            spawnedShip.GetComponent<Ship>().fleetsAvatar = myAvatar;          
            myAvatar.GetComponent<FleetManager>().AddShipToFleet(spawnedShip, false);
            
            
            BroadcastRemoteMethod("SynchSpawnedShip", spawnedShip.GetComponent<Ship>().myFleet.name,spawnIndex);        

            spawnIndex++;    
       }      
    }

    [SynchronizableMethod]
    public void SynchSpawnedShip(string _player, int _spawnIndex){
        
            GameObject _ownerPlayer = GameObject.Find(_player);   
            Alteruna.Avatar _avatar = _ownerPlayer.GetComponent<Alteruna.Avatar>();
            GameObject _ship = GameObject.Find("Ship(Clone)");
            if(_ship != null){  
                _ship.name = "Ship" + _avatar.name + _spawnIndex;          
                _ship.transform.SetParent(_avatar.transform);
                _ship.transform.localScale += new Vector3(1,1,1);
                _ship.GetComponent<Ship>().ChangeShipColour(_avatar.GetComponent<FleetManager>().fleetColour);
                Debug.Log("normal ships colour id is: " + _avatar.GetComponent<FleetManager>().fleetColour);
                string shipColour = _avatar.GetComponent<FleetManager>().fleetColour; 
            }             
    }
    
    public void SpawnFlagShip(){
       GameObject spawnedShip = mySpawner.Spawn(1, spawnPoint.position);
       spawnedShip.name = "FlagShip" + myAvatar.name;
       spawnedShip.transform.SetParent(myAvatar.transform);
       spawnedShip.transform.localScale += new Vector3(1,1,1);
       spawnedShip.GetComponent<Ship>().fleetsAvatar = myAvatar;
       string shipColour = myAvatar.GetComponent<FleetManager>().fleetColour;      
       spawnedShip.GetComponent<Ship>().ChangeShipColour(shipColour);
       myAvatar.GetComponent<FleetManager>().AddShipToFleet(spawnedShip, true);
       BroadcastRemoteMethod("SynchSpawnedFlagShip", spawnedShip.GetComponent<Ship>().myFleet.name);
       spawnIndex++;       
    }

    [SynchronizableMethod]
    public void SynchSpawnedFlagShip(string _player){

            GameObject _ownerPlayer = GameObject.Find(_player);   
            Alteruna.Avatar _avatar = _ownerPlayer.GetComponent<Alteruna.Avatar>();
            Alteruna.User _tempUser = _avatar.Multiplayer.GetUser();
            int _userIndex = _tempUser.Index+1;
            GameObject _ship;

            switch(_userIndex)
            {
                case 1:  _ship = GameObject.Find("FlagShipRed(Clone)"); break;
                case 2:  _ship = GameObject.Find("FlagShipRed(Clone)"); break;
                case 3:  _ship = GameObject.Find("FlagShipRed(Clone)"); break;
                case 4:  _ship = GameObject.Find("FlagShipRed(Clone)"); break;
                default: _ship = GameObject.Find("FlagShipRed(Clone)"); break;
            }
            
            if(_ship != null){  
                _ship.name = "FlagShip" + _avatar.name;          
                _ship.transform.SetParent(_avatar.transform);
                _ship.transform.localScale += new Vector3(1,1,1);
                _ship.GetComponent<Ship>().ChangeShipColour(_avatar.GetComponent<FleetManager>().fleetColour);
                string shipColour = _avatar.GetComponent<FleetManager>().fleetColour; 
            }             
    }
}
