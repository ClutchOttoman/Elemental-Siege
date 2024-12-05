using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{
#pragma warning disable 0649 //private variable
    [SerializeField] private string sceneName;
#pragma warning restore 0649 //private variable

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Enters a scene.
    public void enterScene(){
        Debug.Log("Entering " + sceneName + " level");
        if (PlayerDataManager.Instance == null) Debug.Log("Uh oh - Player Data Manager instance is null.");
        PlayerDataManager.Instance.transitionStatus = true;
        Debug.Log("PlayerDataManager.Instance.transitionStatus = " + PlayerDataManager.Instance.transitionStatus);
        SceneManager.LoadScene(sceneName);
    }
}
