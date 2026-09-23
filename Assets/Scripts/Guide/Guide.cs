using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Guide : MonoBehaviour
{
    StateStore stateStore;
    WindowManager manager;
    [SerializeField] Player player;
    [SerializeField] GameObject arrow, Win, lose, PushP, ChangeVisual;
    [SerializeField] float amplitude = 0.5f;
    [SerializeField] float frequency = 2f;
    bool isArrowEnable;
    Vector3 basePos;
    bool is10_1;
    void Start()
    {
        if(SceneManager.GetActiveScene().name == "10.1")
        {
            is10_1 = true;
        }
        manager = GameObject.FindWithTag("Manager").GetComponent<WindowManager>();
        stateStore = GameObject.FindWithTag("State").GetComponent<StateStore>();
        if(is10_1){arrow.SetActive(false); Win.SetActive(false); lose.SetActive(false); PushP.SetActive(false); ChangeVisual.SetActive(false);}
        manager.OnEndTransition += OnEndTransition;
        basePos = arrow.transform.position;
        player.onwin += () =>
        {
            Win.SetActive(true);
        };
        player.onlose += () =>
        {
            lose.SetActive(true);
        };
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P)&&is10_1)
        {
            PushP.SetActive(false);
        }
        
    }
    public void OnCallVisualButton()
    {
        if(isArrowEnable)
        {
            ChangeVisual.SetActive(false);
            PushP.SetActive(true);
            arrow.SetActive(false);
            isArrowEnable = false;
        }
    }
    void OnEndTransition()
    {
        if (is10_1)
        {
            arrow.SetActive(true);
            ChangeVisual.SetActive(true);
            isArrowEnable = true;
            StartCoroutine(LoopSinCurve());
        }
    }
    IEnumerator LoopSinCurve()
    {
        while (isArrowEnable)
        {
            Vector3 dir = new Vector3(1f, 0f, 1f).normalized;
            float offset = Mathf.Sin(Time.time * frequency) * amplitude;
            arrow.transform.position = basePos + dir * offset;
            yield return null;
        }
    }
}
