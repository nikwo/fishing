using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public Button[] buttons;
    public int btn_selected;
    // Use this for initialization
    void Start()
    {
        buttons[0].onClick.AddListener(delegate () { Debug.Log("time attack"); SceneManager.LoadScene("main_scene", LoadSceneMode.Single); }); // на время
        buttons[1].onClick.AddListener(delegate () { Debug.Log("freeride"); SceneManager.LoadScene("main_scene", LoadSceneMode.Single); });
        buttons[2].onClick.AddListener(delegate () { Debug.Log("options"); SceneManager.LoadScene("options_scene", LoadSceneMode.Single); });
        buttons[3].onClick.AddListener(delegate () { Debug.Log("exit"); });
        btn_selected = -1;
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetMouseButtonDown(0) && btn_selected >= 3 && btn_selected < 7)
        {
            switch (btn_selected)
            {
                case 3:
                    SceneManager.LoadScene("main_scene", LoadSceneMode.Single);
                    PlayerPrefs.SetString("mode", "time attack");
                    break;
                case 4:
                    SceneManager.LoadScene("main_scene", LoadSceneMode.Single);
                    PlayerPrefs.SetString("mode", "endless");
                    break;
                case 5:
                    SceneManager.LoadScene("options_scene", LoadSceneMode.Single);
                    break;
                case 6:
                    Application.Quit();
                    break;
            }
        }
    }
}
