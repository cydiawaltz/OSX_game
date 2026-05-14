using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Window : MonoBehaviour
{
    //public WindowManagerTest windowManager;
    public bool isTopMost;//ステータスバー、各種ボタンのグレーアウトに使用
    public Camera OverViewCamera;//俯瞰かめら
    [SerializeField] public float Rotation{get; private set;}//x軸のみ　ギミック専用　変更はChangeRotation()
    public float originalWidth;//回転後俯瞰で
    
    public bool isMinimumWindow;//最小化ウインドウか？(ジニーエフェクトの問題あるんで検討中)
    public Vector2 originalPosition;//左上の座標(スクリーン)　=> ウインドウ位置を補正しないこと前提
    public float width,height;//横幅・縦幅 
    float minX,minY,maxX,maxY;//ウインドウ各端

    //debug
    public bool isChangeRadius;
    public float Radius;

    public WindowState state;
    void Start()
    {
        OverViewCamera = GameObject.FindWithTag("OverViewCamera").GetComponent<Camera>();
        //windowManager = GameObject.FindWithTag("manager").GetComponent<WindowManagerTest>();
        originalWidth = this.transform.localScale.z;
        //ウインドウサイズの取得設定
        MeshFilter mf = this.GetComponent<MeshFilter>();

        Vector3[] vertices = mf.mesh.vertices;

        minX = float.MaxValue;//ウインドウ左端
        maxX = float.MinValue;//右端

        minY = float.MaxValue;//下端
        maxY = float.MinValue;//上端

        foreach (Vector3 v in vertices)
        {
            // ローカル→ワールド
            Vector3 world = this.transform.TransformPoint(v);

            // ワールド→スクリーン
            Vector3 screen = OverViewCamera.WorldToScreenPoint(world);

            minX = Mathf.Min(minX, screen.x);
            maxX = Mathf.Max(maxX, screen.x);

            minY = Mathf.Min(minY, screen.y);
            maxY = Mathf.Max(maxY, screen.y);
        }

        width = maxX - minX;
        height = maxY - minY;

        // Unityのスクリーン座標は左下原点なので左上座標に変換
        Vector2 leftTop = new Vector2(
            minX,
            Screen.height - maxY
        );
    }
    public void Pre_CheckWindowState()//CheckWindowState()呼ぶ前に必ず呼ぶ　初期化用関数
    {
        isTopMost = false;
    }
    public bool CheckWindowState(Vector3 mousePos)
    {
        bool isactive = mousePos.x >= minX &&
                        mousePos.x <= maxX &&
                        mousePos.y >= minY &&
                        mousePos.y <= maxY;//ウインドウの内側判定
        if(isactive)//上にウインドウがある時呼ばれないのでここでTopMost設定にする
        {
            isTopMost = true;
        }
        
        return isactive;
    }
    void ChangeWindowState()//ウインドウ状態（テクスチャなど）を変更
    {
        //ここでテクスチャ云々を変更する
        if(isTopMost)
        {}
        else
        {}
    }
    void Update()
    {
        if(isChangeRadius)
        {
            isChangeRadius = false;
            ChangeRotation(Radius);
        }
    }
    public void ChangeRotation(float radius)
    {
        Rotation = radius;
        this.gameObject.transform.rotation = Quaternion.Euler(radius,0,0);
        Vector3 scale = transform.localScale;
        scale.z = originalWidth / Mathf.Cos(radius*Mathf.Deg2Rad);
        transform.localScale = scale;
    }
}