using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour {
    float timer;
    public float left;
    float seconds;
    float minutes;
    private Text _timeLeft;
    public float timeBound = 600;
    public float timeExit = 20;
	// Use this for initialization
	void Start () {
        if (PlayerPrefs.GetString("mode").Contains("endless"))
        {
            this.gameObject.SetActive(false);
        }
        timer = 0;
        if (PlayerPrefs.GetInt("debug") == 1)
        {
            timeBound = 60;
            timeExit = 10;
        }
        _timeLeft = this.GetComponentInChildren<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        left = timeBound - timer;
        seconds = left % 60;
        minutes = left / 60;
        if(seconds > 10)
            _timeLeft.text = (int)minutes + ":" + (int)seconds;
        else _timeLeft.text = (int)minutes + ":0" + (int)seconds;
        if(left <= 0)
        {
            var bobber = GameObject.FindWithTag("Bobber");
            var weight = bobber.GetComponent<Bobber>().GetStatisticWeight();
            var sum = 0f;
            foreach (var fish in weight)
            {
                sum += fish;
            }
            _timeLeft.fontSize = 230;
            _timeLeft.text = "КОНЕЦ\nПоймано рыб: " + weight.Count + "\nОбщий вес: " + Math.Round(sum, 2);
            timer += Time.deltaTime;
            if(timer > timeBound + timeExit)
            {
                SceneManager.LoadScene("menu_scene", LoadSceneMode.Single);
            }
        }
        else
        {
            _timeLeft.fontSize = 400;
            timer += Time.deltaTime;
        }
    }
}
