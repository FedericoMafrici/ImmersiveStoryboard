using UnityEngine;

public class BillboardWithoutMovement : MonoBehaviour
{
    public Transform cam;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = GameObject.Find("XR Origin (XR Rig)/Camera Offset/Main Camera").transform;
    }
    

    void LateUpdate()
    {
        // Ottieni la direzione dalla UI alla telecamera
        Vector3 direction = (transform.position - cam.position).normalized;
        
        // Inverti la direzione (perché la UI deve guardare verso la camera)
        transform.rotation = Quaternion.LookRotation(-direction);
    }
}
