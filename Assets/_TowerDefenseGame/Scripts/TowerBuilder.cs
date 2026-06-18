using UnityEngine;

public class TowerBuilder : MonoBehaviour
{
    [System.Serializable]
    public class Tower
    {
        public string name;
        public GameObject prefab;
        public int cost;
    }

    public Tower[] towers;
    int selectedTowerIndex;
    bool selectedTower = false;

    public static TowerBuilder Instance {get; private set;}

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // SelectTower(1);
    }

    public void SelectTower(int index)
    {
        if (index >= 0 && index < towers.Length)
        {
            selectedTowerIndex = index;
            selectedTower = true;
        }
        else
        {
            Debug.Log("Out of range tower index");
            selectedTower = false;
        }
    }

    public GameObject GetSelectTowerPrefab()
    {
        return towers[selectedTowerIndex].prefab;
    }

    public int GetSelectTowerCost()
    {
        return towers[selectedTowerIndex].cost;
    }

    public int GetSelectTowerCost(int index)
    {
        return towers[index].cost;
    }

    public bool HasSelectedTower()
    {
        return selectedTower;
    }

    public void ClearSelection()
    {
        selectedTower = false;
        selectedTowerIndex = -1;
    }
}
