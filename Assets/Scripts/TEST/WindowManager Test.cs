using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class WindowManagerTest : MonoBehaviour
{
    [SerializeField] List<GameObject> windows;//Start()時のnull参照回避策　statestoreに情報流したら以後は使わない
    //[SerializeField] GameObject windowbase;//上のwindowsの親オブジェクト
    [SerializeField] List<Window> windows_statestore;//一個上のやつのWindowStateのインスタンス保持
    
    [SerializeField] float defaultHeight;//ここは末端に合わせる
    public GameObject[] instantiateObject;//出る可能性のあるウインドウ(Assetに書き出し済みのやつ)
    [SerializeField] bool IsOverView;
    public int distBetWindow;//ウインドウ間の距離（y）飛び越えられないラインで SetWindowState()んとこに脳筋プレイしてるので整数限定で
    [SerializeField] GameObject player;
    [SerializeField] CharacterController player_chara;

    //debug
    public bool SetWindow;
    public int currentIndex;
    [SerializeField] bool isFirstFrame = true;//Update()で処理　Start()で初期化するとパス通んない説

    void Start()
    {
        /*for(int i = 0;i<=windows.Count-1;i++)
        {
            windows.Add(windows[i].GetComponent<Window>() as Window);
        }*/
        
        //defaultHeight = windows.Last().gameObject.transform.position.y;
        //SetWindowState();
        //var a = GetComponent<WindowManagerTest>()
        for(int i = 0;i<=windows.Count-1;i++)//windows[]はこのあと使わない
        {
            windows_statestore.Add(windows[i].GetComponent<Window>());
        }
        player_chara= player.GetComponent<CharacterController>();
    }


    // Update is called once per frame
    void Update()
    {
        /*if(isFirstFrame)
        {
            for(int i = 0;i<0;i++)//終わるまで無限ループ
            {
            if(!(windowbase.transform.GetChild(i) == null))
            {
                windows.Add(windowbase.transform.GetChild(i).GetComponent<Window>());
            }
            else{break;}
            }
            isFirstFrame = false;
        }*/
        if(SetWindow)//debug
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
        //currentIndex = windows_statestore.Count - (int)(Mathf.Round(player.transform.position.y -0.6f)/ distBetWindow)-1;//超脳筋　もうちょいパフォーマンス落とさずになんとかする方法探したい
        //player.transform.parent = windows[currentIndex].transform; 
        player_chara.enabled = false;
        SetPlayerParent();
        GameObject parent = windows_statestore[currentIndex].gameObject;
        for(int i = windows_statestore.Count-1; i >= 0; i--)
        {
            var window = windows_statestore[i].gameObject.transform.position;
            window = new Vector3(window.x,pre_Height+distBetWindow,window.z);
            windows_statestore[i].gameObject.transform.position = window;
            pre_Height = window.y;
        }
        /*for(int i = 0;i<=windows.Count-1;i++)
        {
            windows_statestore[i] = windows[i].GetComponent<Window>() as Window;
        }*/
        Vector3 playerPos =  new Vector3(player.transform.position.x,parent.transform.position.y + 0.5f,player.transform.position.z);
        player.transform.position = playerPos;
        foreach(Window state in windows_statestore)
        {
            state.isTopMost = false;
        }
        windows_statestore[0].isTopMost = true;
        player_chara.enabled= true;
    }
    public void SetPlayerParent()
    {
        List<float> distList = new List<float>();
        for(int i = 0;i<windows_statestore.Count;i++)
        {
            distList.Add(Mathf.Abs(windows_statestore[i].gameObject.transform.position.y-player.transform.position.y));
        }
        currentIndex = distList.IndexOf(distList.Min());
        //player.gameObject.transform.parent = windows_statestore[currentIndex].transform;
    }
    public void FocusWindow()//ウインドウをクリックして最前列に
    {
        //クリック位置をスクリーン座標→ワールド座標に変換してxz成分だけ使って判定　配列0(一番上)から順にやってく
        Vector3 mousePos = Input.mousePosition;
        foreach(Window state in windows_statestore)
        {
            state.Pre_CheckWindowState();
        }
        for(int i = 0;i<=windows_statestore.Count-1;i++)
        {
            bool istarget = windows_statestore[i].CheckWindowState(mousePos);
            if(istarget)
            {
                Window target = windows_statestore[i];
                windows_statestore.RemoveAt(i);
                windows_statestore.Insert(0,target);
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
        var window = Instantiate(instantiateObject[WindowNumber]).GetComponent<Window>();
        window.gameObject.transform.position = new Vector3(position.x,0,position.y);
        windows_statestore.Insert(0,window);
        SetWindowState();
    }
}
