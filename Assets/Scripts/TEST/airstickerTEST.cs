using AirSticker.Runtime.Scripts;
using UnityEngine;

/// <summary>
/// マウスクリックした位置にランダムなデカール（投影ステッカー）を表示するデモクラス。
/// AirStickerProjector を使って receiverObjects（複数のオブジェクト）にデカールを貼り付けます。
/// </summary>
public class airstickersystemTEST : MonoBehaviour
{
    /*[Header("デカール設定")]

    [Tooltip("使用するデカールマテリアルの配列（ランダムに選ばれます）")]
    [SerializeField] private Material[] shotDecalMaterials;

    [Tooltip("各マテリアルに対応するデカールサイズ（Vector3）")]
    [SerializeField] private Vector3[] projectorSize;

    [Header("ターゲット")]

    [Tooltip("デカールを貼り付ける対象のオブジェクト群（複数可）")]
    [SerializeField] private GameObject[] receiverObjects;

    /// <summary>
    /// マウスクリック（左ボタン）でデカールを発射・貼り付け。
    /// </summary>
    private void Update()
    {
        if (Input.GetMouseButtonUp(0)) // 左クリックを離したとき
        {
            // マウス位置からレイを生成
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Raycast によるヒット情報取得
            var max_distance = 100f;
            if (Physics.Raycast(ray, out RaycastHit hit_info, max_distance))
            {
                // ヒットしたオブジェクトが、receiverObjects のどれかか確認
                foreach (var receiverObject in receiverObjects)
                {
                    // 自身または子オブジェクトであるかを確認
                    if (hit_info.collider.gameObject == receiverObject ||
                        hit_info.collider.transform.IsChildOf(receiverObject.transform))
                    {
                        // デカール用の空オブジェクトを生成
                        var projectorObject = new GameObject("Decal Projector");

                        // ヒット地点に少しめり込ませた位置に配置（Zファイティング防止）
                        projectorObject.transform.position = hit_info.point + Camera.main.transform.forward * -0.1f;

                        // ランダムにマテリアルとサイズを選択
                        var matNo = Random.Range(0, shotDecalMaterials.Length);
                        var size = projectorSize[matNo] * 0.5f; // 半サイズで使用

                        // AirStickerProjector を使用してデカールを生成・投影
                        var projector = AirStickerProjector.CreateAndLaunch(
                            projectorObject,                    // 投影元オブジェクト
                            receiverObject,                     // 投影対象（貼り付け対象）
                            shotDecalMaterials[matNo],          // 使用マテリアル
                            size.x, size.y, size.z,             // 幅・高さ・奥行き
                            true,                               // 即時起動フラグ
                            result => { Destroy(projectorObject); } // コールバック（デカール投影後にオブジェクト削除）
                        );

                        break; // 一度貼ったら終了（他のオブジェクトには貼らない）
                    }
                }
            }
        }
    }*/
}