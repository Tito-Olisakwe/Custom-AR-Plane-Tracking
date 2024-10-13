using UnityEngine;
using UnityEngine.UI;

public class ColorChanger : MonoBehaviour
{
    private GameObject spawnedObject;

    public Button redButton;
    public Button greenButton;
    public Button blueButton;
    public Button yellowButton;

    void Start()
    {
        redButton.onClick.AddListener(() => ChangeColor(Color.red));
        greenButton.onClick.AddListener(() => ChangeColor(Color.green));
        blueButton.onClick.AddListener(() => ChangeColor(Color.blue));
        yellowButton.onClick.AddListener(() => ChangeColor(Color.yellow));
    }

    void ChangeColor(Color newColor)
    {
        if (spawnedObject != null)
        {
            Renderer objectRenderer = spawnedObject.GetComponent<Renderer>();
            if (objectRenderer != null)
            {
                objectRenderer.material.color = newColor;
            }
        }
        else
        {
            Debug.LogError("No spawned object assigned to ColorChanger.");
        }
    }
 
    public void SetSpawnedObject(GameObject obj)
    {
        spawnedObject = obj;
    }
}
