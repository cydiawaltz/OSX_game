using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Collections;
using System;
using DG.Tweening;


public class menubar : MonoBehaviour//子側　
{
    public bool isOS9;//OS9のメニューバーか否か 判定はGetMouseUpで消す
    public bool isMenubarEnable = false;//メニューバーが有効状態か
    public List<Renderer> elements;//左から順にアサイン プルダウンした時のやつ
    public List<RectAngleSet> elementsHit = new List<RectAngleSet>();
    public List<Renderer> CommandMenu;//開いた時に出てくるやつ　 materialはtransparentで 
    WindowManager manager;
    public int enableIndex;
    [SerializeField] float ComMenuFadeDuration;//コマンドメニューのフェードアウト時間
    [SerializeField] bool canTouch = true;//メニュー要素のフェード中？
    public Ease ease;
    //debug
    public int frameCount;
    bool isSkipProcess;
    int DownFrameCount = 0;
    
    void Start()
    {
        manager = GameObject.FindWithTag("Manager").GetComponent<WindowManager>();
        //manager.ClickDown += ClickDown;
        for(int i = 0; i < elements.Count; i++)
        {
            elementsHit.Add(FunctionSet.GetRectAngle(elements[i].gameObject,WindowManager.overCam));
            Debug.Log("メニューバーの長方形"+i);
        }
        foreach(var element in elements)
        {
            element.enabled = false;//プルダウンした時だけ入れる感じで
        }
        foreach(var command in CommandMenu)
        {
            command.material.color = new Color(1,1,1,0);
        }
        /*try
        {
            CommandMenu = new List<Renderer>(elements.Count);
            for(int i = 0; i < elements.Count; i++)
            {
                CommandMenu[i] = elements[i].transform.GetChild(0).GetComponent<Renderer>();
            }
        }
        catch(Exception e)
        {
            Debug.LogWarning("command Menuがねぇ"+e+"及び:"+gameObject.name);
        }*/
        manager.OnEndTransition+=redoSetRect;
    }
    void redoSetRect()
    {
        for(int i = 0; i < elementsHit.Count; i++)
        {
            //elementsHit.Add(FunctionSet.GetRectAngle(elements[i].gameObject,WindowManager.overCam));
            elementsHit[i] = FunctionSet.GetRectAngle(elements[i].gameObject,WindowManager.overCam);
            Debug.Log("メニューバーの長方形"+i);
        }
    }
    
