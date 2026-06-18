using UnityEngine;

public class TileBehavior : MonoBehaviour
{
    public Material highlightMat;
    public GameObject towerPrefab;
    Renderer _renderer;
    Material originalMat;
    bool tileTower = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _renderer = GetComponent<Renderer>();
        originalMat = _renderer.material;
    }

    void OnMouseOver()
    {
        if (!TowerBuilder.Instance.HasSelectedTower())
            return;
        HighLight();
    }

    void OnMouseExit()
    {
        if (!TowerBuilder.Instance.HasSelectedTower())
            return;
        if (!tileTower)
            _renderer.sharedMaterial = originalMat;
    }

    void OnMouseDown()
    {
        if (!tileTower)
        {
            if (TowerBuilder.Instance.HasSelectedTower())
            {
                int towerCost = TowerBuilder.Instance.GetSelectTowerCost();
                if (MoneyManager.Instance.BuyTower(towerCost))
                {
                    GameObject towerPrefab = TowerBuilder.Instance.GetSelectTowerPrefab();
                    var tower = Instantiate(towerPrefab, transform.parent.position, transform.parent.rotation);
                    tileTower = true;
                    TowerBuilder.Instance.ClearSelection();
                }
                else
                {
                    Debug.LogWarning("No enough Money");
                }
            }
        }
    }

    void HighLight()
    {
        if (highlightMat)
            _renderer.sharedMaterial = highlightMat;
    }
}
