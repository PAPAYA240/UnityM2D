using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Diagnostics;

using static Defines;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class Managers : MonoBehaviour
{
    // For. Instance
    public static Managers s_Instance = null;
    public static Managers Instance { get { return s_Instance; } }

    private static Input_Manager s_InputManager = new Input_Manager(); // 입력 매니저
    private static Resource_Manager s_ResourceManager = new Resource_Manager(); // 리소스 매니저
    private static Scene_Manager s_SceneManager = new Scene_Manager();  
    private static UI_Manager s_UIManager = new UI_Manager();
    private static Transform_Manager s_TransformManager = new Transform_Manager();
    private static Timer_Manager s_TimerManager = new Timer_Manager();
    public static WeaponLoader s_WeaponLoader = new WeaponLoader();
    public static WeaponLoader Weapon { get { return s_WeaponLoader; } }
    public static TurnManager s_TurnManager = new TurnManager();   
    public static ObjectPool_Manager s_ObjectPoolManager = new ObjectPool_Manager();

    // For. Getter Setter
    public static Input_Manager Input {  get { return s_InputManager; } }   
    public static Resource_Manager Resource { get { return s_ResourceManager; } }
    public static Scene_Manager Scene { get { return s_SceneManager; } }
    public static UI_Manager UIManager { get { return s_UIManager; } }
    public static Transform_Manager TransformManager { get { return s_TransformManager; } }
    public static Timer_Manager TimerManager { get { return s_TimerManager; } }
    public static TurnManager TurnManager { get { return s_TurnManager;  } }      
    public static ObjectPool_Manager ObjectPoolManager { get {return s_ObjectPoolManager; } }

    void Start()
    {
        Init();
    }

    private static void Init()
    {
        if(s_Instance == null)
        {
            GameObject gameObject = GameObject.Find("@Managers");
            if (gameObject == null)
                gameObject = new GameObject { name = "@Managers" };

            s_Instance = Setting.GetOrAddComponent<Managers>(gameObject);
            DontDestroyOnLoad(gameObject);

            s_WeaponLoader.Init();
            Application.targetFrameRate = 60;
        }
    }

    private void Update()
    {
        s_TimerManager.UpdateTimer(Time.deltaTime);
    }

}
