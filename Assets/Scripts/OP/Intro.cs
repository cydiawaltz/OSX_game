using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
    [SerializeField] float SkipTime;
    [SerializeField] VideoPlayer vp;
    int phase = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        vp.loopPointReached += Loop;
    }

    // Update is called once per frame
    void Update()
    {
        if((Input.anyKeyDown || Input.GetMouseButtonDown(0))&&phase == 0)
        {
            phase = 1;
            vp.time = SkipTime;
        }
        else if((Input.anyKeyDown || Input.GetMouseButtonDown(0))&&phase == 1)
        {
            Loop(vp);
        }
    }
    void Loop(VideoPlayer source)
    {
        SceneManager.LoadScene("Home");
    }
}
