using UnityEngine;

public class Mapcube : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Transform child in gameObject.transform)
        {
            //Destroy(child.GetChild(0).GetComponent<Renderer>().materials[0]);
            child.GetChild(0).localScale = new Vector3(0.01f,0.01f,0.01f);
           
        }
    }

}
