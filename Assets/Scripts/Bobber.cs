using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

struct Fish
{
	public GameObject Prefab;
	public string FishName;
	public float WeightStart;
	public float WeightEnd;
	public float StartDepth;
	public float EndDepth;
	public float StartTimer;
	public float EndTimer;
	public int BaitType;
}

struct StatFish
{
	public string Name;
	public float Weight;
}

public class Bobber : MonoBehaviour {
	public float bobberWaterHeight;
	public GameObject bobber;
	public GameObject water;
	public GameObject terrain;
	public GameObject rodEnd;
	public float rotSmoothing = 0.5f;
	public float moveSmoothing = 0.01f;
	// used for Lerp animation
	private float _rotationTimer;
	private float _moveTimer;
	//
	private float _depth;
	public bool debug = false;
	// the length of sharp movement required to catch fish on the hook
	private float _catchMovement;
	// flags
	private bool _inWater = false;
	private bool _bobberIn = false;
	private bool _beforeCast = true;
	private bool _inCast = false;
	private bool _waitingFish = false;
	private bool _onHook = false;
	private bool _catch = false;
	private bool _gotcha = false;
	private bool _gotcha_after = false;
	private bool _onHookTmpFlag = false;
	// timers
	private float _waitingTimer;
	private float _onHookTimer;
	private float _afterLossTimer = 0;
	private Vector3 _rodEndLastPos;
	private Vector3 _bobberPos;
	// fish prefab
	private Fish[] _fishes = new Fish[3];
	// fish
	private GameObject _fish;
	private int _fishPrefab = -1;
	// 
	private float _fishingLineLength;
	private float _baitLineLength;
	// 
	public float reelSpeed = 1f;
	private AudioClip _smallSplash;
	private AudioClip _bobberOnHook;
	private AudioClip _reelSlash;
	private AudioClip _reelRotation;
	private AudioSource audioSourceBobber;

	private List<StatFish> _stats = new List<StatFish>();
	
	private void GenFishes()
	{
		Fish okun;
		okun.Prefab = Resources.Load("Prefabs/okunHigh", typeof(GameObject)) as GameObject;
		okun.WeightEnd = 4;
		okun.WeightStart = 2;
		okun.FishName = "Окунь";
		okun.StartDepth = 4;
		okun.EndDepth = 8;
		okun.StartTimer = 20;
		okun.EndTimer = 30;
		okun.BaitType = 2;
		_fishes[2] = okun;
		Fish som;
		som.Prefab = Resources.Load("Prefabs/SomHigh1", typeof(GameObject)) as GameObject;
		som.WeightEnd = 2;
		som.WeightStart = 1;
		som.FishName = "Сом";
		som.StartDepth = 7;
		som.EndDepth = 10;
		som.StartTimer = 50;
		som.EndTimer = 100;
		som.BaitType = 1;
		_fishes[1] = som;
		Fish crucian;
		crucian.Prefab = Resources.Load("Prefabs/crucianHigh", typeof(GameObject)) as GameObject;
		crucian.WeightStart = 0.5f;
		crucian.WeightEnd = 1;
		crucian.FishName = "Карась";
		crucian.StartDepth = 1;
		crucian.EndDepth = 6;
		crucian.StartTimer = 50;
		crucian.EndTimer = 100;
		crucian.BaitType = 0;
		_fishes[0] = crucian;
	}
	
