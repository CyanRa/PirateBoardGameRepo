using UnityEngine;
using UnityEngine.UI;

public class IconBehaviour : MonoBehaviour
{
    public void DisplayColor(bool HasActions){
        if(HasActions){
            GetComponent<Image>().color = new Color32(0,255,0,255);
        }else{
            GetComponent<Image>().color = new Color32(0,0,0,255);
        }
    }

   
}
