using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
//関連する型群・関数ぐん
public class WindowState
{
    int width, Height;
    bool isTopMost;//最前列に表示されているobjか　ボタン・カラムの透明化と影
}
public class RectAngleSet
{
    public float width, height;//横幅・縦幅 
    public float minX, minY, maxX, maxY;//ウインドウ各端
}
public class RankingData
{
    public string name;
    public string stage;
    public int score;
    public string comment;

    public RankingData(string name, string stage, int score, string comment)
    {
        this.name = name;
        this.stage = stage;
        this.score = score;
        this.comment = comment;
    }
}
public class FunctionSet
{
    public static RectAngleSet GetRectAngle(GameObject target, Camera OverViewCamera)
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
    //ランキング
    private const string RankingFileName = "ranking.txt";


    // 実際に読み書きするランキングファイルのパス
    public static string GetRankingFilePath()
    {
        return Path.Combine(
            Application.persistentDataPath,
            RankingFileName
        );
    }


    //==================================================
    // ランキング読み込み
    //==================================================

    public static List<RankingData> LoadRanking()
    {
        List<RankingData> rankingList = new List<RankingData>();

        string path = GetRankingFilePath();

        // 初回起動時など、persistentDataPathに存在しない場合
        // StreamingAssetsからコピー
        if (!File.Exists(path))
        {
            string sourcePath = Path.Combine(
                Application.streamingAssetsPath,
                RankingFileName
            );

            if (File.Exists(sourcePath))
            {
                File.Copy(sourcePath, path);
            }
            else
            {
                ShowMessage(
                    "ランキングファイルが存在しません: " + sourcePath
                );

                return rankingList;
            }
        }

        try
        {
            // UTF-8で読み込み
            Encoding encoding = Encoding.UTF8;

            string[] lines = File.ReadAllLines(path, encoding);

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] data = line.Split(':');

                // 名前・ステージ・スコア・コメント
                if (data.Length < 4)
                {
                    ShowMessage(
                        "ランキングデータの形式が不正です: " + line
                    );

                    continue;
                }

                if (!int.TryParse(data[2], out int score))
                {
                    ShowMessage(
                        "スコアが数値ではありません: " + line
                    );

                    continue;
                }

                rankingList.Add(
                    new RankingData(
                        data[0],
                        data[1],
                        score,
                        data[3]
                    )
                );
            }

            // スコアの高い順
            rankingList.Sort(
                (a, b) => b.score.CompareTo(a.score)
            );
        }
        catch (Exception e)
        {
            ShowMessage(
                "ランキング読み込み失敗: " + e.Message
            );
        }

        return rankingList;
    }


    //==================================================
    // ランキング追加・保存
    //==================================================

    public static void AddRanking(
        string name,
        string stage,
        int score,
        string comment
    )
    {
        List<RankingData> rankingList = LoadRanking();

        rankingList.Add(
            new RankingData(
                name,
                stage,
                score,
                comment
            )
        );

        // スコアの高い順
        rankingList.Sort(
            (a, b) => b.score.CompareTo(a.score)
        );

        SaveRanking(rankingList);
    }


    public static void SaveRanking(List<RankingData> rankingList)
    {
        string path = GetRankingFilePath();

        try
        {
            // UTF-8で保存
            Encoding encoding = Encoding.UTF8;

            using (StreamWriter writer = new StreamWriter(
                path,
                false,
                encoding
            ))
            {
                foreach (RankingData data in rankingList)
                {
                    // タブ・改行によって1レコードが壊れないようにする
                    string name = CleanRankingText(data.name);
                    string stage = CleanRankingText(data.stage);
                    string comment = CleanRankingText(data.comment);

                    writer.WriteLine(
                        name + ":" +
                        stage + ":" +
                        data.score + ":" +
                        comment
                    );
                }
            }
        }
        catch (Exception e)
        {
            ShowMessage(
                "ランキング保存失敗: " + e.Message
            );
        }
    }


    //==================================================
    // ランキング取得
    //==================================================

    // 全ランキングを取得
    public static List<RankingData> GetRanking()
    {
        return LoadRanking();
    }


    // 指定順位を取得
    // 0 = 1位
    public static RankingData GetRanking(int index)
    {
        List<RankingData> rankingList = LoadRanking();

        if (index < 0 || index >= rankingList.Count)
            return null;

        return rankingList[index];
    }


    // ランキング件数を取得
    public static int GetRankingCount()
    {
        return LoadRanking().Count;
    }


    //==================================================
    // テキスト処理
    //==================================================

    private static string CleanRankingText(string text)
    {
        if (string.IsNullOrEmpty(text))
            return "";

        return text
            .Replace(":", " ")
            .Replace("\r", " ")
            .Replace("\n", " ");
    }
    public static void ExportRankingToDownloads()
    {
        string sourcePath = GetRankingFilePath();

        if (!File.Exists(sourcePath))
        {
            ShowMessage("ランキングファイルが存在しません: " + sourcePath);
            return;
        }

        string downloadsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "Downloads"
        );

        string destinationPath = Path.Combine(
            downloadsPath,
            RankingFileName
        );

        try
        {
            File.Copy(sourcePath, destinationPath, true);

            ShowMessage(
                "ランキングファイルをダウンロードフォルダに保存しました: "
                + destinationPath
            );
        }
        catch (Exception e)
        {
            ShowMessage(
                "ランキングファイルのエクスポートに失敗しました: "
                + e.Message
            );
        }
    }
    //ダイアログ表示
    public static void ShowMessage(string message)
    {
#if UNITY_STANDALONE_WIN
    System.Windows.Forms.MessageBox.Show(
        message,
        "PineApple",
        System.Windows.Forms.MessageBoxButtons.OK,
        System.Windows.Forms.MessageBoxIcon.Information
    );
#elif UNITY_STANDALONE_OSX
        // macOSではAppleScriptを利用してダイアログを表示
        System.Diagnostics.Process.Start(
            "/usr/bin/osascript",
            "-e 'display dialog \"" +
            message.Replace("\"", "\\\"") +
            "\" with title \"PineApple\" buttons {\"OK\"}'"
        );
#else
    ShowMessage(message);
#endif
    }
}
/* summary
GetRankingFilePath()	ランキングファイルを保存する Application.persistentDataPath のパスを取得します。
LoadRanking()	ranking.txt をUTF-8で読み込み、ランキングデータを List<RankingData> として取得します。読み込み後、スコアの高い順に並べ替えます。
AddRanking()	名前・ステージ・スコア・コメントを新しいランキングとして追加し、スコア順に並べ替えて保存します。
SaveRanking()	List<RankingData> の内容を ranking.txt にUTF-8で書き込みます。
GetRanking()	ランキング全体を List<RankingData> として取得します。
GetRanking(int index)	指定した順位のランキングデータを取得します。0 が1位です。
GetRankingCount()	現在登録されているランキングの件数を取得します。
CleanRankingText()	名前・ステージ・コメントに含まれるタブや改行を空白へ変換し、1行1データの形式が崩れないようにします。
*/