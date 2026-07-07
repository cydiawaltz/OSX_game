using UnityEngine;
using System.Collections.Generic;

public class menubar : MonoBehaviour//子側　
{
    public bool isOS9;//OS9のメニューバーか否か 判定はGetMouseUpで消す
    public bool isMenubarEnable = false;//メニューバーが有効状態か
    public List<Renderer> elements;//左から順にアサイン プルダウンした時のやつ
    public List<MenubarTypeSet> elementsHit = new List<MenubarTypeSet>();
    [SerializeField] Camera OverViewCamera;
    WindowManagerTest manager;
    public int enableIndex;
    //debug
    public int frameCount;
    
    
    void Start()
    {
        manager = GameObject.FindWithTag("Manager").GetComponent<WindowManagerTest>();
        OverViewCamera = GameObject.FindWithTag("OverViewCamera").GetComponent<Camera>();
        manager.ClickDown += ClickDown;
        for(int i = 0; i < elements.Count; i++)
        {
            elementsHit.Add(GetRectAngle(elements[i].gameObject));
            Debug.Log("メニューバーの長方形"+i);
        }
        foreach(var element in elements)
        {
            element.enabled = false;//プルダウンした時だけ入れる感じで
        }
    }
    void ClickDown()
    {
        var mousePos = Input.mousePosition;
        
            Debug.Log("クリック(menubar.cs)"+gameObject.name);
            if(isMenubarEnable)
            {
                Debug.Log("メニュー閉判定前");
                    if(mousePos.x >= elementsHit[enableIndex].minX && mousePos.x <= elementsHit[enableIndex].maxX && mousePos.y >= elementsHit[enableIndex].minY && mousePos.y <= elementsHit[enableIndex].maxY)
                    {
                        CloseElements(enableIndex);
                        isMenubarEnable = false;
                        Debug.Log("メニューを閉じる");
                    }
            }
            else
            {
                Debug.Log("メニュー開判定前");
                for(int i = 0; i < elementsHit.Count; i++)
                {
                    Debug.Log("接触判定前"+i);
                    if(mousePos.x >= elementsHit[i].minX && mousePos.x <= elementsHit[i].maxX && mousePos.y >= elementsHit[i].minY && mousePos.y <= elementsHit[i].maxY)
                    {
                        OpenElements(i);
                        enableIndex = i;
                        isMenubarEnable = true;
                        Debug.Log("メニューを開く");
                        break;
                    }
                }
            }
        
    }
    void Update()
    {
        frameCount++;
        var mousePos = Input.mousePosition;
        //clickdownの中身はここにあった
        /*if(Input.GetMouseButtonDown(0))
        {
            Debug.Log("クリック(menubar.cs)"+gameObject.name);
            if(isMenubarEnable)
            {
                Debug.Log("メニュー閉判定前");
                    if(mousePos.x >= elementsHit[enableIndex].minX && mousePos.x <= elementsHit[enableIndex].maxX && mousePos.y >= elementsHit[enableIndex].minY && mousePos.y <= elementsHit[enableIndex].maxY)
                    {
                        CloseElements(enableIndex);
                        isMenubarEnable = false;
                        Debug.Log("メニューを閉じる");
                    }
            }
            else
            {
                Debug.Log("メニュー開判定前");
                for(int i = 0; i < elementsHit.Count; i++)
                {
                    Debug.Log("接触判定前"+i);
                    if(mousePos.x >= elementsHit[i].minX && mousePos.x <= elementsHit[i].maxX && mousePos.y >= elementsHit[i].minY && mousePos.y <= elementsHit[i].maxY)
                    {
                        OpenElements(i);
                        enableIndex = i;
                        isMenubarEnable = true;
                        Debug.Log("メニューを開く");
                        break;
                    }
                }
            }
        }*/
        if(Input.GetMouseButtonUp(0))//OS9のメニューバーはクリック終了で消える+1fラグあり？
        {
            Debug.Log("マウスup(OS9)");
            if(isOS9)
            {
                isMenubarEnable = false;
            }
        }
        if(isMenubarEnable)
        {
            int oldIndex = enableIndex;
            for(int i = 0; i < elementsHit.Count; i++)
            {
                if(mousePos.x >= elementsHit[i].minX && mousePos.x <= elementsHit[i].maxX && mousePos.y >= elementsHit[i].minY && mousePos.y <= elementsHit[i].maxY)
                {
                    enableIndex = i;
                    break;
                }
            }
            if(oldIndex != enableIndex)
            {
                ChangeElementsState(oldIndex,enableIndex);
            }
        }
    }
    void OpenElements(int index)
    {
        elements[index].enabled = true;
        //詳細メニューの表示処理
        Debug.Log("open");
    }
    void CloseElements(int index)
    {
        elements[index].enabled = false;
        //詳細メニューの非表示処理
        Debug.Log("close");
    }
    void ChangeElementsState(int oldIndex,int newIndex)
    {
        elements[oldIndex].enabled = false;
        elements[newIndex].enabled = true;
        Debug.Log("state change");
    }
    MenubarTypeSet GetRectAngle(GameObject target)
    {
        MenubarTypeSet result = new MenubarTypeSet();
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
    void OnDisable()
    {
        isMenubarEnable = false;
        for(int i = 0; i < elements.Count; i++)
        {
            elements[i].enabled = false;
        }
    }
}

