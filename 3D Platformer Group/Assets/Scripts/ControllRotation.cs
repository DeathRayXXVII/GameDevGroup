using UnityEngine;

public class ControllRotation : MonoBehaviour
{
    public float rotationSpeed = 4f;
    public Transform obj;
    
    public void Start()
    {
        obj = GetComponent<Transform>();
    }
    void Update()
    {
        obj.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }
}