	void Start () {
		water = GameObject.FindGameObjectWithTag ("Water");
		terrain = GameObject.FindGameObjectWithTag ("Terrain");
		rodEnd = GameObject.FindGameObjectWithTag ("Tool").transform.GetChild(0).gameObject;
		bobber = Resources.Load("Prefabs/bobber", typeof(GameObject)) as GameObject;
		_rodEndLastPos = rodEnd.transform.position;
		GenFishes();
		bobber = GameObject.FindGameObjectWithTag ("Bobber");
		audioSourceBobber = bobber.GetComponent<AudioSource>();
		_smallSplash = Resources.Load("Sounds/small_splash", typeof(AudioClip)) as AudioClip;
		_bobberOnHook = Resources.Load("Sounds/bobber_onHook", typeof(AudioClip)) as AudioClip;
		_reelSlash = Resources.Load("Sounds/reel_slash", typeof(AudioClip)) as AudioClip;
		_reelRotation = Resources.Load("Sounds/reelRotation", typeof(AudioClip)) as AudioClip;
		_smallSplash.LoadAudioData();
		_bobberOnHook.LoadAudioData();
		_reelSlash.LoadAudioData();
		_reelRotation.LoadAudioData();
		var audioSourceRod = rodEnd.GetComponent<AudioSource>();
		audioSourceRod.volume = 0.3f;
		audioSourceRod.clip = _reelRotation;
		// GameObject.Find("Timer").SetActive(false);
		if (PlayerPrefs.GetInt("debug") == 1)
		{
			debug = true;
		}
		else
		{
			debug = false;
		}
	}

