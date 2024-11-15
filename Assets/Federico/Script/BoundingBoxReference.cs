using UnityEngine;

public class BoundingBoxReference : MonoBehaviour
{
    [SerializeField] public BoundingBoxInteractionManager BoundingBoxController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 GetBoundingBoxSize()
    {
        if (Application.isEditor)
        {
           // Debug.LogError("Dimensione bounding box non trovata");
            return this.BoundingBoxController.transform.Find("BoundingBoxWrapper/Cube").GetComponent<Renderer>().bounds.size;
        }
        else
        {
            if (this.BoundingBoxController.boundingBoxSize == Vector3.zero)
            {
                Debug.LogError("bounding box nulla");
            }
          //  return this.BoundingBoxController.boundingBoxSize;
          //  return this.BoundingBoxController.transform.Find("BoundingBoxWrapper/Cube").GetComponent<Renderer>().bounds.size; 
          return Vector3.one;
        }
        return new Vector3(-1, -1, -1);
    }
}
