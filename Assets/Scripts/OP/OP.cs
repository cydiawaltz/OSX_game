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
        StartCoroutine(StartSet());

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
        source.DOFade(0.3f, 10f);
    }
    public void ClickPuma()
    {
        SceneManager.LoadScene("10.1");
    }
    public void ClickPanther()
    {
        SceneManager.LoadScene("10.3Pre");
    }
    public void ClickTiger()
    {
        SceneManager.LoadScene("10.4Pre");
    }
    public void ClickLeopard()
    {
        SceneManager.LoadScene("10.5Pre");
    }
    public void ClickQuit()
    {
        Application.Quit();
    }
    public void ClickReboot()
    {
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
