using System.Collections.Generic;
using NUnit.Framework.Internal;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using NUnit.Framework;
using Alteruna;

public class ConsumableBehaviour : MonoBehaviour
{
public Consumable myConsumable;
public TextMeshProUGUI _name;
public TextMeshProUGUI description;
public Image image;
[SerializeField]private Button myButton;
[SerializeField]private Button consumableIcon;
[SerializeField]private Button closeInspectorButton;
Multiplayer MultiplayerSystem;

    public void Start(){
        MultiplayerSystem = GameObject.Find("Multiplayer").GetComponent<Multiplayer>();
    }
    public void LoadConsumableInspector(Consumable consumable){
        
        _name.text = consumable.name;
        description.text = consumable.description;
        image.sprite = Resources.Load<Sprite>(consumable.image);
        myButton.onClick.RemoveAllListeners();
        myButton.onClick.AddListener(() => consumable.UseConsumable(MultiplayerSystem.GetAvatar().GetComponent<FleetManager>()));
        myButton.onClick.AddListener(() => CloseConsumableInspector());
        myButton.onClick.AddListener(() => CloseAll());
    }

    public void CloseConsumableInspector(){
        myButton.onClick.RemoveAllListeners();
        gameObject.SetActive(false);
    }
    private void CloseAll(){
        ClearChildren();
        transform.parent.gameObject.SetActive(false);
    }
    private void ClearChildren(){
        foreach(Transform child in transform.parent.GetChild(0)){
            Destroy(child.gameObject);
        }
    }

}
