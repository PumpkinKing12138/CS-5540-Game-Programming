using UnityEngine;
using UnityEngine.SceneManagement;

public class CenterLevelManager : MonoBehaviour
{
    public GameObject Door1;
    public GameObject Door2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Door1.GetComponent<DoorBehavior>().levelfinished == 1 && Door2.GetComponent<DoorBehavior>().levelfinished == 1)
        {
            Debug.Log("Success!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    static public void ManageLevel(int levelID)
    {
        SceneManager.LoadScene(levelID + 1);
    }
}