    void Update()
    {
        //frameCount++;
        //var mousePos = Input.mousePosition;
        /*isSkipProcess = false;
        if(isMenubarEnable)
        {
            int oldIndex = enableIndex;
            for(int i = 0; i < elementsHit.Count; i++)
            {
                if(mousePos.x >= elementsHit[i].minX && mousePos.x <= elementsHit[i].maxX && mousePos.y >= elementsHit[i].minY && mousePos.y <= elementsHit[i].maxY)
                {
                    enableIndex = i;
                    isSkipProcess = true;
                    break;
                }
            }
            if(oldIndex != enableIndex)
            {
                ChangeElementsState(oldIndex,enableIndex);
                isSkipProcess = true;
            }
        }
        //clickdownの中身はここにあった
        if(Input.GetMouseButton(0)&&!isSkipProcess)
        {
            bool tmp_skipProcess = false;
            if(DownFrameCount == 0)//押した一回だけ（GetMouseDownの代わり）
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
                        tmp_skipProcess = true;
                    }
            }
            else if(!tmp_skipProcess)
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
            DownFrameCount ++;
        }
        else
        {
            DownFrameCount = 0;
        }
        if(Input.GetMouseButtonUp(0))//OS9のメニューバーはクリック終了で消える+1fラグあり？
        {
            Debug.Log("マウスup(OS9)");
            if(isOS9)
            {
                isMenubarEnable = false;
            }
        }
        */
        Vector2 mousePos = Mouse.current.position.ReadValue();
        isSkipProcess = false;
        if(isMenubarEnable&&canTouch)
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
                //StartCoroutine(ChangeElementsState(oldIndex,enableIndex));
                isSkipProcess = true;
            }
        }
        if(Input.GetMouseButtonDown(0)&&!isSkipProcess)//開ける時
        {
            if(DownFrameCount == 0)//押した一回だけ（GetMouseDownの代わり）
            {
                Debug.Log("クリック(menubar.cs)"+gameObject.name);
                if(isMenubarEnable)
                {
                    CloseElements();
                }
                else
                {
                    for(int i = 0; i < elementsHit.Count; i++)
                    {
                        if(mousePos.x >= elementsHit[i].minX && mousePos.x <= elementsHit[i].maxX && mousePos.y >= elementsHit[i].minY && mousePos.y <= elementsHit[i].maxY)
                        {
                            StartCoroutine(OpenElements(i));
                            enableIndex = i;
                            break;
                        }
                    }
                }
                DownFrameCount ++;
            }
        }
        else
        {
            DownFrameCount = 0;
        }
        if(Mouse.current.leftButton.wasReleasedThisFrame)//OS9のメニューバーはクリック終了で消える+1fラグあり？
        {
            if(isOS9)
            {
                isMenubarEnable = false;
            }
        }
        
    }
    IEnumerator OpenElements(int index)
    {
        canTouch = false;
        elements[index].enabled = true;
        Debug.Log("open");
        isMenubarEnable = true;
        //2fずらして詳細メニューの表示処理 当時のiMacG3の重さを鑑みると共通で30fps前提で組む => 1/15s
        yield return new WaitForSeconds(1/15f);
        if(CommandMenu.Count>index)
        {
            CommandMenu[index].enabled = true;
            CommandMenu[index].material.color = new Color(1,1,1,0);
            CommandMenu[index].material.DOColor(new Color(1,1,1,1),ComMenuFadeDuration).SetEase(ease).OnComplete(()=>{
                canTouch = true;
            });
        }
        else{ canTouch = true;}
    }
    void CloseElements()
    {
        canTouch = false;
        foreach(var element in elements)
        {
            if(element.enabled){element.enabled = false;}
        }
        //詳細メニューの非表示処理
        Debug.Log("close");
        isMenubarEnable = false;
        if(CommandMenu.Count>enableIndex)
        {
            CommandMenu[enableIndex].material.DOColor(new Color(1,1,1,0),ComMenuFadeDuration).SetEase(ease).OnComplete(()=>{
                canTouch = true;
                CommandMenu[enableIndex].enabled = false;
                CommandMenu[enableIndex].material.color = new Color(1,1,1,0);
            });
        }
        else
        {
            canTouch = true;
        }
    }
    void ChangeElementsState(int oldIndex,int newIndex)
    {
        var sequence = DOTween.Sequence();
        elements[oldIndex].enabled = false;
        elements[newIndex].enabled = true;
        if(CommandMenu.Count>oldIndex)
        {
            CommandMenu[oldIndex].material.color = new Color(1,1,1,1);
            sequence.Append(CommandMenu[oldIndex].material.DOColor(new Color(1,1,1,0),ComMenuFadeDuration)).SetEase(ease).OnComplete(()=>{
                CommandMenu[oldIndex].enabled = false;
            });
        }
        if(CommandMenu.Count>newIndex)
        {
            CommandMenu[newIndex].material.color = new Color(1,1,1,0);
            CommandMenu[newIndex].enabled = true;
            sequence.Append(CommandMenu[newIndex].material.DOColor(new Color(1,1,1,1),ComMenuFadeDuration)).SetEase(ease);
        }
        Debug.Log("state change");
    }
    
    void OnDisable()
    {
        isMenubarEnable = false;
        for(int i = 0; i < elements.Count; i++)
        {
            elements[i].enabled = false;
        }
        Debug.Log("Menubar Disable");
    }
}

