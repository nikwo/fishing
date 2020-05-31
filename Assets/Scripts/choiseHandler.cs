using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class choiseHandler : MonoBehaviour, IPointerEnterHandler
{
    public bool isHightlighted;
    public int button_number;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button_number < 3)
        {
            GameObject.Find("GameObject").GetComponent<CharController>().btn_selected = button_number;
        }
        if (button_number >= 3 && button_number < 7)
        {
            GameObject.Find("Menu").GetComponent<MenuController>().btn_selected = button_number;
        }
        if(button_number >= 7 && button_number < 10)
        {
            GameObject.Find("OptionMenu").GetComponent<OptionMenuController>().btn_selected = button_number;
        }
    }

    // Use this for initialization
    void Start () {
        isHightlighted = false;
	}
	
	// Update is called once per frame
	void Update () {
        
    }
}
