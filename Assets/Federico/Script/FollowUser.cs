using UnityEngine;

public class FolllowUser : MonoBehaviour
{
    [SerializeField] public Transform mainCamera;

    [SerializeField] public Vector3 offset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        this.transform.position = mainCamera.position + offset;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
