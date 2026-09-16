using UnityEngine;

public class StateStore : MonoBehaviour
{
    public int score;
    public int bulletCount;
    public float time;
    public int HP;
    public int MaxHP;
    public int enemyHP;
    public string stage;
    bool isGame;
    public float difficulty;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject[] Objects = GameObject.FindGameObjectsWithTag("State");
        if(Objects.Length > 1)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
        }
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
        score = (int)(9999 - difficulty*(1f/4f) * (40f*time + 50f*bulletCount + 1200f*(MaxHP-HP)/MaxHP));
    }
}
