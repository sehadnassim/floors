using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ApiResponse
{
    public float averageDoor;
    public List<string> classes;
    public List<BoundingBox> points;
}

[System.Serializable]
public class BoundingBox
{
    public float x1;
    public float y1;
    public float x2;
    public float y2;
}

public class FloorPlanLoader : MonoBehaviour
{
    private ApiResponse parsedData;
    private float scaleFactor = 1.0f;

    public void LoadJsonData(string json)
    {
        parsedData = JsonUtility.FromJson<ApiResponse>(json);
        scaleFactor = 1f / parsedData.averageDoor;
        GenerateObjects();
    }

    private void GenerateObjects()
    {
        for (int i = 0; i < parsedData.points.Count; i++)
        {
            BoundingBox box = parsedData.points[i];
            string objectType = parsedData.classes[i];

            Vector3 position = new Vector3((box.x1 + box.x2) / 2 * scaleFactor, 0, (box.y1 + box.y2) / 2 * scaleFactor);
            Vector3 scale = new Vector3(Mathf.Abs(box.x2 - box.x1) * scaleFactor, 2.5f, Mathf.Abs(box.y2 - box.y1) * scaleFactor);

            GameObject prefab = null;
            switch (objectType)
            {
                case "wall":
                    prefab = Resources.Load<GameObject>("WallPrefab");
                    break;
                case "door":
                    prefab = Resources.Load<GameObject>("DoorPrefab");
                    break;
                case "window":
                    prefab = Resources.Load<GameObject>("WindowPrefab");
                    break;
            }

            if (prefab != null)
            {
                GameObject newObject = Instantiate(prefab, position, Quaternion.identity);
                newObject.transform.localScale = scale;
            }
        }
    }

    private void Start()
    {
        // Sample JSON string
        string json = "{\"averageDoor\": 2.5, \"classes\": [\"wall\", \"door\"], \"points\": [{\"x1\": 0, \"y1\": 0, \"x2\": 10, \"y2\": 2}, {\"x1\": 12, \"y1\": 0, \"x2\": 14, \"y2\": 2}]}";
        LoadJsonData(json);
    }
}
