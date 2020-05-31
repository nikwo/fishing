using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionManager : MonoBehaviour {

	// Use this for initialization
    
	void Start () {
        this.GetComponentInParent<Text>().text = "Выберите одну из наживок";    
	}
	
	// Update is called once per frame
	void Update () {
        int type = GetComponentInParent<BaitChoice>().baitType;

        if (type >= 0 && GetComponentInParent<BaitChoice>().choosing())
        {
            if(type == 0)
            {
                this.GetComponentInParent<Text>().text = "Наживка \"червяк\", подходит для ловли практически любой рыбы, но чаще ловятся караси";
            }
            if(type == 1)
            {
                this.GetComponentInParent<Text>().text = "Наживка \"опарыш\", идеальна для ловли сомов";
            }
            if(type == 2)
            {
                this.GetComponentInParent<Text>().text = "Наживка \"мотыль\", отлично подойдет для лова окуня";
            }
        }
	}
}
