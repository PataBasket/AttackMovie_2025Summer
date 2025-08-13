using UnityEngine;

public class MovementManager : MonoBehaviour
{
    [SerializeField]
    private GameObject mainGround;

    [SerializeField] 
    private float rotationSpeed;

    private bool _isRotating = true;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_isRotating)
        {
            mainGround.transform.Rotate(Vector3.left, rotationSpeed);
        }
    }
}
