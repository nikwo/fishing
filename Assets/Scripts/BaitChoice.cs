using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum baits
{
    worm = 0,
    maggot = 1,
    bloodworm = 2
}


public class BaitChoice : MonoBehaviour {
    
    private GameObject _cam;
    private GameObject _player;
    private bool _choiceProcessing;
    private GameObject _ui;
    private Button[] _buttons;
    private Button okBtn;
    public int baitType;

	void Start () {
        _choiceProcessing = false;
        _ui = GameObject.Find("Canvas");
        _cam = GameObject.FindGameObjectWithTag("MainCamera");
        _buttons = FindObjectsOfType<Button>();
        baitType = (int)baits.worm;
        this.transform.LookAt(transform.position + _cam.transform.rotation * Vector3.forward, _cam.transform.rotation * Vector3.up);
        this.transform.position = _cam.transform.position + _cam.transform.TransformDirection(new Vector3(-0.05f, 0, -3));
        foreach (Button btn in _buttons)
        {
            if (btn)
            {
                if (btn.name.Contains("OkBtn"))
                {
                    okBtn = btn;
                    okBtn.onClick.AddListener(delegate () { closeMenu(); });
                }
                if (btn.name.Contains("Button_1"))
                {
                    btn.onClick.AddListener(delegate() { baitType = (int)baits.worm; } );
                }
                if (btn.name.Contains("Button_2"))
                {
                    btn.onClick.AddListener(delegate () { baitType = (int)baits.maggot; });
                }
                if (btn.name.Contains("Button_3"))
                {
                    btn.onClick.AddListener(delegate () { baitType = (int)baits.bloodworm; });
                }
            }
        }
        this.closeMenu();
    }

    public int GetBait()
    {
        return baitType;
    }
    
	// Update is called once per frame
	void Update () {
        
        if (GameObject.FindGameObjectWithTag("Player").GetComponent<CharController>().get_flag())
        {
            openMenu();
        }
        if (!_choiceProcessing)
        {
            _ui.transform.LookAt(transform.position + _cam.transform.rotation * Vector3.forward, _cam.transform.rotation * Vector3.up);
            _ui.transform.position = _cam.transform.position + _cam.transform.TransformDirection(new Vector3(-0.05f, 0, -3));
        }
        
    }

    private void openMenu()
    {
       
        if (!_choiceProcessing)
        {
            _choiceProcessing = true;
            _ui.transform.LookAt(transform.position + _cam.transform.rotation * Vector3.forward, _cam.transform.rotation * Vector3.up);
            _ui.transform.position = _cam.transform.position + _cam.transform.TransformDirection(new Vector3(-0.05f, 0, 1));
        }
    }

    private void closeMenu()
    {
        _choiceProcessing = false;
        _ui.transform.LookAt(transform.position + _cam.transform.rotation * Vector3.forward, _cam.transform.rotation * Vector3.up);
        _ui.transform.position = _cam.transform.position + _cam.transform.TransformDirection(new Vector3(-0.05f, 0, -3));
    }

    public bool choosing()
    {
        return _choiceProcessing;
    }
}
