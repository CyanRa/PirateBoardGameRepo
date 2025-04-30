using UnityEngine;
using UnityEngine.UI;

public class VictoryPanelBehaviour : MonoBehaviour
{
    public Button PlunderButton;
    public Button CaptureButton;
    public string decision;

    public FleetManager myFleet;



    public void ConfirmPlunder(){
        decision = "Plunder";
        myFleet.victoryDecisionMade = true;
    }
    public void ConfrimCapture(){
        decision = "Capture";
        myFleet.victoryDecisionMade = true;
    }
}
