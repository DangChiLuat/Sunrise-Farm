using TMPro;
using UnityEngine;
public class DiaLogManager : MonoBehaviour
{

    public TextMeshProUGUI diaLogText;
    public TextMeshProUGUI nameText;
    public GameObject diaLogBox;
    public GameObject nameBox;

    public string[] dialogLines;

    public int currentLine;

    private bool justStarted;

    public static DiaLogManager instance;



    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        //diaLogText.text = dialogLines[currentText];

    }

    // Update is called once per frame
    void Update()
    {

        if (diaLogBox.activeInHierarchy)
        {
            if (Input.GetButtonUp("Fire1"))
            {
                if (!justStarted)
                {
                    currentLine++;
                    if (currentLine >= dialogLines.Length)
                    {
                        diaLogBox.SetActive(false);
                        GameManager.instance.dialogActive = false;
                    }
                    else
                    {
                        checkIfName();
                        diaLogText.text = dialogLines[currentLine];
                    }
                }
                else
                {
                    justStarted = false;
                }
            }
        }

         
    }

    public void ShowDialog(string[] newLines, bool isPerson)
    {
        dialogLines = newLines;
        currentLine = 0;

        checkIfName();
        diaLogText.text = dialogLines[currentLine];
        diaLogBox.SetActive(true);
        justStarted = true;
        nameBox.SetActive(isPerson);
        GameManager.instance.dialogActive = true;   
    }
    public void checkIfName()
    {
        if (dialogLines[currentLine].StartsWith("n-"))
        {
            nameText.text = dialogLines[currentLine].Replace("n-","");
            currentLine++;
        }
    }
}