	void Update()
	{
		var timer = GameObject.Find("Timer");
		bool time;
		if (!timer)
		{
			time = true;
		}
		else
		{
			time = timer.GetComponent<Timer>().left > 0;
		}
		
		if (_beforeCast)
		{
			if (Input.GetKeyDown(KeyCode.Z) && !_bobberIn && GameObject.FindGameObjectWithTag("Player").GetComponent<CharController>().get_flag())
			{
				_bobberIn = true;
				if (bobber.GetComponent<FixedJoint>())
				{
					Destroy(bobber.GetComponent<FixedJoint>());
				}
				bobber.GetComponent<Rigidbody>().useGravity = true;
				var fishLine = bobber.AddComponent<SpringJoint>();
				fishLine.autoConfigureConnectedAnchor = false;
				fishLine.connectedAnchor = rodEnd.transform.position;
				fishLine.maxDistance = 0.5f;
				fishLine.minDistance = 0.8f;
				fishLine.spring = 1.5f;
				bobber.GetComponent<Collider>().isTrigger = false;
			}
			if (_bobberIn)
			{
				var rodEndPos = rodEnd.transform.position;
				bobber.GetComponent<SpringJoint>().connectedAnchor = rodEndPos;
				RotateBobber();
				CheckWaterCollision();
			}
			if (_bobberIn && Input.GetKeyDown(KeyCode.X) && time)
			{
				var audioSourceRod = rodEnd.GetComponent<AudioSource>();
				audioSourceRod.PlayOneShot(_reelSlash);
				Destroy(bobber.GetComponent<SpringJoint>());
				_beforeCast = false;
				_inCast = true;
			}
		}
		else if (_inCast)
		{
			var coil = rodEnd.transform.parent.Find("reel").Find("coil");
			var crot = coil.localRotation.eulerAngles;
			crot.y -= 60;
			coil.localRotation = Quaternion.Euler(crot);
			CheckWaterCollision();
		}
		else if (_inWater)
		{
			PauseRotation();
			audioSourceBobber.PlayOneShot(_smallSplash);
			_bobberPos = bobber.transform.position;
			_bobberPos.y = water.transform.position.y - bobberWaterHeight;
			bobber.transform.position = _bobberPos;
			RaycastHit hit;
			Physics.Raycast(_bobberPos, Vector3.down, out hit, Mathf.Infinity);
			_depth = hit.distance;
			if (_depth > 1)
			{
				List<int> variants = new List<int>();
				for (var i = 0; i < _fishes.Length; i++)
				{
					var fish = _fishes[i];
					variants.Add(i);
					if (_depth >= fish.StartDepth && _depth <= fish.EndDepth)
					{
						variants.Add(i);
						variants.Add(i);
						variants.Add(i);
					}
					if (fish.BaitType == GameObject.FindGameObjectWithTag("UI").GetComponent<BaitChoice>().GetBait())
					{
						variants.Add(i);
						variants.Add(i);
						variants.Add(i);
						variants.Add(i);
						variants.Add(i);
					}
				}
				_fishPrefab = RandomChoiceFish(variants);
				_waitingTimer = Random.Range(_fishes[_fishPrefab].StartTimer, _fishes[_fishPrefab].EndTimer);
				if (debug)
				{
					_waitingTimer = 10;
				}
				_onHookTimer = Random.Range(3f, 10f);
				_inWater = false;
				_waitingFish = true;
				_catchMovement = 0.2f;
				_rotationTimer = 0;
			}
			else
			{
				_inWater = false;
			}
		}
		else if (_waitingFish)
		{
			var y = _bobberPos.y;
			if (Input.GetKey(KeyCode.Z))
			{
				var coil = rodEnd.transform.parent.Find("reel").Find("coil");
				var crot = coil.localRotation.eulerAngles;
				crot.y += 60;
				coil.localRotation = Quaternion.Euler(crot);
				_bobberPos = bobber.transform.position;
				RaycastHit hit;
				Physics.Raycast(_bobberPos, Vector3.down, out hit, Mathf.Infinity);
				_depth = hit.distance;
				if (_depth > 1f)
				{
					List<int> variants = new List<int>();
					for (var i = 0; i < _fishes.Length; i++)
					{
						var fish = _fishes[i];
						variants.Add(i);
						if (_depth >= fish.StartDepth && _depth <= fish.EndDepth)
						{
							variants.Add(i);
							variants.Add(i);
						}
						if (fish.BaitType == GameObject.FindGameObjectWithTag("UI").GetComponent<BaitChoice>().GetBait())
						{
							variants.Add(i);
							variants.Add(i);
							variants.Add(i);
							variants.Add(i);
							variants.Add(i);
						}
					}
					_fishPrefab = RandomChoiceFish(variants);
					_waitingTimer = Random.Range(_fishes[_fishPrefab].StartTimer, _fishes[_fishPrefab].EndTimer);
					_rodEndLastPos = rodEnd.transform.position;
					_bobberPos = _bobberPos - Time.deltaTime * reelSpeed * (_bobberPos - _rodEndLastPos);
					_bobberPos.y = y;
					bobber.transform.position = _bobberPos;
				}
				else
				{
					_waitingFish = false;
				}
			}
			if (Input.GetKeyDown(KeyCode.Z))
			{
				PlayRotation();
			}
			if (Input.GetKeyUp(KeyCode.Z))
			{
				PauseRotation();
			}
			var lookPos = Vector3.down;
			var rot = Quaternion.LookRotation(lookPos, Vector3.forward);
			bobber.transform.Rotate(0, 90, 0);
			if (rot != bobber.transform.rotation)
			{
				_rotationTimer += Time.deltaTime;
				bobber.transform.rotation =
					Quaternion.Lerp(bobber.transform.rotation, rot, _rotationTimer * rotSmoothing * 0.8f);
			}
			else
			{
				_rotationTimer = 1;
			}
			bobber.transform.Rotate(0, -90, 0);
	
			var pos = Mathf.Sin(2 * _waitingTimer) * 0.1f;
			var bobberNewPos = bobber.transform.position;
			bobberNewPos.y = water.transform.position.y - bobberWaterHeight + pos * 0.2f;
			bobber.transform.position = bobberNewPos;
			if (_waitingTimer < 0)
			{
				_waitingFish = false;
				_onHook = true;
				audioSourceBobber.clip = _bobberOnHook;
				audioSourceBobber.Play();
			}
			_waitingTimer -= Time.deltaTime;
			_rodEndLastPos = rodEnd.transform.position;
		}
		else if (_onHook)
		{
			var pos = (Mathf.Sin(5 * _onHookTimer) + Mathf.Sin(3 * _onHookTimer / 2)
			                                         + Mathf.Sin(6 * _onHookTimer / 2) +
			                                         Mathf.Cos(Mathf.Sin(3 * _onHookTimer) - 3) - 1) / 5;
			var bobberNewPos = bobber.transform.position;
			bobberNewPos.y = water.transform.position.y - bobberWaterHeight + pos * 0.2f;
			bobber.transform.position = bobberNewPos;
			var movement = Vector3.Distance(_rodEndLastPos, rodEnd.transform.position);
			_rodEndLastPos = rodEnd.transform.position;
			if (movement >= _catchMovement)
			{
				_fishingLineLength = Vector3.Distance(bobberNewPos, _rodEndLastPos);
				_catch = true;
				_onHook = false;
				var fishPrefab = _fishes[_fishPrefab].Prefab;
				var rot = fishPrefab.transform.rotation;
				var fishPos = bobberNewPos + (bobberNewPos-_rodEndLastPos).normalized;
				_fish = Instantiate(fishPrefab, fishPos, rot);
				var fishScale = Random.Range(_fishes[_fishPrefab].WeightStart, _fishes[_fishPrefab].WeightEnd);
				_fish.transform.localScale = new Vector3(fishScale, fishScale, fishScale);
				
				StatFish newFish = new StatFish();
				newFish.Name = _fishes[_fishPrefab].FishName;
				newFish.Weight = fishScale;
				_stats.Add(newFish);
				
				_baitLineLength =
					Vector3.Distance(bobberNewPos, _fish.transform.Find("mouth").position);
				audioSourceBobber.Stop();
				_onHookTmpFlag = true;
			}
			_onHookTimer -= Time.deltaTime;
			if (_onHookTimer < 0)
			{
				audioSourceBobber.Stop();
				_onHook = false;
			}
		} else if (_onHookTmpFlag)
		{
			PauseRotation();
			var bobberPos = bobber.transform.position;
			var fishPos = bobberPos + (bobberPos-rodEnd.transform.position).normalized;
			var tr = fishPos - _rodEndLastPos;
			tr.y = 0;
			_fish.GetComponent<FishBeh>().RotateY(-Vector3.Angle(tr, Vector3.right));
			_onHookTmpFlag = false;
		}
		else if (_catch)
		{
			RaycastHit hit;
			Physics.Raycast(bobber.transform.position, Vector3.down, out hit, Mathf.Infinity);
			_depth = hit.distance;
			if (_depth < 0.9f)
			{
				_catch = false;
				_gotcha = true;
			}
			else
			{
				var mouth = _fish.transform.Find("mouth");
				if (Input.GetKey(KeyCode.Z) || debug)
				{
					var coil = rodEnd.transform.parent.Find("reel").Find("coil");
					var crot = coil.localRotation.eulerAngles;
					crot.y += 60;
					coil.localRotation = Quaternion.Euler(crot);
					var delta = _fishingLineLength - reelSpeed / 10;
					_fishingLineLength = Mathf.Max(delta, 0.4f);
					var fishPos = _fish.transform.position;
					var y = fishPos.y;
					fishPos = fishPos - Time.deltaTime * reelSpeed * (fishPos - _rodEndLastPos);
					fishPos.y = y;
					_fish.transform.position = fishPos;
				}
				if (Input.GetKeyDown(KeyCode.Z))
				{
					PlayRotation();
				}
				if (Input.GetKeyUp(KeyCode.Z))
				{
					PauseRotation();
				}
				var bobberNewPos =
					_rodEndLastPos +
					(mouth.transform.position - _rodEndLastPos).normalized *
					(Vector3.Distance(mouth.transform.position, _rodEndLastPos) - 0.5f);
				bobberNewPos.y = water.transform.position.y;
				bobber.transform.position = bobberNewPos;
				var lookPos = (mouth.transform.position - bobber.transform.position).normalized;
				var rot = Quaternion.LookRotation(lookPos, Vector3.forward);
				bobber.transform.Rotate(0, 90, 0);
				if (rot != bobber.transform.rotation)
				{
					_rotationTimer += Time.deltaTime;
					bobber.transform.rotation =
						Quaternion.Lerp(bobber.transform.rotation, rot, _rotationTimer * rotSmoothing);
				}
				else
				{
					_rotationTimer = 0;
				}
				bobber.transform.Rotate(0, -90, 0);
				_rodEndLastPos = rodEnd.transform.position;
			}
		} else if (_gotcha)
		{
			_fish.GetComponent<Rigidbody>().useGravity = true;
			var baitline = _fish.AddComponent<SpringJoint>();
			baitline.autoConfigureConnectedAnchor = false;
			baitline.connectedAnchor = bobber.transform.position;
			baitline.maxDistance = 1.5f;
			baitline.minDistance = 1.5f;
			baitline.spring = 50f*_fish.GetComponent<Rigidbody>().mass;
			baitline.anchor = _fish.transform.Find("mouth").localPosition;
			_gotcha_after = true;
			_gotcha = false;
			PauseRotation();
		} else if (_gotcha_after)
		{
			PauseRotation();
			var mouth = _fish.transform.Find("mouth");
			var bobberNewPos =
				mouth.transform.position +
				(rodEnd.transform.position - mouth.transform.position).normalized * 1f;
			bobber.transform.position = bobberNewPos;
			RotateBobber();
			_fish.GetComponent<SpringJoint>().connectedAnchor = rodEnd.transform.position;
			if (Input.GetKey(KeyCode.X))
			{
				Destroy(_fish);
				_gotcha_after = false;
				_beforeCast = true;
				bobberNewPos.y = bobberNewPos.y - 1f;
				bobber.GetComponent<Rigidbody>().useGravity = true;
				var fishLine = bobber.AddComponent<SpringJoint>();
				fishLine.autoConfigureConnectedAnchor = false;
				fishLine.connectedAnchor = rodEnd.transform.position;
				fishLine.maxDistance = 1;
				fishLine.minDistance = 0.8f;
				fishLine.spring = 1.5f;
			}
		}
		else
		{
			if (Input.GetKey(KeyCode.Z))
			{
				var coil = rodEnd.transform.parent.Find("reel").Find("coil");
				var crot = coil.localRotation.eulerAngles;
				crot.y += 60;
				coil.localRotation = Quaternion.Euler(crot);
				_bobberPos = bobber.transform.position;
				_rodEndLastPos = rodEnd.transform.position;
				var y = _bobberPos.y;
				_bobberPos = _bobberPos - Time.deltaTime * reelSpeed * (_bobberPos - _rodEndLastPos);
				_bobberPos.y = y;
				bobber.transform.position = _bobberPos;
			}

			if (Input.GetKeyDown(KeyCode.Z))
			{
				PlayRotation();
			}
			if (Input.GetKeyUp(KeyCode.Z))
			{
				PauseRotation();
			}
			var pos = Mathf.Sin(2 * _afterLossTimer) * 0.1f;
			_afterLossTimer += Time.deltaTime;
			var bobberNewPos = bobber.transform.position;
			bobberNewPos.y = water.transform.position.y - bobberWaterHeight + pos * 0.2f;
			if (Vector3.Distance(_rodEndLastPos, bobberNewPos) < 3 && Input.GetKeyDown(KeyCode.X))
			{
				_fish = null;
				_beforeCast = true;
				bobberNewPos.y = bobberNewPos.y + 1f;
				bobber.GetComponent<Rigidbody>().useGravity = true;
				var fishLine = bobber.AddComponent<SpringJoint>();
				fishLine.autoConfigureConnectedAnchor = false;
				fishLine.connectedAnchor = rodEnd.transform.position;
				fishLine.maxDistance = 1;
				fishLine.minDistance = 0.8f;
				fishLine.spring = 1.5f;
				PauseRotation();
			}
			RotateBobber();
			bobber.transform.position = bobberNewPos;
			_rodEndLastPos = rodEnd.transform.position;
		}
		UpdateHookPos();
	}

