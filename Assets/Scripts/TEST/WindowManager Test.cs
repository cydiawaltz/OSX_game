using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System;

public class WindowManagerTest : MonoBehaviour
{
    [SerializeField] List<GameObject> windows;//Start()時のnull参照回避策　statestoreに情報流したら以後は使わない
    //[SerializeField] GameObject windowbase;//上のwindowsの親オブジェクト
    [SerializeField] List<Window> windows_statestore;//一個上のやつのWindowStateのインスタンス保持
    
    [SerializeField] float defaultHeight;//ここは末端に合わせる
    public GameObject[] instantiateObject;//出る可能性のあるウインドウ(Assetに書き出し済みのやつ)
    public bool IsOverView;
    public float distBetWindow;//ウインドウ間の距離（y）飛び越えられないラインで SetWindowState()んとこに脳筋プレイしてるので整数限定で
    [SerializeField] GameObject player;
    [SerializeField] CharacterController player_chara;
    [SerializeField] GameObject overViewCamera; 
    [SerializeField] GameObject playerCamera;
    public Action changeVisualState;
    public Action changeIndexState;
    public Action ClickDown;//menubar.csのクリックバグはフレーム数の不一致らしいのでこれで管理
    [SerializeField] List<RectTransform> UIobjects;
    public int AppIndex;
    //debug
    public int frameCount;
    /*
    >>10.1
    0:Finder 1:iTunes 2:IE 3:Preview 4:Sherlock 5:システム環境設定 6:Stickies 7:TextEdit 8:Classic startup 9:OS9(SimpleText)
    >>
    */

    //debug
    public bool SetWindow;
    public int currentIndex;
    public Button changebutton;
    //[SerializeField] bool isFirstFrame = true;//Update()で処理　Start()で初期化するとパス通んない説

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
        IsOverView = true;
        playerCamera.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        frameCount++;
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
            //Debug.Log("クリック(WindowManagerTest.cs)");
            ClickDown?.Invoke();
            if(IsOverView)
            {
                FocusWindow();
            }
        }
    }
    public void OnClick()
    {
        if(IsOverView)
        {
            ChangeViewMode(false);
            overViewCamera.SetActive(false);
            playerCamera.SetActive(true);
            
        }
        else
        {
            ChangeViewMode(true);
            overViewCamera.SetActive(true);
            playerCamera.SetActive(false);
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
        for(int i = 0;i<=UIobjects.Count-1;i++)
        {
            if(RectTransformUtility.RectangleContainsScreenPoint(UIobjects[i],mousePos))
            {
                windows_statestore[0].isTopMost = true;//UIオブジェクトの上にいるときは最前面のウインドウを最前面の状態にする
                windows_statestore[0].ChangeWindowState();
                Debug.Log("mouse on UI");
                return;//UIオブジェクトの上にいるときはウインドウ操作をしない
            }
        }
        for(int i = 0;i<=windows_statestore.Count-1;i++)
        {
            bool istarget = windows_statestore[i].CheckWindowState(mousePos);
            windows_statestore[i].ChangeWindowState();
            if(istarget)
            {
                Window target = windows_statestore[i];
                windows_statestore.RemoveAt(i);
                windows_statestore.Insert(0,target);
                SetWindowState();
                AppIndex = target.AppIndex;
                break;
            }
        }
        //暫定
        changeIndexState.Invoke();
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
        changeVisualState?.Invoke();
    }
    public void CreateWindow(int WindowNumber,Vector2 position)
    {
        var window = Instantiate(instantiateObject[WindowNumber]).GetComponent<Window>();
        window.gameObject.transform.position = new Vector3(position.x,0,position.y);
        windows_statestore.Insert(0,window);
        SetWindowState();
    }
}
