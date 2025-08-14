using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class PhotoTourCamera : MonoBehaviour
{
    [SerializeField] private PlayerMovement _playerMovement = default;
    
    [Header("映したい写真(QuadやMeshのRenderer)を順番に")]
    public List<Renderer> photos;

    [Header("全体設定")]
    public bool unscaledTime = true;     // ポーズ中も進めたいならtrue
    public float normalFOV = 60f;        // ツアー終了後のFOV（カメラ初期値を使いたければそのままでもOK）

    [Header("ズーム設定（1枚目）")]
    public float initialDelay = 0.8f;    // ★開始前に待つ秒数（インスペクターで変更可）
    public float firstZoomTime = 1.2f;   // 1枚目へのズーム時間
    public float zoomFOV = 35f;          // ズーム時のFOV
    public float holdTime = 0.9f;        // 各写真で止める時間（共通）
    public float padding = 0.2f;         // 画面端の余白（m）

    [Header("スライド移動の時間（2枚目以降を個別調整）")]
    [Tooltip("要素数 = photos.Count - 1。足りない分は最後の値/既定値が使われます。")]
    public List<float> slideTimesPerPhoto = new List<float>() { 0.8f, 0.8f, 0.8f, 0.8f, 0.8f };

    [Header("終了時")]
    public float zoomOutTime = 1.2f;     // 元位置へ戻る時間

    Camera cam;
    Vector3 startPos; Quaternion startRot; float startFov;

    void Awake()
    {
        cam = GetComponent<Camera>();
        startPos = transform.position;
        startRot = transform.rotation;
        startFov = cam.fieldOfView;
        if (normalFOV <= 0f) normalFOV = startFov;
    }

    [ContextMenu("Start Tour")]
    public void StartTour() => RunTour().Forget();

    async UniTask RunTour()
    {
        if (photos == null || photos.Count == 0) return;

        // ★開始前の待機
        if (initialDelay > 0f)
            await UniTask.Delay((int)(initialDelay * 1000), ignoreTimeScale: unscaledTime);

        // 1枚目：ズームイン
        var p0 = GetPose(photos[0]);
        await MoveCam(p0.pos, p0.rot, firstZoomTime, zoomFOV);
        await UniTask.Delay((int)(holdTime * 1000), ignoreTimeScale: unscaledTime);

        // 2〜N枚目：個別スライド
        for (int i = 1; i < photos.Count; i++)
        {
            float slideTime = GetSlideTime(i - 1); // 0→1、1→2… の移動に対応
            var p = GetPose(photos[i]);

            await MoveCam(p.pos, p.rot, slideTime, zoomFOV);
            await UniTask.Delay((int)(holdTime * 1000), ignoreTimeScale: unscaledTime);
        }

        // 元の位置へズームアウト
        await MoveCam(startPos, startRot, zoomOutTime, normalFOV);
        _playerMovement.StartRunning();
    }

    float GetSlideTime(int index)
    {
        if (slideTimesPerPhoto == null || slideTimesPerPhoto.Count == 0) return 0.8f;
        if (index < slideTimesPerPhoto.Count) return Mathf.Max(0f, slideTimesPerPhoto[index]);
        // 足りない分は最後の要素を使う
        return Mathf.Max(0f, slideTimesPerPhoto[slideTimesPerPhoto.Count - 1]);
    }

    (Vector3 pos, Quaternion rot) GetPose(Renderer r)
    {
        var center = r.bounds.center;
        float height = r.bounds.size.y;

        float fovRad = zoomFOV * Mathf.Deg2Rad;
        float dist = (height * 0.5f) / Mathf.Tan(fovRad * 0.5f) + padding;

        // 写真の「表」を向く法線。逆向きなら -r.transform.forward に変更
        Vector3 normal = r.transform.forward;

        Vector3 pos = center - normal * dist;
        Quaternion rot = Quaternion.LookRotation(normal);
        return (pos, rot);
    }

    async UniTask MoveCam(Vector3 pos, Quaternion rot, float time, float fov)
    {
        var seq = DOTween.Sequence().SetUpdate(unscaledTime);
        seq.Join(transform.DOMove(pos, time).SetEase(Ease.InOutSine));
        seq.Join(transform.DORotateQuaternion(rot, time).SetEase(Ease.InOutSine));
        seq.Join(cam.DOFieldOfView(fov, time).SetEase(Ease.InOutSine));
        await seq.AsyncWaitForCompletion();
    }
}