	public float GetWaitingTimer()
	{
		return _waitingTimer;
	}
	
	public float GetOnHookTimer()
	{
		return _onHookTimer;
	}

	public float GetFishingLineLength()
	{
		return _fishingLineLength;
	}

	public float GetBaitLineLength()
	{
		return _baitLineLength;
	}

	
	private void PlayRotation()
	{
		var audioSourceRod = rodEnd.GetComponent<AudioSource>();
		audioSourceRod.Play();
	}

	private void PauseRotation()
	{
		var audioSourceRod = rodEnd.GetComponent<AudioSource>();
		audioSourceRod.Pause();
	}
	

	public int GetFlag()
	{
		if (_inWater)
		{
			return 2; // поплавок попал в воду и проверят глубину заброса
		}

		if (_beforeCast)
		{
			return 0; // леска не отпущена, поплавок не отправлен в воду
		}

		if (_inCast)
		{
			return 1; // поплавок летит в воду
		}

		if (_waitingFish)
		{
			return 3; // ожидание рыбы
		}

		if (_onHook)
		{
			return 4; // рыба питается наживкой
		} 
		
		if (_catch)
		{
			return 5; // рыба зацепилась за крючок
		}
		
		if (_gotcha)
		{
			return 6; // момент вытягивания рыбы
		}

		if (_gotcha_after)
		{
			return 7; // рыба вытянута
		}

		return -1;
	}

