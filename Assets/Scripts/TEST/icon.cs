using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;

public class Icon : MonoBehaviour//iconにアタッチ
{
    public bool isStarting;
    public bool isLaunched;
    public int BoundTimes;//バウンドする回数
    public bool isEndlessBound;
    public bool isAnimation;//アニメーション再生中？
    public bool allowStarting;


    [SerializeField] float upDuration;
    [SerializeField] float downDuration;
    [SerializeField] float WaitTimes;
    [SerializeField] Ease easeTypeMae, easeTypeAto;
    public Camera OverViewCamera;//俯瞰かめら
    [SerializeField] float boundHeight;
    [SerializeField] public float Rotation { get; private set; }//x軸のみ　ギミック専用　変更はChangeRotation()
    public float originalWidth;//回転後俯瞰で

    //public bool isMinimumWindow;//最小化ウインドウか？(ジニーエフェクトの問題あるんで検討中)
    //public Vector2 originalPosition;//左上の座標(スクリーン)　=> ウインドウ位置を補正しないこと前提
    //public float width,height;//横幅・縦幅 
    //float minX,minY,maxX,maxY;//ウインドウ各端
    public RectAngleSet rect;
    [SerializeField] GameObject status;//アイコン下の三角
    [SerializeField] bool isText;
    [SerializeField] Renderer Text;
    [SerializeField] float fadeTime;
    public bool isUsingExp;//説明テキストを出すか？
    [SerializeField] List<Material> targetMaterial;
    [SerializeField] Color pushingColor;
    bool ispushing = false;
    //アプリ起動関連
    [SerializeField] GameObject window;
    [SerializeField] WindowManager Manager;
    [SerializeField] bool isUnLaunchedIcon;
    [SerializeField] bool noBounceWhenLaunched = true; // 起動済みならバウンドしない
    [SerializeField] SignalButton signal;
    [SerializeField] bool bounceOnly; // バウンドだけして起動しない
    Window targetWindow;

