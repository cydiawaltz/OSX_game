using UnityEngine;

public class threeDUIs : MonoBehaviour//動かすものの親（つまりウインドウ側に）置く
{
    public GameObject Button;
    bool isUsingButton;//ボタン使うか


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(!(Button == null))
        {
            isUsingButton = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(isUsingButton)
            {
                
            }
        }
    }
}
