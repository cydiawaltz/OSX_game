using UnityEngine;
using System.Collections.Generic;

public class menubarController : MonoBehaviour
{
    //これは親側のみ　子側（クリック時のブルーダウン判定など）はmenubar.cs
    [SerializeField] List<GameObject> MenubarBases;//各種メニューバーの親(index管理の関係上、そのindexと対応) 子は左(Appのタイトル)から0
    [SerializeField] bool isEnable;//メニューバーが有効状態か
    [SerializeField] WindowManager manager;
    [SerializeField] int AppIndex;
    void Start()
    {
        manager = GameObject.FindWithTag("Manager").GetComponent<WindowManager>(); 
        manager.changeIndexState += ChangeIndex;
        /*for(int i = 0; i < MenubarBases.Count; i++)
        {
            if(!(i == 0)) MenubarBases[i].SetActive(false);
            else MenubarBases[i].SetActive(true);
        }*/
    }
    void ChangeIndex()
    {
        //oldspeedと同じ形 最後に代入
        MenubarBases[AppIndex].SetActive(false);
        MenubarBases[manager.AppIndex].SetActive(true);
        AppIndex = manager.AppIndex;
    }
    void OnDestroy()
    {
        manager.changeIndexState -= ChangeIndex;
    }
}
