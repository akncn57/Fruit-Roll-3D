using System.IO;
using UnityEngine;

namespace Inventory
{
    public class InventoryService
    {
        private readonly string _path = Path.Combine(Application.persistentDataPath, "inventory.json");

        public void SaveInventory(InventoryData data)
        {
            try
            {
                var json = JsonUtility.ToJson(data, true);
                File.WriteAllText(_path, json);
                Debug.Log($"Inventory saved to: {_path}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to save inventory: {e.Message}");
            }
        }

        public InventoryData LoadInventory()
        {
            if (!File.Exists(_path))
            {
                Debug.Log("No save file found, creating new inventory.");
                var newData = new InventoryData();
                SaveInventory(newData); 
                return newData;
            }

            try
            {
                var json = File.ReadAllText(_path);
                var data = JsonUtility.FromJson<InventoryData>(json);
                return data ?? new InventoryData();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load inventory: {e.Message}");
                return new InventoryData();
            }
        }

        public void DeleteInventory()
        {
            if (File.Exists(_path))
            {
                File.Delete(_path);
            }
        }
    }
}