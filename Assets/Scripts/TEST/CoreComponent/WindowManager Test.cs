using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System;

public class WindowManager : MonoBehaviour
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
    //[SerializeField] Rigidbody player_rigidbody;
    [SerializeField] GameObject overViewCamera; //この二個は他から参照しまくるのでstatic
    [SerializeField] GameObject playerCamera;

    public static Camera overCam;
    public static Camera playerCam;
    public Action changeVisualState;
    public Action changeIndexState;
    [SerializeField] List<RectTransform> UIobjects;
    float previousWindowY;
    Window currentWindow;
    public int AppIndex;
    public float playerOffset;
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
    public int windowIndex;
    bool isMoveplayer;
    Window current;
    float oldWindowPos;
    //[SerializeField] bool isFirstFrame = true;//Update()で処理　Start()で初期化するとパス通んない説
    void Awake()
    {
        overCam = overViewCamera.GetComponent<Camera>();
        playerCam = playerCamera.GetComponent<Camera>();
    }
    void Start()
    {
        /*for(int i = 0;i<=windows.Count-1;i++)
        {
            windows.Add(windows[i].GetComponent<Window>() as Window);
        }*/

        //defaultHeight = windows.Last().gameObject.transform.position.y;
        //SetWindowState();
        //var a = GetComponent<WindowManagerTest>()
        for (int i = 0; i <= windows.Count - 1; i++)//windows[]はこのあと使わない
        {
            windows_statestore.Add(windows[i].GetComponent<Window>());
        }
        player_chara = player.GetComponent<CharacterController>();
        //player_rigidbody = player.GetComponent<Rigidbody>();
        IsOverView = true;
        playerCamera.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        //frameCount++;
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
        if (SetWindow)//debug
        {
            SetWindowState();
            SetWindow = false;
        }
        if (Input.GetMouseButtonDown(0))//左ボタン押下取得
        {
            //Debug.Log("クリック(WindowManagerTest.cs)");
            if (IsOverView)
            {
                FocusWindow();
            }
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            OnChangeView();
        }
        //FollowCurrentWindow();
    }
    /*void FollowCurrentWindow()
    {
        if (IsOverView)
            return;

        if (windows_statestore.Count == 0)
            return;

        GameObject currentWindow =
            windows_statestore[currentIndex].gameObject;

        float currentWindowY = currentWindow.transform.position.y;
        float deltaY = currentWindowY - previousWindowY;

        if (deltaY != 0f)
        {
            player.transform.position += new Vector3(0f, deltaY, 0f);
        }

        previousWindowY = currentWindowY;
    }*/
    public void OnChangeView()
    {
        if (IsOverView)
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
    void Pre_SetWindowState()//CheckWindowState()呼ぶ前に必ず呼ぶ　初期化用関数
    {
        //プレイヤー処理
        Vector2 playerScreenPos = WindowManager.overCam.WorldToScreenPoint(player.transform.position);
        //playerScreenPos = new Vector2(playerScreenPos.x - Screen.width / 2, playerScreenPos.y);
        Debug.Log("playerScreenPos" + playerScreenPos.x + "," + playerScreenPos.y);
        Debug.Log("Mouse:" + Input.mousePosition.x + "," + Input.mousePosition.y);
        List<float> distList = new List<float>();
        isMoveplayer = false;
        int backwindowIndex = (int)(player.transform.position.y / distBetWindow);
        windowIndex = windows_statestore.Count - 1 - backwindowIndex;
        if (windowIndex < 0) windowIndex = 0;
        if (windows_statestore[windowIndex].CheckWindowState(playerScreenPos))
        {
            Debug.LogWarning("player on window");
            current = windows_statestore[windowIndex];
            isMoveplayer = true;
        }
        else Debug.LogWarning("player not on window"); current = windows_statestore[0];//これはダミー
        oldWindowPos = windows_statestore[windowIndex].transform.position.y;
    }
    public void SetWindowState()
    {
        player_chara.enabled = false;


        float pre_Height = -distBetWindow;

        // Windowを再配置
        for (int i = windows_statestore.Count - 1; i >= 0; i--)
        {
            Transform windowTransform = windows_statestore[i].transform;

            windowTransform.position = new Vector3(
                windowTransform.position.x,
                pre_Height + distBetWindow,
                windowTransform.position.z
            );

            pre_Height = windowTransform.position.y;
        }


        foreach (Window state in windows_statestore)
        {
            state.isTopMost = false;
        }

        windows_statestore[0].isTopMost = true;

        // 並び替え後のcurrentIndexだけ更新
        currentIndex = windows_statestore.IndexOf(currentWindow);
        //player処理
        if (isMoveplayer)
        {
            float newWindowPos = current.transform.position.y;
            float deltaY = newWindowPos - oldWindowPos;
            player.transform.position += new Vector3(0f, deltaY + 1.8f, 0f);
        }
        player_chara.enabled = true;
    }

    public void FocusWindow()//ウインドウをクリックして最前列に
    {
        //クリック位置をスクリーン座標→ワールド座標に変換してxz成分だけ使って判定　配列0(一番上)から順にやってく
        Vector3 mousePos = Input.mousePosition;
        Pre_SetWindowState();
        foreach (Window state in windows_statestore)
        {
            state.Pre_SetWindowState();
        }
        for (int i = 0; i <= UIobjects.Count - 1; i++)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(UIobjects[i], mousePos))
            {
                windows_statestore[0].isTopMost = true;//UIオブジェクトの上にいるときは最前面のウインドウを最前面の状態にする
                windows_statestore[0].ChangeWindowState();
                Debug.Log("mouse on UI");
                return;//UIオブジェクトの上にいるときはウインドウ操作をしない
            }
        }
        for (int i = 0; i <= windows_statestore.Count - 1; i++)
        {
            bool istarget = windows_statestore[i].SetWindowState(mousePos);
            windows_statestore[i].ChangeWindowState();
            if (istarget)
            {
                Window target = windows_statestore[i];
                windows_statestore.RemoveAt(i);
                windows_statestore.Insert(0, target);
                SetWindowState();
                AppIndex = target.AppIndex;
                //暫定
                changeIndexState?.Invoke();
                break;
            }
        }

    }
    public void ChangeViewMode(bool isOverView)//表示を変更 引数は変更先が俯瞰か否か
    {
        if (isOverView)//俯瞰視点
        {
            IsOverView = true;
        }
        else
        {
            IsOverView = false;
        }
        changeVisualState?.Invoke();
    }
    public void EnableWindowAsNewWindow(GameObject newWindow)
    {
        newWindow.SetActive(true);
        Window newState = newWindow.GetComponent<Window>();

        if (windows_statestore.Contains(newState))
        {
            if(windows_statestore[0] == newState)
            {
                return;
            }
            windows_statestore.Remove(newState);
        }

        windows_statestore.Insert(0, newState);

        // 新しく開いたWindowを現在Windowにする
        currentWindow = newState;
        currentIndex = 0;

        AppIndex = newState.AppIndex;

        SetWindowState();
        foreach (Window state in windows_statestore)
        {
            state.ChangeWindowState();
        }

        changeIndexState?.Invoke();
    }
    public void CloseWindow(Window targetWindow)
    {
        if (windows_statestore.Contains(targetWindow))
        {
            windows_statestore.Remove(targetWindow);
            targetWindow.gameObject.SetActive(false);
            SetWindowState();
            changeIndexState?.Invoke();
        }
    }
    /*public void EnableWindowAsOpendWindow(GameObject newWindow)
    {
        
    }*/
    /*public void CreateWindow(int WindowNumber,Vector2 position)
    {
        var window = Instantiate(instantiateObject[WindowNumber]).GetComponent<Window>();
        window.gameObject.transform.position = new Vector3(position.x,0,position.y);
        windows_statestore.Insert(0,window);
        SetWindowState();
    }*/
}
