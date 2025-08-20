using UnityEngine;
using DG.Tweening;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private float skyboxDuration;
    
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
        
    }
}
