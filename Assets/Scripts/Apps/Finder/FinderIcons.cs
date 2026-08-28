using UnityEngine;
using DG.Tweening;
using System.Collections;

public class FinderIcons : MonoBehaviour
{
    WindowManager manager;
    [SerializeField] Window parent;
    [SerializeField] Renderer thisObj;
    [SerializeField] Texture[] textures;//0が通常、1が一回クリック
    [SerializeField] RectAngleSet rectangle;
    public enum OpenFileType{png,mov,pdf}//ReadMeはpdf表記だが内部処理はpngと同じ movはQTPlayer
    [SerializeField] OpenFileType type;//インスペクターでアサイン
    [SerializeField] GameObject Preview,QTplayer;
    [SerializeField] Icon previewIcon,qtIcon;
    public bool isPanther;
    public bool isSelected;
    public float RagTime;//クリックしてから開くまでのラグ
    [Header("ここから先はPanther/Tigerだけ")]
    [SerializeField] Renderer iconObj;//アイコンの上にエフェクト用の平面を重ねる
    [SerializeField] float initialAlpha;
    [SerializeField] float openDuration;
    public float ExpandScale;//ふわっと開く時の最後のスケール
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager = GameObject.FindWithTag("Manager").GetComponent<WindowManager>();
        thisObj = this.GetComponent<Renderer>();
        rectangle = FunctionSet.GetRectAngle(this.gameObject,WindowManager.overCam);
        parent.OnDragEnd += () => { rectangle = FunctionSet.GetRectAngle(this.gameObject,WindowManager.overCam); };
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            var mousePos = Input.mousePosition;
            if(mousePos.x >= rectangle.minX && mousePos.x <= rectangle.maxX && mousePos.y >= rectangle.minY && mousePos.y <= rectangle.maxY)
            {
                if(isSelected)
                {
                    /*switch(type)
                    {
                    case OpenFileType.png:
                    //Previewを起動し、画像表示用のマテリアルのテクスチャを更新
                    break;
                    case OpenFileType.mov:
                    //QTPlayerを起動し、動画用のマテリアルに流す動画を変更する
                    break;
                    case OpenFileType.pdf:
                    //Previewを起動し、PDF表示用のマテリアルのテクスチャを更新
                    OpenPreview();
                    break;
                    }*/
                    OpenPreview();
                    OpenFile();//これは見た目だけ
                    thisObj.material.mainTexture = textures[0];
                    isSelected = false;
                }
                else
                {
                    thisObj.material.mainTexture = textures[1];
                    //OpenFile(); <=?
                    isSelected = true;
                }
            }
            else if(isSelected)
            {
                thisObj.material.mainTexture = textures[0];
                isSelected = false;
            }
        }
    }
    void OpenFile()
    {
        if(isPanther)
        {
            //隠してたiconの上のオブジェクトを有効にし、フェード
            iconObj.material.color = new Color(1,1,1,initialAlpha);
            var sequence = DOTween.Sequence();
            var tmp = iconObj.gameObject.transform.localScale;
            sequence.Append(iconObj.material.DOColor(new Color(1,1,1,0),openDuration))
                    .Join(iconObj.gameObject.transform.DOScale(new Vector3(tmp.x*ExpandScale,1,tmp.z*ExpandScale),openDuration));
        }
    }
    void OpenPreview()
    {
        /*yield return new WaitForSeconds(RagTime);
        //Preview.SetActive(true);
        manager.EnableWindowAsNewWindow(Preview);*/
        previewIcon.StartorBackToApp();
    }
}
