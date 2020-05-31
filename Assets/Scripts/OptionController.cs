using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionController : MonoBehaviour {
    public Toggle sound;
    public Toggle debugger;
    public Button back;
	// Use this for initialization
	void Start () {
        back.onClick.AddListener(delegate() { SceneManager.LoadScene("menu_scene", LoadSceneMode.Single); });
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