    void Start()
    {
        //StartCoroutine(DoBounce());
        //Window.csから移植
        OverViewCamera = WindowManager.overCam;
        Manager = GameObject.FindWithTag("Manager").GetComponent<WindowManager>();
        originalWidth = this.transform.localScale.z;
        try
        {
            if (isUsingExp)
            {
                Text.material.color = new Color(Text.material.color.r, Text.material.color.g, Text.material.color.b, 0);
            }
            //transform.GetChild(0).GetComponent<Renderer>();
        }
        catch
        {
            Debug.LogError("テキストの子オブジェクトがねぇ," + this.gameObject.name);
        }
        //ウインドウサイズの取得設定
        rect = FunctionSet.GetRectAngle(this.gameObject, WindowManager.overCam);
        /*MeshFilter mf = this.GetComponent<MeshFilter>();

        Vector3[] vertices = mf.mesh.vertices;

        minX = float.MaxValue;//ウインドウ左端
        maxX = float.MinValue;//右端

        minY = float.MaxValue;//下端
        maxY = float.MinValue;//上端

        foreach (Vector3 v in vertices)
        {
            // ローカル→ワールド
            Vector3 world = this.transform.TransformPoint(v);

            // ワールド→スクリーン
            Vector3 screen = OverViewCamera.WorldToScreenPoint(world);

            minX = Mathf.Min(minX, screen.x);
            maxX = Mathf.Max(maxX, screen.x);

            minY = Mathf.Min(minY, screen.y);
            maxY = Mathf.Max(maxY, screen.y);
        }

        width = maxX - minX;
        height = maxY - minY;

        // Unityのスクリーン座標は左下原点なので左上座標に変換
        Vector2 leftTop = new Vector2(
            minX,
            Screen.height - maxY
        );*/

        targetMaterial.Add(this.GetComponent<Renderer>().material);
        if(status != null) bounceOnly = false;
        if(status != null)
        {
            status.SetActive(false);
            if(window.activeSelf)
            {
                status.SetActive(true); 
                isLaunched = true;
            } 
            else
            {
                status.SetActive(false);
                isLaunched = false;
            }
        }
        if(window != null)
        {
            targetWindow = window.GetComponent<Window>();
        }
        Manager.OnEndTransition+=redoSetRect;
    }
    void redoSetRect()
    {
        rect = FunctionSet.GetRectAngle(this.gameObject, WindowManager.overCam);
    }
    void Update()
    {
        var mousePos = Input.mousePosition;
        bool isactive = mousePos.x >= rect.minX &&
                        mousePos.x <= rect.maxX &&
                        mousePos.y >= rect.minY &&
                        mousePos.y <= rect.maxY;//ボタンの内側判定
        if (Input.GetMouseButtonDown(0) && !isAnimation && isactive)
        {
            foreach (var target in targetMaterial)
            {
                target.color = pushingColor;
            }
        }
        else if (Input.GetMouseButtonUp(0) && !isAnimation)
        {
            if (isactive)
            {
                foreach (var target in targetMaterial)
                {
                    target.color = new Color(1, 1, 1, 1);
                }
            }
            StartCoroutine(ClickButtonDown(isactive));
        }
        else if (isUsingExp)
        {
            if (isactive)
            {
                if (!isText)
                {
                    FadeInText(); //マウスオーバー時 
                }
            }
            else
            {
                if (isText)
                {
                    FadeOutText();
                }
            }
        }

    }
    public void StartorBackToApp()
    {
        StartCoroutine(StartorBackToAppCoroutine());
    }
    IEnumerator StartorBackToAppCoroutine()
    {
        if(!allowStarting||isLaunched)
        {
            Manager.EnableWindowAsNewWindow(window);
            yield break;
        }
        //window.SetActive(true);
        yield return StartCoroutine(DoBounce());
        if (!isLaunched)
        {
            status.SetActive(true);
        }
        isLaunched = true;
        yield return new WaitForSeconds(0.8f);
        Manager.EnableWindowAsNewWindow(window);
    }
    IEnumerator ClickButtonDown(bool isactive)
    {
        if (!allowStarting || !isactive)
            yield break;
        if(targetWindow ! == null)
        {
            //if(targetWindow.isTopMost) => なんか色々競合して直すのもだりぃのでmanagerの参照と比較
            if(Manager.windows_statestore[0] == targetWindow)
            {
                yield break;
            }
        }
        // 起動済みならウインドウを最前面へ
        if (isLaunched)
        {
            Manager.EnableWindowAsNewWindow(window);
            yield break;
        }

        // ゴミ箱などは起動しない
        if (isUnLaunchedIcon)
            yield break;

        // 初回起動
        // バウンド
        yield return StartCoroutine(DoBounce());

        // バウンドのみ
        if (bounceOnly)
            yield break;

        // 起動
        OnStarted();
    }
    void FadeInText()
    {
        isText = true; isAnimation = true;
        var sequence = DOTween.Sequence();
        sequence.Append(Text.material.DOFade(1.0f, fadeTime));
        sequence.Play().OnComplete(() => isAnimation = false);
    }
    void FadeOutText()
    {
        isAnimation = true;
        var sequence = DOTween.Sequence();
        sequence.Append(Text.material.DOFade(0.0f, fadeTime));
        sequence.Play().OnComplete(() => { isText = false; isAnimation = false; });
    }
    IEnumerator DoBounce()
    {
        isAnimation = true;
        if (isEndlessBound)
        {
            while (true)
            {
                yield return StartCoroutine(Bounce());
                if (!isEndlessBound) break;
            }
        }
        else
        {
            for (int i = 0; i < BoundTimes; i++)
            {
                yield return StartCoroutine(Bounce());
            }

        }
        isAnimation = false;
    }
    void OnStarted()
    {
        if (!isLaunched)
        {
            status.SetActive(true);

        }
        isLaunched = true;
        //window.SetActive(true);
        Manager.EnableWindowAsNewWindow(window);
    }

    IEnumerator Bounce()
    {
        transform.DOKill();

        Vector3 basePos = transform.position;

        Sequence seq = DOTween.Sequence();

        float currentHeight = boundHeight;

        /*for(int i = 0; i < BoundTimes; i++)
        {*/
        // 上昇
        seq.Append(
            transform.DOMoveZ(
                basePos.z + currentHeight,
                upDuration
            ).SetEase(easeTypeMae)
        );

        // 落下
        seq.Append(
            transform.DOMoveZ(
                basePos.z,
                downDuration
            ).SetEase(easeTypeAto)
        );
        //}

        yield return seq.WaitForCompletion();
        yield return new WaitForSeconds(WaitTimes);
        transform.position = basePos;
    }
    public void QuitApp()
    {
        if (isLaunched)
        {
            status.SetActive(false);
        }
        isLaunched = false;
    }
}
