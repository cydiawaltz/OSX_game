using UnityEngine;

public class PreviewApp : MonoBehaviour
{
    [SerializeField] Renderer[] scrolltarget;//上部・下部ともに入れとく
    public Vector2 minUV,maxUV;
    public float maxBar,minBar;//動く方向だけ
    public float currentPos;//0~1
    [Header("ここのscrollbarは直で参照とおす")]
    public ScrollBar scrollBar;
    [SerializeField] Vector2 local;
    [SerializeField] Vector2 oldLocal;
    [SerializeField] Color pullDownColor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scrollBar.ScrollBarDown+=OnEnterDrag;
        scrollBar.ScrollBarUp+=OnExitDrag;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnEnterDrag()
    {
        foreach(var target in scrolltarget)
        {
            target.material.color = pullDownColor;
        }
    }
    void OnExitDrag()
    {
        foreach(var target in scrolltarget)
        {
            target.material.color = new Color(1,1,1,1);
        }
    }
}
