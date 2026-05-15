using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;


public class Dock : MonoBehaviour
{
    [SerializeField] GameObject AquaDockBase;//ドックの基底部分(羅線入ってるやつ) Leopardについては下？
    [SerializeField] GameObject LeopardDockBase;//leopard 3Dと2Dで切り替える？ 2DではDock縮ませるとアスペクト上遠近感が縮まり遺憾
    [SerializeField] static bool isAqua;//~Tigerと以後の区別
    [SerializeField] List<GameObject> icons;//インデックスは左から順に
    [SerializeField] float xScale;//Dockの横スケール
    [SerializeField] float iconSize;//ここのサイズは横幅 隣との距離
    [SerializeField] float IconRemoveDuration;//アイコン削除の時にかかる時間
    [SerializeField] float bordersize;
    [SerializeField] Renderer Dock_render;
    [SerializeField] List<Renderer> icons_render;
    //debug
    public bool isDelete;
    public int deleteindex;

    void Start()
    {
        xScale = this.gameObject.transform.localScale.x;//半分
        Dock_render = GetComponent<Renderer>();
        for(int i = 0;i<icons.Count;i++)
        {
            icons_render.Add(icons[i].GetComponent<Renderer>());
        }
    }
    void Update()
    {
        if(isDelete)
        {
            DeleteIcon(deleteindex);
            isDelete = false;
        }
    }
    public void DeleteIcon(int deleteIndex)
    {
        xScale = this.gameObject.transform.localScale.x;
        var sequence = DOTween.Sequence();
        int oldCount = icons.Count();
        GameObject deleteicon = icons[deleteIndex];
        icons.RemoveAt(deleteIndex); icons_render.RemoveAt(deleteIndex);
        xScale= xScale*((float)icons.Count/oldCount);
        Debug.Log(xScale);
        sequence.Append(this.gameObject.transform.DOScaleX(xScale,IconRemoveDuration));
        sequence.Join(deleteicon.GetComponent<Renderer>().material.DOFade(0f,IconRemoveDuration));
        SortIcons();
        float dockCenter = Dock_render.bounds.center.x;
        float width = Dock_render.bounds.size.x*(float)icons.Count/oldCount;
        float tmp_totaliconsize = 0;
        for(int i = 0;i<icons.Count;i++)
        {
            tmp_totaliconsize+=icons_render[i].bounds.size.x;
        }
        float TotalMargin = width -tmp_totaliconsize;
        float margin = TotalMargin/(icons.Count+1);//植木算
        float currentPos = dockCenter-width/2;
        for(int i = 0;i<icons.Count;i++)
        {
            currentPos+= margin;
            currentPos += icons_render[i].bounds.size.x / 2f;
            sequence.Join(icons[i].transform.DOMoveX(currentPos,IconRemoveDuration));
            currentPos+=icons_render[i].bounds.size.x/2f;
        }
        sequence.Play();
    }
    
    void SortIcons()
    {
    }
    public void AddNewIcon(int insertindex,GameObject target) //未完！！
    {
        int oldCount = icons.Count();
        icons.Insert(insertindex,target);
        xScale= xScale*(icons.Count/oldCount);
        this.gameObject.transform.localScale = new Vector3(xScale,this.gameObject.transform.localScale.y,this.gameObject.transform.localScale.z);
        var sequence = DOTween.Sequence();
        for(int i = 0;i<icons.Count;i++)
        {
            //icons[i].transform.position = new Vector3()
        }
    }
}