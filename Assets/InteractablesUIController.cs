using NUnit.Framework.Constraints;
using UnityEngine;

public class InteractablesUIController : MonoBehaviour
{
    public GameObject panel;
    public void Hide()
    {
        if (panel.activeSelf)
        {
            panel.SetActive(false);
        }
        else
        {
            panel.SetActive(true);
        }

    }
}
