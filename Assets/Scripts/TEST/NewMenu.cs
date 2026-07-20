using UnityEngine;

public class NewMenu : MonoBehaviour//廃止
{
    [SerializeField] GameObject[] menuPanels;
    [SerializeField] RectAngleSet[] panel_hit;
    [SerializeField] Camera OverViewCamera;
    [SerializeField] int mouseFrame = 0;
    void Start()
    {
        OverViewCamera = GameObject.FindWithTag("OverViewCamera").GetComponent<Camera>();
        panel_hit = new RectAngleSet[menuPanels.Length];
        for(int i = 0; i < menuPanels.Length; i++)
        {
            panel_hit[i] = GetRectAngle(menuPanels[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("frameCount(NewMenu.cs)");
        var mousePos = Input.mousePosition;
        if(Input.GetMouseButton(0))
        {
            if(mouseFrame == 0)//最初にクリックした時　clickDownの代わり
            {
                Debug.Log("mmouse down (NewMenu.cs)");
                
            }
            mouseFrame++;
        }
        else mouseFrame = 0;
    }
    RectAngleSet GetRectAngle(GameObject target)
    {
        RectAngleSet result = new RectAngleSet();
        //ウインドウサイズの取得設定
        MeshFilter mf = target.GetComponent<MeshFilter>();

        Vector3[] vertices = mf.mesh.vertices;

        result.minX = float.MaxValue;//ウインドウ左端
        result.maxX = float.MinValue;//右端

        result.minY = float.MaxValue;//下端
        result.maxY = float.MinValue;//上端

        foreach (Vector3 v in vertices)
        {
            // ローカル→ワールド
            Vector3 world = target.transform.TransformPoint(v);

            // ワールド→スクリーン
            Vector3 screen = OverViewCamera.WorldToScreenPoint(world);

            result.minX = Mathf.Min(result.minX, screen.x);
            result.maxX = Mathf.Max(result.maxX, screen.x);

            result.minY = Mathf.Min(result.minY, screen.y);
            result.maxY = Mathf.Max(result.maxY, screen.y);
        }

        result.width = result.maxX - result.minX;
        result.height = result.maxY - result.minY;

        // Unityのスクリーン座標は左下原点なので左上座標に変換
        Vector2 leftTop = new Vector2(
            result.minX,
            Screen.height - result.maxY
        );
        return result;
    }
}

