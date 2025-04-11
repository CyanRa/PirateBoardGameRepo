using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public Transform InventoryPanel;
    public GameObject consumablePrefab;
    [SerializeField]List<Consumable> allConsumables;
    [SerializeField]List<Consumable> myConsumables;
    FleetManager myFleet;

    public void Awake()
    {
        myFleet = GetComponent<FleetManager>();
    }

    public void CreateConsumable(int _consumableIndex){
        myConsumables?.Add(allConsumables[_consumableIndex]);  
        GameObject tempConsumablePrefab = Instantiate(consumablePrefab);   
        tempConsumablePrefab.GetComponent<Button>().onClick.AddListener(() => allConsumables[_consumableIndex].UseConsumable(myFleet));
    }
    public void Update(){
    
        if(Input.GetKeyDown(KeyCode.K)){
            myConsumables[0].UseConsumable(myFleet);
        }
        
    }

}
