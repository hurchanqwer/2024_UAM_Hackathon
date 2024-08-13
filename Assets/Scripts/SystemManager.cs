using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SystemManager : MonoBehaviour
{
    public Camera MainCamera;
    public Transform UAM;
    public VectorFieldManager vectorFieldManager;
    public GameObject MainScene, EditScene;
    private static SystemManager instance;
    public static SystemManager Instance {  get { return instance; } }
    public bool isVisiable = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

    
    }
   
    private void Start()
    {
    }

    private void Update()
    {
        if (Input.anyKey)
        {
            if (Input.GetKeyDown(KeyCode.Space)){
                switch(MainScene.activeSelf)
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

            if(SceneManager.GetActiveScene().name == "Main"){
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {

                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    vectorFieldManager.VectorVisiableMode();
                }
               
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {

            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {

            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                if (transform.parent == null)
                {
                    MainCamera.transform.parent = UAM;
                    MainCamera.transform.localPosition = new Vector3(0, 2, -4);
                    MainCamera.transform.localRotation = Quaternion.Euler(13, 0, 0);
                }
                else { MainCamera.transform.parent = null; }
            }

        }

    }
}
