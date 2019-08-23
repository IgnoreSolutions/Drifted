using UnityEngine;
using System.Collections;
using Drifted;
using UnityEngine.UI;
using Drifted.Input;

public class WMTabHandler : MonoBehaviour
{
    public Color SelectedTabColor = new Color(.37f,.37f,.37f,1f);
    public Color TabColor = Color.gray;

    public GameObject[] Tabs;
    public GameObject[] Tab_Content;

    public int CurrentTab = 0;
    public bool Allow = false;
    // Use this for initialization
    void Start()
    {
        Allow = (Tabs.Length == Tab_Content.Length);
        if (!Allow) Debug.LogWarning("Tab Content and tab mismatch");

        for(int i = 0; i < Tab_Content.Length; i++)
        {
            if(i == CurrentTab)
            {
                Tabs[i].GetComponent<Image>().color = SelectedTabColor;
                Tab_Content[i].SetActive(true);
            }
            else
            {
                Tabs[i].GetComponent<Image>().color = TabColor;
                Tab_Content[i].SetActive(false);
            }
        } 
    }

    // Update is called once per frame
    void Update()
    {
        if(!Allow) return;

        int oldIndex = 0;
        if (Allow)
        {
            if (DriftedInputManager.KeyDown("PreviousTab"))
            {
                oldIndex = CurrentTab;
                CurrentTab--;
                if (CurrentTab < 0) CurrentTab = 0;
            }
            else if (DriftedInputManager.KeyDown("NextTab"))
            {
                oldIndex = CurrentTab;
                CurrentTab++;
                if (CurrentTab > (Tabs.Length - 1)) CurrentTab = (Tabs.Length - 1);
            }

            if (oldIndex != CurrentTab)
            {
                Tabs[oldIndex].GetComponent<Image>().color = TabColor;
                Tabs[CurrentTab].GetComponent<Image>().color = SelectedTabColor;

                Tab_Content[oldIndex].SetActive(false);
                Tab_Content[CurrentTab].SetActive(true);
            }
        }
    }
}
