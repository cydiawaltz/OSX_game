using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Pre : MonoBehaviour
{
    [SerializeField] StateStore stateStore;
    [SerializeField] Button[] buttons;
    [SerializeField] Texture[] ClickZoneTex;//0:無効 1:有効
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
                stateStore.MaxHP = 20;
                stateStore.enemyHP = 10;
                break;
            case Difficulty.Normal:
                diff = 2;
                stateStore.MaxHP = 10;
                stateStore.enemyHP = 15;
                break;
            case Difficulty.Hard:
                diff = 3;
                stateStore.MaxHP = 5;
                stateStore.enemyHP = 20;
                break;
            case Difficulty.Lunatic:
                diff = 4;
                stateStore.MaxHP = 1;
                stateStore.enemyHP = 20;
                break;
            default:
                diff = 2;
                stateStore.MaxHP = 10;
                stateStore.enemyHP = 15;
                break;
        }
        stateStore.difficulty = diff;
    }
    public void StartGame()
    {
        Destroy(GameObject.FindWithTag("Source"));
        //stateStore.stage = SceneManager.GetActiveScene().name.Replace("Pre","");
        if(stateStore != null)
        {
            SceneManager.LoadScene(stateStore.stage);
        }
        else
        {
            SceneManager.LoadScene("10.1");
        }
        
    }
    public void ChangeTexture(int index)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i == index)
            {
                buttons[i].GetComponent<RawImage>().texture = ClickZoneTex[1];
            }
            else
            {
                buttons[i].GetComponent<RawImage>().texture = ClickZoneTex[0];
            }
        }
        switch(index)
        {
            case 0:
                SetDifficulty(Difficulty.Easy);
                break;
            case 1:
                SetDifficulty(Difficulty.Normal);
                break;
            case 2:
                SetDifficulty(Difficulty.Hard);
                break;
            case 3:
                SetDifficulty(Difficulty.Lunatic);
                break;
        }
    }
}
