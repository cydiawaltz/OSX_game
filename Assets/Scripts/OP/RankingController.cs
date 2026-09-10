using UnityEngine;
using UnityEngine.UI;

public class RankingController : MonoBehaviour
{
    [SerializeField] RankingData[] data = new RankingData[6];
    [SerializeField] Text[] 
        playername,
        stage,
        score,
        comment;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for(int i=0;i<6;i++)
        {
            data[i] = FunctionSet.GetRanking(i);
            playername[i].text = data[i].name;
            stage[i].text = data[i].stage;
            score[i].text = data[i].score.ToString();
            comment[i].text = data[i].comment;
        }

    }
    public void ExportRanking()
    {
        FunctionSet.ExportRankingToDownloads();
    }
    public void ClickBack()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Home");
    }
}
