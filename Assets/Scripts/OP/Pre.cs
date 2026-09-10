using UnityEngine;
using UnityEngine.SceneManagement;

public class Pre : MonoBehaviour
{
    [SerializeField] GameObject[] tiles;
    [SerializeField] StateStore stateStore;
    bool trigger = false;
    public enum Difficulty
    {
        Easy,Normal,Hard,Lunatic
    }
    void Start()
    {
        stateStore = GameObject.FindWithTag("State").GetComponent<StateStore>();
        if(stateStore == null)
        {
            trigger = true;
        }
    }
    public void SetDifficulty(Difficulty set)
    {
        if(trigger) return;
        int diff;
        switch(set)
        {
            case Difficulty.Easy:
                diff = 1;
                break;
            case Difficulty.Normal:
                diff = 2;
                break;
            case Difficulty.Hard:
                diff = 3;
                break;
            case Difficulty.Lunatic:
                diff = 4;
                break;
            default:
                diff = 2;
                break;
        }
        stateStore.difficulty = diff;
    }
    public void StartGame()
    {
        SceneManager.LoadScene("10.1");
    }
}
