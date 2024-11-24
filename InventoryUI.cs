using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    Inventory inven;

    public GameObject inventoryPanel;
    bool activeInventory = false;

    public Slot[] slots;
    public Transform slotHolder;
    private void Start()
    {
        slots = slotHolder.GetComponentsInChildren<Slot>();
        inventoryPanel.SetActive(activeInventory);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) {
            activeInventory = !activeInventory;
            inventoryPanel.SetActive(activeInventory);
        }
    }
}
