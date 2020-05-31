using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishBeh : MonoBehaviour {

	private Transform _fishTransform;
	private Transform _mouth;
	private Transform _tail;

	private float _rotation=0;
	private bool _flag;
	private Vector3 _forward;
	// private Vector3 _right;
	private Vector3 _left;
	// private Vector3 _backward;
	private GameObject _bobber;
	private Vector3 _initial3DObjectRotation;
    private Animation _animation;
	void Start ()
	{
		_fishTransform = transform;
		_mouth = transform.Find("mouth");
		_tail = transform.Find("tail");
		_bobber = GameObject.FindGameObjectWithTag ("Bobber");
		_initial3DObjectRotation = _fishTransform.rotation.eulerAngles;
		GetComponent<Rigidbody>().centerOfMass = _tail.localPosition + (_mouth.localPosition - _tail.localPosition)/2;
		UpdateVectors();
        _animation = GetComponent<Animation>();
        _animation.Play("Swim");
	}
	
	void Update ()
	{
		var flag = _bobber.GetComponent<Bobber>().GetFlag();
        if(flag < 3 && flag != -1)
        {
            if (!_animation.IsPlaying("Swim"))      
                _animation.Play("Swim");
        }
		else if (flag == 5)
		{
            if(!_animation.IsPlaying("gettingCaught"))
                _animation.Play("gettingCaught");
			_fishTransform.position += (_left - _left * 0.5f) * -Mathf.Cos(_rotation) * Time.deltaTime;
			_fishTransform.Rotate(-_initial3DObjectRotation);
			_fishTransform.rotation = Quaternion.Euler(0, Mathf.Cos(_rotation) * 15, 0);
			_fishTransform.Rotate(_initial3DObjectRotation);
			_rotation += Time.deltaTime * 2;
		}
        else if(flag > 5)
        {
	        if (_flag)
	        {
		        _flag = !(_flag);
		        _fishTransform.GetComponent<Rigidbody>().AddForce(new Vector3(0, 0.00001f, 0));
	        }
	        else
	        {
		        _flag = !(_flag);
		        _fishTransform.GetComponent<Rigidbody>().AddForce(new Vector3(0, -0.00001f, 0));
	        }
            if (!_animation.IsPlaying("gettingCaught"))
                _animation.Play("gettingCaught");
        }
	}

	public void UpdateVectors()
	{
		_forward = (_mouth.position - _tail.position).normalized;
		_left = Quaternion.AngleAxis(-90, Vector3.up) * _forward;
	}

	public void RotateY(float angle)
	{
		_fishTransform.rotation = Quaternion.Euler(_initial3DObjectRotation);
		_fishTransform.Rotate(0, angle, 0);
		_initial3DObjectRotation = _fishTransform.rotation.eulerAngles;
	}
}
