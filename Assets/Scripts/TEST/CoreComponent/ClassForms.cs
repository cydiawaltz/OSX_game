using UnityEngine;
using System;
//関連する型群・関数ぐん
public class WindowState
{
    int width,Height;
    bool isTopMost;//最前列に表示されているobjか　ボタン・カラムの透明化と影
}
public class RectAngleSet
{
    public float width,height;//横幅・縦幅 
    public float minX,minY,maxX,maxY;//ウインドウ各端
}
public class FunctionSet
{
    public static RectAngleSet GetRectAngle(GameObject target,Camera OverViewCamera)
    {
        RectAngleSet result = new RectAngleSet();
        //ウインドウサイズの取得設定
        MeshFilter mf = target.GetComponent<MeshFilter>();
        Vector3[] vertices = mf.mesh.vertices;

        result.minX = float.MaxValue;//ウインドウ左端
        result.maxX = float.MinValue;//右端

        result.minY = float.MaxValue;//下端
        result.maxY = float.MinValue;//上端

        foreach (Vector3 v in vertices)
        {
            // ローカル→ワールド
            Vector3 world = target.transform.TransformPoint(v);

            // ワールド→スクリーン
            Vector3 screen = OverViewCamera.WorldToScreenPoint(world);

            result.minX = Mathf.Min(result.minX, screen.x);
            result.maxX = Mathf.Max(result.maxX, screen.x);

            result.minY = Mathf.Min(result.minY, screen.y);
            result.maxY = Mathf.Max(result.maxY, screen.y);
        }

        result.width = result.maxX - result.minX;
        result.height = result.maxY - result.minY;

        // Unityのスクリーン座標は左下原点なので左上座標に変換
        Vector2 leftTop = new Vector2(
            result.minX,
            Screen.height - result.maxY
        );
        return result;
    }
}