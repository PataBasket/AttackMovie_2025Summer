using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class PhotoTourCamera : MonoBehaviour
{
    [Header("Targets (カメラを置く位置＆向き)")]
    public List<Transform> targets;

    [Header("Timing")]
    public float firstZoomTime = 1.2f;   // 1枚目ズームイン時間
    public float slideTime     = 0.8f;   // 2枚目以降のスライド時間
    public float holdTime      = 0.9f;   // 各写真で静止する時間
    public float zoomOutTime   = 1.2f;   // 最後のズームアウト時間

    [Header("FOV設定")]
    public float zoomFOV   = 35f;        // 近景用FOV（好きに調整）
    public float normalFOV = 60f;        // 元のFOV

    [Header("挙動")]
    public bool unscaledTime = true;     // 一時停止中やスローモで動かしたい場合 true

    Camera cam;
    Vector3 startPos; Quaternion startRot; float startFov;

    void Awake()
    {
        cam = GetComponent<Camera>();
        startPos = transform.position;
        startRot = transform.rotation;
        startFov = cam.fieldOfView > 0 ? cam.fieldOfView : normalFOV;
        if (normalFOV <= 0) normalFOV = startFov;
    }

    [ContextMenu("Start Tour")]
    public void StartTour() => RunTour().Forget();

    public async UniTask RunTour()
    {
        // 1枚目：ズームイン
        var t0 = targets[0];
        await MoveCam(t0.position, t0.rotation, firstZoomTime, zoomFOV);

        await UniTask.Delay((int)(holdTime*1000), ignoreTimeScale: unscaledTime);

        // 2〜6枚目：スライド移動（FOVはズーム維持）
        for (int i = 1; i < targets.Count; i++)
        {
            var t = targets[i];
            await MoveCam(t.position, t.rotation, slideTime, zoomFOV);
            await UniTask.Delay((int)(holdTime*1000), ignoreTimeScale: unscaledTime);
        }

        // 元位置へズームアウト
        await MoveCam(startPos, startRot, zoomOutTime, normalFOV);
    }

    async UniTask MoveCam(Vector3 pos, Quaternion rot, float time, float fov)
    {
        var s = DOTween.Sequence().SetUpdate(unscaledTime);
        s.Join(transform.DOMove(pos, time).SetEase(Ease.InOutSine));
        s.Join(transform.DORotateQuaternion(rot, time).SetEase(Ease.InOutSine));
        s.Join(cam.DOFieldOfView(fov, time).SetEase(Ease.InOutSine));
        await s.AsyncWaitForCompletion();
    }
}
