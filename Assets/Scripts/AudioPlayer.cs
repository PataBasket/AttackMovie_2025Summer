using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    private static BGMPlayer instance;

    private void Awake()
    {
        // すでに存在する場合は新しいものを破棄
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 最初のインスタンスを保存
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}