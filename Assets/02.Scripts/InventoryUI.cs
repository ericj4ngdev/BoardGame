using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("Key")] 
    [SerializeField] private KeyCode keyCodeInvectory = KeyCode.E;
    public GameObject inventoryPanel;
    bool activeInventory = false;
    
    void Start()
    {
        inventoryPanel.SetActive(activeInventory);
    }
    void Update()
    {
        if (Input.GetKeyDown(keyCodeInvectory))
        {
            activeInventory = !activeInventory;
            inventoryPanel.SetActive(activeInventory);
        }
    }
}
