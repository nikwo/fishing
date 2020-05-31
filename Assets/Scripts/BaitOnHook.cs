using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaitOnHook : MonoBehaviour
{
	private GameObject _worm;
	private GameObject _maggot;
	private GameObject _bloodworm;

	private int _baitType = -1;
	private GameObject _bait;
	// Use this for initialization
	void Start () {
		_worm = Resources.Load("Prefabs/worm", typeof(GameObject)) as GameObject;
		_maggot = Resources.Load("Prefabs/maggot", typeof(GameObject)) as GameObject;
		_bloodworm = Resources.Load("Prefabs/bloodworm", typeof(GameObject)) as GameObject;
	}
	
	// Update is called once per frame
	void Update () {
		var baitType = GameObject.FindGameObjectWithTag("UI").GetComponent<BaitChoice>().GetBait();;
		if (baitType != _baitType)
		{
			if (baitType == 0)
			{
				Destroy(GameObject.FindWithTag("Bait"));
				_bait = Instantiate(_worm);
			}
			if (baitType == 1)
			{
				Destroy(GameObject.FindWithTag("Bait"));
				_bait = Instantiate(_maggot);
			}
			if (baitType == 2)
			{
				Destroy(GameObject.FindWithTag("Bait"));
				_bait = Instantiate(_bloodworm);
			}
			_baitType = baitType;
		}
	}
}
