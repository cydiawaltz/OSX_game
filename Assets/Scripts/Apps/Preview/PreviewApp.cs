using UnityEngine;

public class PreviewApp : MonoBehaviour
{
    [SerializeField] MeshFilter[] scrolltarget;//上部・下部ともに入れとく
    [SerializeField] MeshFilter containts;

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

    private Vector2[] originalUV;

    void Start()
    {
        scrollBar.ScrollBarDown += OnEnterDrag;
        scrollBar.ScrollBarUp += OnExitDrag;

        Mesh mesh = containts.mesh;

        // 元のUVを保存
        originalUV = mesh.uv;

        // 現在のUVの範囲を取得
        Vector2 uvMin = originalUV[0];
        Vector2 uvMax = originalUV[0];

        foreach (Vector2 uv in originalUV)
        {
            uvMin = Vector2.Min(uvMin, uv);
            uvMax = Vector2.Max(uvMax, uv);
        }

        // 現在位置を最上部として保存
        topUV = uvMax;

        // UVの高さ
        Vector2 scale = uvMax - uvMin;

        // 画像の底辺とUVの底辺が一致する位置
        bottomUV = new Vector2(
            topUV.x,
            topUV.y - (1.0f - scale.y)
        );

        SetUV(scrollBar.currentPosition);
    }

    void Update()
    {
        if (isdragging)
        {
            SetUV(scrollBar.currentPosition);
        }
    }

    void SetUV(float position)
    {
        Mesh mesh = containts.mesh;

        Vector2[] uv = mesh.uv;

        // 最上部 → 最下部へ移動
        float offsetY = Mathf.Lerp(
            topUV.y,
            bottomUV.y,
            position
        );

        // 元のUVからの移動量
        float deltaY = offsetY - topUV.y;

        for (int i = 0; i < uv.Length; i++)
        {
            uv[i] = originalUV[i] + new Vector2(0, deltaY);
        }

        mesh.uv = uv;

        Debug.Log(
            "SetUV position:" + position +
            " offsetY:" + offsetY
        );
    }

    void OnEnterDrag()
    {
        foreach (MeshFilter target in scrolltarget)
        {
            Mesh mesh = target.mesh;

            // MeshFilterだけでは色を変更できないため、
            // MeshRenderer側を取得
            MeshRenderer renderer = target.GetComponent<MeshRenderer>();

            if (renderer != null)
            {
                renderer.material.color = pullDownColor;
            }
        }

        isdragging = true;
    }

    void OnExitDrag()
    {
        foreach (MeshFilter target in scrolltarget)
        {
            MeshRenderer renderer = target.GetComponent<MeshRenderer>();

            if (renderer != null)
            {
                renderer.material.color = Color.white;
            }
        }

        isdragging = false;
    }
}