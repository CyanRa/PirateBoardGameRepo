using System.Collections.Generic;
using NUnit.Framework.Internal;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using NUnit.Framework;

public class ConsumableBehaviour : MonoBehaviour
{
  public Consumable myConsumable;
  public TextMeshProUGUI name;
  public TextMeshProUGUI description;
  public Image image;
  [SerializeField]private Button myButton;
  [SerializeField]private Button consumableIcon;
  [SerializeField]private Button closeInspectorButton;
   

    public void LoadConsumableInspector(Consumable consumable){
        
        name.text = consumable.name;
        description.text = consumable.description;
        image.sprite = Resources.Load<Sprite>(consumable.image);
        myButton.onClick.RemoveAllListeners();
        myButton.onClick.AddListener(() => consumable.UseConsumable(consumable.userFleet));
    }

    public void CloseConsumableInspector(){
        myButton.onClick.RemoveAllListeners();
        gameObject.SetActive(false);
    }

}
