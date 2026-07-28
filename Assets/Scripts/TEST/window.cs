using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System.Collections;

public class Window : MonoBehaviour
{
    //public WindowManagerTest windowManager;
    public int AppIndex;//このウインドウが何のアプリか　基本的にはDock順 WindowManagerTestのAppIndex
    public bool isTopMost;//ステータスバー、各種ボタンのグレーアウトに使用
    public bool oldTopMost;
    public Camera OverViewCamera;//俯瞰かめら
    [SerializeField] public float Rotation { get; private set; }//x軸のみ　ギミック専用　変更はChangeRotation()
    public float originalWidth;//回転後俯瞰で

    public bool isMinimumWindow;//最小化ウインドウか？(ジニーエフェクトの問題あるんで検討中)
    //public Vector2 originalPosition;//左上の座標(スクリーン)　=> ウインドウ位置を補正しないこと前提
    //public float width, height;//横幅・縦幅 
    //float minX, minY, maxX, maxY;//ウインドウ各端
    public bool isDraging;//何某かをドラッグ中 スライダー？
    public Texture[] textures;//ウインドウ状態ごとのテクスチャ　0:通常　1:最前面
    public List<Renderer> targetRenders;//テクスチャ変更するレンダラー　同じテクスチャにまとめてるので全部に同じの流してもOK
    //ここからスライダー実装
    [SerializeField] GameObject slider;//スライダーのポインター

    [SerializeField] GameObject target;//これはrectanglesetの取得用
    //chatgpt
    [SerializeField] float titleBarHeight; // スクリーン座標
    bool isDragging;
    Vector3 dragOffset;
    float screenZ;
    private RectAngleSet rect;

    //debug
    public bool isChangeRadius;
    public float Radius;
    public bool isSkipChangeTexture = false;//テクスチャ不足で変更処理スキップするか　debug用

    //public WindowState state;
    void Start()
    {
        //OverViewCamera = GameObject.FindWithTag("OverViewCamera").GetComponent<Camera>();
        OverViewCamera = WindowManager.overCam;
        //windowManager = GameObject.FindWithTag("manager").GetComponent<WindowManagerTest>();
        if (textures.Length < 2)//debug
        {
            Debug.Log("テクスチャ不足 -Window.cs -ObjectName:" + this.gameObject.name);
            isSkipChangeTexture = true;
        }
        if (target == null)
        {
            Debug.Log("target Windowがねぇ! -Window.cs -ObjectName:" + this.gameObject.name);
            target = this.gameObject;
        }
        originalWidth = target.transform.localScale.z;

        //ウインドウサイズの取得設定 => classformに移譲
        /*MeshFilter mf = target.GetComponent<MeshFilter>();

        Vector3[] vertices = mf.mesh.vertices;

        minX = float.MaxValue;//ウインドウ左端
        maxX = float.MinValue;//右端

        minY = float.MaxValue;//下端
        maxY = float.MinValue;//上端

        foreach (Vector3 v in vertices)
        {
            // ローカル→ワールド
            Vector3 world = target.transform.TransformPoint(v);

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
        );*/
        rect = FunctionSet.GetRectAngle(target, OverViewCamera);
    }
    IEnumerator SliderDrag()
    {
        yield return null;
    }
    public void Pre_CheckWindowState()//CheckWindowState()呼ぶ前に必ず呼ぶ　初期化用関数
    {
        isTopMost = false;
    }
    public bool CheckWindowState(Vector3 mousePos)
    {
        bool isactive = mousePos.x >= rect.minX &&
                        mousePos.x <= rect.maxX &&
                        mousePos.y >= rect.minY &&
                        mousePos.y <= rect.maxY;//ウインドウの内側判定
        if (isactive)//上にウインドウがある時呼ばれないのでここでTopMost設定にする
        {
            isTopMost = true;
        }

        return isactive;
    }
    public void ChangeWindowState()//ウインドウ状態（テクスチャなど）を変更
    {
        if (isSkipChangeTexture)//debug
        {
            return;
        }
        //ここでテクスチャ云々を変更する
        if (isTopMost)
        {
            for (int i = 0; i <= targetRenders.Count - 1; i++)
            {
                targetRenders[i].material.mainTexture = textures[0];//最前面のテクスチャ
            }
            oldTopMost = true;
        }
        else
        {
            if (oldTopMost)
            {
                for (int i = 0; i <= targetRenders.Count - 1; i++)
                {
                    targetRenders[i].material.mainTexture = textures[1];//最前面のテクスチャ
                }
            }
            oldTopMost = false;
        }
    }
    void Update()
    {
        if (isChangeRadius)
        {
            isChangeRadius = false;
            ChangeRotation(Radius);
        }
        //
        if (!WindowManager.overCam.gameObject.activeSelf)
            return;

        Vector3 mouse = Input.mousePosition;

        if (Input.GetMouseButtonDown(0) && isTopMost)
        {
            bool onTitleBar =
                mouse.x >= rect.minX &&
                mouse.x <= rect.maxX &&
                mouse.y >= rect.maxY - titleBarHeight &&
                mouse.y <= rect.maxY;

            if (onTitleBar)
            {
                screenZ = OverViewCamera.WorldToScreenPoint(target.transform.position).z;

                mouse.z = screenZ;
                Vector3 world = OverViewCamera.ScreenToWorldPoint(mouse);

                dragOffset = this.transform.position - world;
                isDragging = true;
            }
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            mouse.z = screenZ;

            Vector3 world = OverViewCamera.ScreenToWorldPoint(mouse);
            Vector3 pos = world + dragOffset;

            this.transform.position = new Vector3(
                pos.x,
                target.transform.position.y,
                pos.z
            );
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            rect = FunctionSet.GetRectAngle(target, OverViewCamera);
        }

    }
    public void ChangeRotation(float radius)
    {
        Rotation = radius;
        target.gameObject.transform.rotation = Quaternion.Euler(radius, 0, 0);
        Vector3 scale = transform.localScale;
        scale.z = originalWidth / Mathf.Cos(radius * Mathf.Deg2Rad);
        transform.localScale = scale;
    }
}