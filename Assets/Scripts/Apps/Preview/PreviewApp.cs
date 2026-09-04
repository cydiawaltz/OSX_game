using UnityEngine;

public class PreviewApp : MonoBehaviour
{
    [SerializeField] Renderer[] scrolltarget;//上部・下部ともに入れとく
    public Vector2 minUV, maxUV;
    public float maxBar, minBar;//動く方向だけ
    public float currentPos;//0~1
    [Header("ここのscrollbarは直で参照とおす")]
    public ScrollBar scrollBar;
    [SerializeField] Vector2 local;
    [SerializeField] Vector2 oldLocal;
    [SerializeField] Color pullDownColor;
    public bool isdragging = false;
    [Header("UV設定")]
    [SerializeField] Vector2 topUV;       // スクロール最上部
    [SerializeField] Vector2 bottomUV;    // スクロール最下部
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scrollBar.ScrollBarDown += OnEnterDrag;
        scrollBar.ScrollBarUp += OnExitDrag;

        // 初期位置
        SetUV(scrollBar.currentPosition);
    }

    void Update()
    {
        SetUV(scrollBar.currentPosition);
    }

    void SetUV(float position)
    {
        foreach (var target in scrolltarget)
        {
            Material mat = target.material;

            // 表示範囲を topUV ～ bottomUV の間で移動
            Vector2 uv = Vector2.Lerp(bottomUV, topUV, position);

            mat.mainTextureOffset = uv;
        }
    }
    void OnEnterDrag()
    {
        foreach (var target in scrolltarget)
        {
            target.material.color = pullDownColor;
        }
        isdragging = true;
    }
    void OnExitDrag()
    {
        foreach (var target in scrolltarget)
        {
            target.material.color = new Color(1, 1, 1, 1);
        }
        isdragging = false;
    }
}
