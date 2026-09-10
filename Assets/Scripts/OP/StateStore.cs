using UnityEngine;

public class StateStore : MonoBehaviour
{
    public int score;
    public int bulletCount;
    public float time;
    public int HP;
    public int MaxHP;
    bool isGame;
    public float difficulty;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if(isGame)
        {
            time += Time.deltaTime;
        }
    }
    public void StartGame()
    {
        isGame = true;
    }
    public void EndGame()
    {
        isGame = false;
        score = (int)(9999 - (5-difficulty)*(1/4) * (40*time + 50*bulletCount + 1200*(MaxHP-HP)/MaxHP));
    }
}
