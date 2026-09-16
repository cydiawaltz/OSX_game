using UnityEngine;
using UnityEngine.Video;
using System.Collections;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OP : MonoBehaviour
{
    [SerializeField] VideoPlayer vp;
    [SerializeField] AudioSource source;
    [SerializeField] StateStore stateStore;
    [SerializeField] float audioStartFrame;
    [SerializeField] float videoLoopTime;
    [SerializeField] float enableBaseTime;
    [SerializeField] GameObject baseObj;
    [SerializeField] bool isReadingHowto;
    [SerializeField] Image howtoImage;
    [SerializeField] Sprite[] howtoSprites;

    public int howtoSpriteIndex = 0;
    bool trigger = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject[] Objects = GameObject.FindGameObjectsWithTag("State");
        if(Objects.Length > 1)//既にある時
        {
            //Destroy(Objects[0]);
            stateStore = Objects[1].GetComponent<StateStore>();
            Destroy(source.gameObject);
            source = GameObject.FindWithTag("Source").GetComponent<AudioSource>();
            source.Play();
            vp.time = enableBaseTime;
            vp.Play();
        }
        else
        {
            stateStore = GameObject.FindWithTag("State").GetComponent<StateStore>();
            StartCoroutine(StartSet());
            DontDestroyOnLoad(source.gameObject);
        }
        
    }
    void Update()
    {
        //debug
        if(Input.GetKeyDown(KeyCode.D)&&Input.GetKey(KeyCode.LeftShift))
        {
            EnableYoukoso();
        }
        if(vp.time >= enableBaseTime&&!trigger)
        {
            trigger = true;
            EnableYoukoso();
        }
    }
    IEnumerator StartSet()
    {
        baseObj.SetActive(false);
        DisableHowto();
        vp.Stop();
        vp.time = 0;

        // ��ɃC�x���g��o�^
        vp.loopPointReached += Vp_loopPointReached;

        // ���������
        vp.Prepare();

        // ���������܂ő҂�
        while (!vp.isPrepared)
            yield return null;

        // 0�b����Đ�
        vp.time = 0;
        vp.Play();

        // �������w��ʒu����Đ�
        source.time = audioStartFrame;
        source.Play();
    }


    private void Vp_loopPointReached(VideoPlayer source)
    {
        vp.Stop();
        vp.time = videoLoopTime;
        vp.Play();
    }

    void EnableYoukoso()
    {
        baseObj.SetActive(true);
        source.DOFade(0.9f, 10f);
    }
    public void ClickPuma()
    {
        stateStore.stage = "10.1";
        SceneManager.LoadScene("Pre");
    }
    public void ClickPanther()
    {
        stateStore.stage = "10.3";
        SceneManager.LoadScene("Pre");
    }
    public void ClickTiger()
    {
        stateStore.stage = "10.4";
        SceneManager.LoadScene("Pre");
    }
    public void ClickLeopard()
    {
        stateStore.stage = "10.5";
        SceneManager.LoadScene("Pre");
    }
    public void ClickQuit()
    {
        Application.Quit();
    }
    public void ClickReboot()
    {
        Destroy(GameObject.FindWithTag("Source"));
        Destroy(GameObject.FindWithTag("State"));
        SceneManager.LoadScene("Home");
    }
    public void ClickRanking()
    {
        SceneManager.LoadScene("Ranking");
    }

    public void HowtoUp()
    {
        if (!isReadingHowto)
            return;

        // 上がない場合は何もしない
        if (howtoSpriteIndex >= howtoSprites.Length - 1)
            return;

        howtoSpriteIndex++;

        howtoImage.sprite = howtoSprites[howtoSpriteIndex];
    }

    public void HowtoDown()
    {
        if (!isReadingHowto)
            return;

        // 下がない場合は何もしない
        if (howtoSpriteIndex <= 0)
            return;

        howtoSpriteIndex--;

        howtoImage.sprite = howtoSprites[howtoSpriteIndex];
    }
    public void EnableHowto()
    {
        isReadingHowto = true;
        howtoSpriteIndex = 0;
        howtoImage.sprite = howtoSprites[howtoSpriteIndex];
        howtoImage.gameObject.SetActive(true);
    }
    public void DisableHowto()
    {
        isReadingHowto = false;
        howtoImage.gameObject.SetActive(false);
    }
}
