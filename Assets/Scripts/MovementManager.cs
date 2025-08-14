using UnityEngine;
using DG.Tweening;

public class MovementManager : MonoBehaviour
{
    [SerializeField] private GameObject mainGround;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float skyboxDuration;
    [SerializeField] private GameObject player;

    private bool _isRotating = true;
    public bool _isRunning = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var mat = new Material(RenderSettings.skybox);
        RenderSettings.skybox = mat;
        DOTween.To(
                () => mat.GetFloat("_Rotation"),
                v  => mat.SetFloat("_Rotation", v),
                360f, skyboxDuration
            )
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental); // ずっと回す
    }

    // Update is called once per frame
    void Update()
    {
        if (_isRotating && !_isRunning)
        {
            mainGround.transform.Rotate(Vector3.left, rotationSpeed*Time.deltaTime);
        }
        else if (_isRotating && _isRunning)
        {
            mainGround.transform.Rotate(Vector3.left, rotationSpeed*1.5f*Time.deltaTime);
        }
    }

    public void StopMovement()
    {
        _isRotating = false;
    }

    public void UnoRun()
    {
        _isRotating = true;
        _isRunning = true;
    }
}
