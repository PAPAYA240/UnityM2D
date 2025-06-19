using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_Manager : MonoBehaviour
{
    private Defines.Scene currentSceneType = Defines.Scene.None;
    public Defines.Scene CurrentSceneType
    {
        get
        {
            if (currentSceneType != Defines.Scene.None)
                return currentSceneType;
            return Defines.Scene.None;
        }
        set { currentSceneType = value; }
    }


    public void Init()
    {

    }

    
    public void ChangeScene(Defines.Scene type)
    {
        currentSceneType = type;

       // SceneManager.LoadScene(GetSceneName(type));
    }

    string GetSceneName(Defines.Scene type)
    {
        string name = System.Enum.GetName(typeof(Defines.Scene), type);
        char[] letters = name.ToLower().ToCharArray();
        letters[0] = char.ToUpper(letters[0]);
        return new string(letters);
    }
}
