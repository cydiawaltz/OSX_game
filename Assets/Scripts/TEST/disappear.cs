using UnityEngine;
using System.Collections;

public class disappear : MonoBehaviour
{
    [Header("Playerだけ直で参照とおせ")]
    public GameObject pCamera;//負荷軽減のため、プレイヤーはインスタンス参照
    public GameObject oCamera;//俯瞰カメラ
    [SerializeField] float updateTime;//画像を更新する時間
    [SerializeField] Mesh mesh;
    [SerializeField] Vector2[] defaultUV;

    // 現在フレーム
    public int frame = 0;

    [Header("コマ数")]
    // 縦3コマ
    public int frameCount;
    // 何某が消えた時に呼ばれる 位置はinstantiate()で指定
    void Start()
    {
        //pCamera = GameObject.FindWithTag("MainCamera");ここの2行は式神スクリプトの方から飛ばす
        //oCamera = GameObject.FindWithTag("OverViewCamera");
        /*if(oCamera == null)
        {
            oCamera = transform.Find("OverViewCamera").gameObject;
        }*/
        mesh = GetComponent<MeshFilter>().mesh;
        defaultUV = mesh.uv;
        StartCoroutine(Disappear());
    }

    // Update is called once per frame
    void Update()
    {
        if(oCamera.activeSelf)
        {
            transform.LookAt(oCamera.transform);
        }
        else if(pCamera.activeSelf)
        {
            transform.LookAt(pCamera.transform);
        }
    }
    IEnumerator Disappear()
    {
        SetFrame(0);

        while (frame < frameCount - 1)
        {
            yield return new WaitForSeconds(updateTime);

            frame++;
            SetFrame(frame);
        }
        Destroy(gameObject);
    }
    void SetFrame(int frameIndex)
    {
        Vector2[] uv = new Vector2[defaultUV.Length];

        float offsetY = -(1.0f / frameCount) * frameIndex;

        for (int i = 0; i < uv.Length; i++)
        {
            uv[i] = defaultUV[i];
            uv[i].y += offsetY;
        }

        mesh.uv = uv;
    }
}
