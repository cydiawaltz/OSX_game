using UnityEngine;
using System.Collections.Generic;

public class menubarController : MonoBehaviour
{
    //これは親側のみ　子側（クリック時のブルーダウン判定など）はmenubar.cs
    [SerializeField] List<GameObject> MenubarBases;//各種メニューバーの親(index管理の関係上、そのindexと対応) 子は左(Appのタイトル)から0
    [SerializeField] bool isInSpace;//
    [SerializeField] bool isEnable;//メニューバーが有効状態か
    [SerializeField] WindowManagerTest manager;
    void Start()
    {
        manager = GameObject.FindWithTag("Manager").GetComponent<WindowManagerTest>(); 
        manager.changeIndexState += ChangeIndex;  
        ChangeIndex();
    }
    void ChangeIndex()
    {
        foreach(GameObject menu in MenubarBases)
        {
            menu.SetActive(false);
        }
        MenubarBases[manager.AppIndex].SetActive(true);
    }
}
