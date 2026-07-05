using UnityEngine;
using System.Collections.Generic;

public class menubar : MonoBehaviour//子側　
{
    public bool isOS9;//OS9のメニューバーか否か 判定はGetMouseUpで消す
    public bool isMenubarEnable;//メニューバーが有効状態か
    public List<Renderer> elements;//左から順にアサイン プルダウンした時のやつ
    public List<MenubarTypeSet> elementsHit = new List<MenubarTypeSet>();
    [SerializeField] Camera OverViewCamera;
    public int enableIndex;
    
    void Start()
    {
        GameObject.FindWithTag("Manager").GetComponent<WindowManagerTest>();
        OverViewCamera = GameObject.FindWithTag("OverViewCamera").GetComponent<Camera>();
        for(int i = 0; i < elements.Count; i++)
        {
            elementsHit.Add(GetRectAngle(elements[i].gameObject));
        }
        foreach(var element in elements)
        {
            element.enabled = false;//プルダウンした時だけ入れる感じで
        }
    }
    void Update()
    {
        var mousePos = Input.mousePosition;
        if(Input.GetMouseButtonDown(0))//開ける時
        {
            if(isMenubarEnable)
            {
                ChangeElementsState(enableIndex,enableIndex,false);
                isMenubarEnable = false;
            }
            else
            {
                for(int i = 0; i < elementsHit.Count; i++)
                {
                    if(mousePos.x >= elementsHit[i].minX && mousePos.x <= elementsHit[i].maxX && mousePos.y >= elementsHit[i].minY && mousePos.y <= elementsHit[i].maxY)
                    {
                        ChangeElementsState(null,i,false);
                        enableIndex = i;
                        isMenubarEnable = true;
                        break;
                    }
                }
            }
        }
        if(Input.GetMouseButtonUp(0))//OS9のメニューバーはクリック終了で消える+1fラグあり？
        {
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
                ChangeElementsState(oldIndex,enableIndex,false);
            }
        }
    }
    void ChangeElementsState(int? oldIndex,int newIndex,bool newWindow)
    {
        if(oldIndex == null&&newWindow)//新規に開く時
        {
            elements[newIndex].enabled = true;
            //詳細メニューの表示処理
        }
        if(oldIndex == null&&!newWindow)//閉じる時
        {
            elements[newIndex].enabled = false;
            //詳細メニューの非表示処理
        }
        else
        {
            int oldIndexValue = oldIndex.Value;
            elements[oldIndexValue].enabled = false;
            elements[newIndex].enabled = true;
        }
        
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
}

