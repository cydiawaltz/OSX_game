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
    [SerializeField] List<Renderer> icons_render;//上のレンダラー
    [SerializeField] float xScale;//Dockの横スケール
    [SerializeField] float iconSize;//ここのサイズは横幅 隣との距離
    [SerializeField] float IconRemoveDuration;//アイコン削除の時にかかる時間
    [SerializeField] float bordersize;
    [SerializeField] Renderer Dock_render;

    //debug
    public bool isDelete;
    public int deleteindex;

    void Awake()
    {
        xScale = this.gameObject.transform.localScale.x;//半分
        Dock_render = GetComponent<Renderer>();
        icons_render = icons.ConvertAll(icon => icon.GetComponent<Renderer>()); // icons_render を一括初期化
        AlignIcons();//<=これAI製なので真っ先にバグ疑
    }

    void Update()
    {
        if (isDelete)
        {
            DeleteIcon(deleteindex);
            isDelete = false;
        }
    }

    public void DeleteIcon(int deleteIndex)
    {
        xScale = this.gameObject.transform.localScale.x;
        var sequence = DOTween.Sequence();
        int oldCount = icons.Count;

        GameObject deleteicon = icons[deleteIndex];
        icons.RemoveAt(deleteIndex);
        icons_render.RemoveAt(deleteIndex);

        xScale *= (float)icons.Count / oldCount;
        sequence.Append(this.gameObject.transform.DOScaleX(xScale, IconRemoveDuration));
        sequence.Join(deleteicon.GetComponent<Renderer>().material.DOFade(0f, IconRemoveDuration));

        float dockCenter = Dock_render.bounds.center.x;
        float width = Dock_render.bounds.size.x * (float)icons.Count / oldCount;

        float tmp_totaliconsize = icons_render.Sum(renderer => renderer.bounds.size.x); // 合計サイズを一括計算
        float TotalMargin = width - tmp_totaliconsize;
        float margin = TotalMargin / (icons.Count + 1);//植木算

        float currentPos = dockCenter - width / 2;
        foreach (var (icon, renderer) in icons.Zip(icons_render, (icon, renderer) => (icon, renderer)))
        {
            currentPos += margin + renderer.bounds.size.x / 2f;
            sequence.Join(icon.transform.DOMoveX(currentPos, IconRemoveDuration));
            currentPos += renderer.bounds.size.x / 2f;
        }

        sequence.Play();
    }
    public void AlignIcons()
{
    float dockCenter = Dock_render.bounds.center.x;
    float width = Dock_render.bounds.size.x;

    float totalIconSize =
        icons_render.Sum(renderer => renderer.bounds.size.x);

    float totalMargin = width - totalIconSize;
    float margin = totalMargin / (icons.Count + 1);

    float currentPos = dockCenter - width / 2;

    foreach (var (icon, renderer) in icons.Zip(
                 icons_render,
                 (icon, renderer) => (icon, renderer)))
    {
        currentPos += margin + renderer.bounds.size.x / 2f;

        Vector3 pos = icon.transform.position;
        pos.x = currentPos;
        icon.transform.position = pos;

        currentPos += renderer.bounds.size.x / 2f;
    }
}

    void SortIcons()
    {
        // 未実装
    }

    public void AddNewIcon(int insertIndex, GameObject target) //未完！！
    {
        int oldCount = icons.Count;
        icons.Insert(insertIndex, target);
        icons_render.Insert(insertIndex, target.GetComponent<Renderer>());

        xScale *= (float)icons.Count / oldCount;
        this.gameObject.transform.localScale = new Vector3(xScale, this.gameObject.transform.localScale.y, this.gameObject.transform.localScale.z);

        var sequence = DOTween.Sequence();
        float dockCenter = Dock_render.bounds.center.x;
        float width = Dock_render.bounds.size.x * (float)icons.Count / oldCount;

        float tmp_totaliconsize = icons_render.Sum(renderer => renderer.bounds.size.x); // 合計サイズを一括計算
        float TotalMargin = width - tmp_totaliconsize;
        float margin = TotalMargin / (icons.Count + 1);//植木算

        float currentPos = dockCenter - width / 2;
        foreach (var (icon, renderer) in icons.Zip(icons_render, (icon, renderer) => (icon, renderer)))
        {
            currentPos += margin + renderer.bounds.size.x / 2f;
            sequence.Join(icon.transform.DOMoveX(currentPos, IconRemoveDuration));
            currentPos += renderer.bounds.size.x / 2f;
        }

        sequence.Play();
    }
}