	public void CheckWaterCollision()
	{
		if (bobber.transform.position.y <= water.transform.position.y)
		{
			Destroy(bobber.GetComponent<SpringJoint>());
			_inWater = true;
			_inCast = false;
			_beforeCast = false;
			Rigidbody bobberRigBody = bobber.GetComponent<Rigidbody>();
			bobberRigBody.useGravity = false;
			bobberRigBody.velocity = Vector3.zero;
			bobberRigBody.angularVelocity = Vector3.zero;
		}
	}

	private int RandomChoiceFish(List<int> source)
	{
		
		var result = -1;
		if (source.Count > 0)
		{
			result = source[Random.Range(0, source.Count - 1)];
		}
		return result;
	}
	
	private void RotateBobber()
	{
		var lookPos = (bobber.transform.position - rodEnd.transform.position).normalized;
		var rot = Quaternion.LookRotation(lookPos, Vector3.forward);
		bobber.transform.Rotate(0, 90, 0);
		if (rot != bobber.transform.rotation)
		{
			_rotationTimer += Time.deltaTime;
			bobber.transform.rotation =
				Quaternion.Lerp(bobber.transform.rotation, rot, _rotationTimer * rotSmoothing);
		}
		else
		{
			_rotationTimer = 0;
		}
		bobber.transform.Rotate(0, -90, 0);
	}

