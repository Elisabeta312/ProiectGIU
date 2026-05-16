using UnityEngine;

public class JournalPagesUI : MonoBehaviour
{
    public GameObject animalsPage1;
    public GameObject animalsPage2;

    public GameObject buttonNext;
    public GameObject buttonPrev;

    void Start()
    {
        ShowPage1();
    }

    public void ShowPage1()
    {
        animalsPage1.SetActive(true);
        animalsPage2.SetActive(false);

        buttonNext.SetActive(true);
        buttonPrev.SetActive(false);
    }

    public void ShowPage2()
    {
        animalsPage1.SetActive(false);
        animalsPage2.SetActive(true);

        buttonNext.SetActive(false);
        buttonPrev.SetActive(true);
    }
}