using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private MovementManager _movementManager = default;
    [SerializeField] private PhotoTourCamera _photoTourCamera = default;
    private Animator _animator;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("StopWall"))
        {
            _animator.SetBool("isWalking", false);
            _movementManager.StopMovement();
            _photoTourCamera.StartTour();
        }
    }
}
