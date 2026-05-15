using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Enermy_shikigami : MonoBehaviour //スライダーに改造
{
    public Camera OverViewCamera;//俯瞰かめら
    public float width;//横幅・縦幅 
    float minX,maxX;//スライダー範囲

    void Start()
    {
        OverViewCamera = GameObject.FindWithTag("OverViewCamera").GetComponent<Camera>();
        MeshFilter mf = this.GetComponent<MeshFilter>();

        Vector3[] vertices = mf.mesh.vertices;

        minX = float.MaxValue;//左端
        maxX = float.MinValue;//右端

        foreach (Vector3 v in vertices)
        {
            // ローカル→ワールド
            Vector3 world = this.transform.TransformPoint(v);

            // ワールド→スクリーン
            Vector3 screen = OverViewCamera.WorldToScreenPoint(world);

            minX = Mathf.Min(minX, screen.x);
            maxX = Mathf.Max(maxX, screen.x);
        }
        width = maxX - minX;
    }
    void Update()
    {
        
    }
}