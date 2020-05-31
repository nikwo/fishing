using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class OptionMenuController : MonoBehaviour {
    public Button back;
    public Toggle[] checkboxes;
    public int btn_selected;
    // Use this for initialization
    void Start () {
        back.onClick.AddListener(delegate() { SceneManager.LoadScene("menu_scene", LoadSceneMode.Single); });
        btn_selected = -1;
        checkboxes[0].isOn = Convert.ToBoolean(PlayerPrefs.GetInt("sound"));
        checkboxes[1].isOn = Convert.ToBoolean(PlayerPrefs.GetInt("debug"));
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0) && btn_selected >= 7)
        {
            switch (btn_selected)
            {
                case 7:
                    if (checkboxes[0].isOn)
                    {
                        checkboxes[0].isOn = false;
                        PlayerPrefs.SetInt("sound", 0);
                    }
                    else
                    {
                        checkboxes[0].isOn = true;
                        PlayerPrefs.SetInt("sound", 1);
                    }
                    break;
                case 8:
                    if (checkboxes[1].isOn)
                    {
                        checkboxes[1].isOn = false;
                        PlayerPrefs.SetInt("debug", 0);
                    }
                    else
                    {
                        checkboxes[1].isOn = true;
                        PlayerPrefs.SetInt("debug", 1);
                    }
                    break;
                case 9:
                    SceneManager.LoadScene("menu_scene", LoadSceneMode.Single);
                    break;
            }
        }

    }
}
