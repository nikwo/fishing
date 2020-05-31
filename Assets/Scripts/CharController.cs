using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CharController : MonoBehaviour {
    private GameObject _hand;
    private Animation _animation;
    private RaycastHit _raycast;
    private GameObject _fishing_rod;
    public int btn_selected;
    public bool grabed;
    
    private int dist = 10;
    void Start () {
        _fishing_rod = GameObject.FindGameObjectWithTag("Tool");
        _animation = this.GetComponentInChildren<Animation>();
        _hand = GameObject.FindGameObjectWithTag("Hand");
        grabed = false;
        btn_selected = -1;
    }
	
	void Update () {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if(Physics.Raycast(ray, out _raycast, dist))
        {
            if((Input.GetMouseButtonDown(0) || Input.touchCount > 0) && _raycast.transform.CompareTag("Tool"))
            {
                if (!grabed)
                {
                    _animation.Play("grab");
                    _hand.transform.localPosition = new Vector3(0.74f, -0.54f, 0.93f);
                    _hand.transform.localEulerAngles = new Vector3(-45f, 187.52f, 70.35f);
                    _fishing_rod.GetComponent<MeshCollider>().isTrigger = true;
                    _fishing_rod.transform.SetParent(_hand.transform);
                    _fishing_rod.transform.localPosition = new Vector3(0.9799998f, -0.04132181f, 0.07999812f);
                    _fishing_rod.transform.localEulerAngles = new Vector3(1.898f, -93.65601f, -267.718f);
                    _fishing_rod.GetComponent<Rigidbody>().useGravity = false;
                    _fishing_rod.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    _fishing_rod.GetComponent<Rigidbody>().isKinematic = true;
                    _fishing_rod.GetComponent<Rigidbody>().freezeRotation = true;
                    grabed = true;
                }
            }
            else if((Input.GetButtonDown("Fire1") || Input.touchCount > 0) && grabed && !this.GetComponentInChildren<BaitChoice>().choosing())
            {
                _animation.Play("ungrab");
                _fishing_rod.GetComponent<Rigidbody>().freezeRotation = false;
                _fishing_rod.transform.SetParent(null);
                _fishing_rod.GetComponent<MeshCollider>().isTrigger = false;
                _fishing_rod.GetComponent<Rigidbody>().useGravity = true;
                _fishing_rod.GetComponent<Rigidbody>().isKinematic = false;
                grabed = false;
            }
        }
        if (Input.GetMouseButtonDown(0) && btn_selected >= 0 && btn_selected < 3)
        {
            this.GetComponentInChildren<BaitChoice>().baitType = btn_selected;
        }
        
    }

    public bool get_flag()
    {
        return grabed;
    }

}
