using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Dreamteck;
using static UnityEngine.UI.Image;



public class CrewMember : MonoBehaviour
{
    public string name;
    public string image;
    public int power;

    public bool isTutor; //still need to make isTutor logic
    public bool isSelected;
    public bool isCommited;
    
    public Hand handScript;

    public void Start()
    {
        isSelected = false;
        isCommited = false;
    }

    public void Update()
    {
        //clciking on cards selects them
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null)
            {
                hit.transform.GetComponent<CrewMember>().SelectThisCrew();
            }
        }
    }

    //method for select cards, still need to add actual highlighting of cards.
    public void SelectThisCrew()
    {
        if (!isSelected) 
        {
            isSelected = true;
            Debug.Log("is Selected");
        }
        else
        {
            isSelected = false;
            Debug.Log("is Deselected");
        }
    }

    //method for commiting this card
    public void CommitCrewToBattle()
    {
        if (isSelected)
        {
            isSelected = false;
            isCommited = true;
            //handScript.CommitCard(GetComponent<CrewMember>());
        }
    }

}
