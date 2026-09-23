using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NameEntry : MonoBehaviour
{
    [SerializeField] StateStore stateStore;
    [SerializeField] Text stage,score;
    [SerializeField] InputField namef, comment;
    [SerializeField] AudioSource source;
    [SerializeField] AudioClip[] clips;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stateStore = GameObject.FindWithTag("State").GetComponent<StateStore>();
        int scoreint = Mathf.Clamp(stateStore.score, 0, 9999);
        stage.text = stateStore.stage;
        score.text = scoreint.ToString();
        switch(stage.text)
        {
            case "10.1":
                source.clip = clips[0];
                break;
            case "10.3":
                source.clip = clips[1];
                break;
            case "10.4":
                source.clip = clips[2];
                break;
            default:
                source.clip = clips[0];
                break;
        }
        source.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SaveRanking()
    {
        FunctionSet.AddRanking(namef.text, stateStore.stage, stateStore.score, comment.text);
        //SceneManager.LoadScene("Home");
        Application.Quit();
    }
    public void Cancel()
    {
        //SceneManager.LoadScene("Home");
        Application.Quit();
    }
}
