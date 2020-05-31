using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bobber_hook : MonoBehaviour {

    // Use this for initialization
    public GameObject bobber;
    public GameObject hook;
    private LineRenderer renderer;
    void Start () {
        renderer = GetComponent<LineRenderer>();
        bobber = GameObject.FindWithTag("Bobber");
        hook = GameObject.Find("hook");
    }
	
	// Update is called once per frame
	void Update () {
        draw();
	}

    private void draw()
    {
        Vector3 []positions = new Vector3[2];
        positions[0] = hook.transform.position;
        positions[1] = bobber.transform.position;
        renderer.positionCount = 2;
        renderer.SetPositions(positions);
    }
}
