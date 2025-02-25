using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Dreamteck;
using static UnityEngine.UI.Image;




public class CrewMember : MonoBehaviour
{
    public string _name;
    public string _image;
    public int _power;

    public bool isTutor;
    public bool isSelected;
    public bool isCommited;

    public void Start()
    {
        isSelected = false;
        isCommited = false;
    }

    public void Update()
    {
        Ray ray;
        RaycastHit hit;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (Input.GetMouseButtonDown(0))
                print(hit.collider.name);
        }
    }

    //unsure how to set booleans
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

    public void CommitCrewToBattle()
    {

    }


}
