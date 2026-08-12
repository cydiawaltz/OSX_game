using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.UI;


public class PlayerExtra : MonoBehaviour
{
    [SerializeField] float rotateTime;

    Tween rotateTween;
    [SerializeField] GameObject XtargetObject;
    [SerializeField] GameObject IyakoObject;
    [SerializeField] Image rawImage;
    [SerializeField] float rotateSpeed;
    [SerializeField] float disableAfterSeconds;

    bool active = false;
    bool rotating = false;
    bool firstKPressed = false;
    //debug
    public bool XActivate;
    public bool IyakoActivate;
    void Start()
    {
        XtargetObject.SetActive(false);
        IyakoObject.SetActive(false);
        rawImage.enabled = false;
    }

    void Update()
    {
        //debug
        if(XActivate)
        {
            ActivateBigX();
            XActivate = false;
        }
        if(IyakoActivate)
        {
            ActiveIyako();
            IyakoActivate = false;
        }

        //
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(-v, 0, h);

        if (dir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);

            rotateTween?.Kill();
            rotateTween = transform
                .DORotateQuaternion(targetRot, rotateTime)
                .SetEase(Ease.OutQuad);
        }
        if (!active) return;

        if (Input.GetKeyDown(KeyCode.K))
        {
            if (!firstKPressed)
            {
                firstKPressed = true;
                rawImage.enabled = false;
            }

            rotating = !rotating;
        }

        if (rotating)
        {
            XtargetObject.transform.Rotate(0,0, rotateSpeed * Time.deltaTime);
        }
        
    }
    public void ActivateBigX()
    {
        active = true;
        rotating = false;
        firstKPressed = false;

        XtargetObject.SetActive(true);
        rawImage.enabled = true;

        StopAllCoroutines();
        StartCoroutine(DisableRoutine());
    }
    public void ActiveIyako()
    {
        IyakoObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(DisableIyakoRoutine());
    }
    IEnumerator DisableRoutine()
    {
        yield return new WaitForSeconds(disableAfterSeconds);

        active = false;
        rotating = false;
        rawImage.enabled = false;
        XtargetObject.SetActive(false);
    }
    IEnumerator DisableIyakoRoutine()
    {
        yield return new WaitForSeconds(disableAfterSeconds);

        IyakoObject.SetActive(false);
    }
}
