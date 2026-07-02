using UnityEngine;

public class menubar : MonoBehaviour
{
    public GameObject[] menubarBases; //indexはWindowManagerTestのAppIndex(dock左から)
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for(int i = 0; i < menubarBases.Length; i++)
        {
            if(!(i == 0)) menubarBases[i].SetActive(true);
            else menubarBases[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
