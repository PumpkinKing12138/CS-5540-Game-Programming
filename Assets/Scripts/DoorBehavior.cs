using UnityEngine;

public class DoorBehavior : MonoBehaviour
{
    public int levelfinished = 0;
    public int ID;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //levelfinished = PlayerPrefs.GetInt("levelfinished", 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (levelfinished == 1)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            levelfinished = 1;
            PlayerPrefs.SetInt("levelfinished", levelfinished);
            CenterLevelManager.ManageLevel(ID);
        }
    }
}
