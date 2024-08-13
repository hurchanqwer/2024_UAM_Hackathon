using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SystemManager : MonoBehaviour
{

    public VectorFieldManager vectorFieldManager;
    public GameObject MainScene, EditScene;
    private static SystemManager instance;
    public static SystemManager Instance { get { return instance; } }
    public bool isVisiable;

    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            
            switch (MainScene.activeSelf)
            {
                case true:
                    MainScene.SetActive(false);
                    EditScene.SetActive(true);
                    break;
                case false:
                    EditScene.SetActive(false);
                    MainScene.SetActive(true);
                    break;
            }

        }

    }

}

