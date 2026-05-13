using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class WindowManagerTest : MonoBehaviour
{
    [SerializeField] List<GameObject> windows;
    [SerializeField] List<Window> windows_statestore;//一個上のやつのWindowStateのインスタンス保持
    
    [SerializeField] float defaultHeight;//ここは末端に合わせる
    public GameObject[] instantiateObject;//出る可能性のあるウインドウ(Assetに書き出し済みのやつ)
    [SerializeField] bool IsOverView;
    public int distBetWindow;//ウインドウ間の距離（y）飛び越えられないラインで SetWindowState()んとこに脳筋プレイしてるので整数限定で
    [SerializeField] GameObject player;

    //debug
    public bool SetWindow;
    public int currentIndex;

    void Start()
    {
        for(int i = 0;i<=windows.Count-1;i++)
        {
            windows_statestore.Add(windows[i].GetComponent<Window>() as Window);
        }
        defaultHeight = windows.Last().transform.position.y;
        SetWindowState();
        //var a = GetComponent<WindowManagerTest>()
    }

    // Update is called once per frame
    void Update()
    {
        if(SetWindow)
        {
            SetWindowState();
            SetWindow = false;
        }
        if(Input.GetMouseButtonDown(0))//左ボタン押下取得
        {
            FocusWindow();
        }
    }
    public void SetWindowState()
    {
        float pre_Height = -distBetWindow;
        currentIndex = windows.Count - (int)(Mathf.Round(player.transform.position.y -0.6f)/ distBetWindow)-1;//超脳筋　もうちょいパフォーマンス落とさずになんとかする方法探したい
        player.transform.parent = windows[currentIndex].transform;
        for(int i = windows.Count-1; i >= 0; i--)
        {
            var window = windows[i].transform.position;
            window = new Vector3(window.x,pre_Height+distBetWindow,window.z);
            windows[i].transform.position = window;
            pre_Height = window.y;
        }
        for(int i = 0;i<=windows.Count-1;i++)
        {
            windows_statestore[i] = windows[i].GetComponent<Window>() as Window;
        }
        foreach(Window state in windows_statestore)
        {
            state.isTopMost = false;
        }
        windows_statestore[0].isTopMost = true;
    }
    public void FocusWindow()//ウインドウをクリックして最前列に
    {
        //クリック位置をスクリーン座標→ワールド座標に変換してxz成分だけ使って判定　配列0(一番上)から順にやってく
        Vector3 mousePos = Input.mousePosition;
        foreach(Window state in windows_statestore)
        {
            state.Pre_CheckWindowState();
        }
        for(int i = 0;i<=windows.Count-1;i++)
        {
            bool istarget = windows_statestore[i].CheckWindowState(mousePos);
            if(istarget)
            {
                GameObject target = windows[i];
                windows.RemoveAt(i);
                windows.Insert(0,target);
                SetWindowState();
                break;
            }
        }
    }
    public void ChangeViewMode(bool isOverView)//表示を変更 引数は変更先が俯瞰か否か
    {
        if(isOverView)//俯瞰視点
        {
            IsOverView = true;
        }
        else
        {
            IsOverView = false;
        }
    }
    public void CreateWindow(int WindowNumber,Vector2 position)
    {
        var window = Instantiate(instantiateObject[WindowNumber]) as GameObject;
        window.transform.position = new Vector3(position.x,0,position.y);
        windows.Insert(0,window);
        SetWindowState();
    }
}