	private void UpdateHookPos()
	{
		var rodEndPos = rodEnd.transform.position;
		var bobberPos = bobber.transform.position;
		var hook = GameObject.Find("hook");
		if (GetFlag() < 5)
		{
			if (_bobberIn)
			{
				var dist = bobberPos - rodEndPos;
				hook.transform.position = rodEndPos + dist.normalized * (dist.magnitude + 0.6f);
			}
			else
			{
				hook.transform.position = rodEnd.transform.parent.transform.Find("hook_pos").transform.position;
			}
		}
		else
		{
			if (_fish)
			{
				var mouth = _fish.transform.Find("mouth");
				hook.transform.position = mouth.position;
			}
		}
		hook.transform.rotation = Quaternion.Euler(89.98f, 0, 0);
		var lookPos = (bobberPos - rodEndPos).normalized;
		var rot = Quaternion.LookRotation(lookPos, Vector3.forward).eulerAngles;
		hook.transform.Rotate(rot);
		if (GameObject.FindWithTag("Bait")) {
			// GameObject.FindWithTag("Bait").transform.localRotation = Quaternion.Euler(-89.98f, 0, 0);
			GameObject.FindWithTag("Bait").transform.rotation = hook.transform.rotation;
			GameObject.FindWithTag("Bait").transform.Rotate(90, 0, 0);
			// GameObject.FindWithTag("Bait").transform.Rotate(rot);
			GameObject.FindWithTag("Bait").transform.position = hook.transform.position;
		}
	}

	public List<float> GetStatisticWeight()
	{
		List<float> tmp = new List<float>();
		foreach (var fish in _stats)
		{
			tmp.Add(fish.Weight);
		}
		return tmp;
	}
	
	public List<string> GetStatisticNames()
	{
		List<string> tmp = new List<string>();
		foreach (var fish in _stats)
		{
			tmp.Add(fish.Name);
		}
		return tmp;
	}
	
}

