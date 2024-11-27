using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
//C 2024 Daniel Snapir alias Baltazar Benoni
public class IntroTextManager : MonoBehaviour
{
    private Text textBoxContent; 
    [SerializeField] private TextAsset introLoreAsset;
    [SerializeField] private TextAsset introLineTimers;
    private string IntroLoreText;
    private string[] introLines;
    private float[] introLineTimersTable;
    private string selectedLine;
    private char selectedChar;
    private int lineNum;
    private int charNum;
    private float timerStart;
    private float charWaitTimer;
    private float emptyTextTimerStart;
    private float timerLimitLineBreak;
    [SerializeField] private float timerLimitPerioid;
    [SerializeField] private float timerLimitLetter;
    [SerializeField] private float lineEmptyRatio;
    private float emptyTextBoxTimeLimit;
    [SerializeField] private AudioSource speechAudioPlayer;

    private bool lineWaitComplete;
    private bool charWaitComplete;
    private bool lineEmptied;
    private float timerToEmptyTextBox;
    [SerializeField] private GameObject gameLogoSprite;
    private bool imageNotVisible;
    void Awake()
    {
        gameLogoSprite.SetActive(false);

    }
    void Start()
    {
        GetStringsFromAsset();
        InitializeNumberVariables();
        InitializeBooleans();
        introLineTimersTable = GetLineBreakTable();
        timerStart = speechAudioPlayer.time;
    }
    void Update()
    {
        timerStart += Time.deltaTime;
        if(lineNum < introLines.Length)
        {
            ProcessText();
        }
        else if(imageNotVisible)
        {
            MakeGameLogoVisible();
        }
    }
    void InitializeNumberVariables()
    {
        lineNum = 0;
        charNum = 0;
        charWaitTimer = 0;
        selectedLine = introLines[lineNum];
    }
    void InitializeBooleans()
    {
        lineEmptied = true;
        lineWaitComplete = false;
        charWaitComplete = true;
        imageNotVisible = true;
    }
    void GetStringsFromAsset()
    {
        textBoxContent = GetComponent<Text>();
        IntroLoreText = introLoreAsset.ToString();
        introLines = IntroLoreText.Split("\n");
    }

    void ProcessText()
    {
        Debug.Log(lineNum);
        //Verify that this is not the last line of the text.
        if(lineNum + 1 >= introLines.Length)
        { lineNum++; return; }

        //If the last character of the line has been written, go to next.
        if(charNum >= selectedLine.Length)
        {
            AssignNewLineAndChar();
        }
        //Waiting to begin a new line. Check when to empty the text box.
        if(!lineWaitComplete)
        {
            lineWaitComplete = timerStart >= GetLineBreakTimer(lineNum);

            if(!lineEmptied)
            {
                ManageLineWait();
            }
        }
        //If waiting time has been completed, keep writing this line.
        if(lineWaitComplete && charNum < selectedLine.Length)
        {
            ProcessLine();
        }
    }
    //Check when to empty the text box.
    void ManageLineWait()
    {
        emptyTextBoxTimeLimit = GetEmptyLineTimer();
        if(timerToEmptyTextBox > emptyTextBoxTimeLimit || lineWaitComplete)
        {
            EmptyTextBox();
            charWaitComplete = true;
        }
        else
        {
            timerToEmptyTextBox += Time.deltaTime; 
        }
    }
    //Wait before writing the next character. If this has been done, write the next character.
    void ProcessLine()
    {
        selectedChar = selectedLine[charNum];
        if(!charWaitComplete)
        {
            int charNumBehind = charNum - 1 >= 0 ? charNum - 1 : 0;
            charWaitComplete = Time.timeSinceLevelLoad - charWaitTimer >= GetCharTimeLimit(charNumBehind);
        }
        if(charWaitComplete)
        {
            WriteOneLetter(selectedChar);
            charWaitTimer = Time.timeSinceLevelLoad;
            charWaitComplete = false;
            charNum++;
        }
    }
    void WriteOneLetter(char selectedChar)
    {
        textBoxContent.text += selectedChar.ToString();
    }
    void EmptyTextBox()
    {
        lineEmptied = true;
        textBoxContent.text = "";
        timerToEmptyTextBox = 0f;
    }
    //Upon moving on to the next line, change appropriate variables and fetch new timer values.
    void AssignNewLineAndChar()
    {
        lineNum++;
        selectedLine = introLines[lineNum];
        lineWaitComplete = false;
        lineEmptied = false;
        charWaitComplete = true;
        timerLimitLineBreak = GetLineBreakTimer(lineNum);
        emptyTextTimerStart = speechAudioPlayer.time;
        charNum = 0;
    }
    //Wait longer for certain types of characters.
    float GetCharTimeLimit(int a)
    {
        char charToCheck = selectedLine[a];
        if(charToCheck == '.' || charToCheck == ';' || charToCheck == '-')
        { return timerLimitPerioid; }
        else
        { return timerLimitLetter; }
    }
    //Get the time to begin a new line from the lookup table.
    float GetLineBreakTimer(int a)
    {
        float thisValue = introLineTimersTable[a];
        return thisValue;
    }
    //Look at how long we are waiting before the next line and time the emptying of the text box accordingly.
    float GetEmptyLineTimer()
    {
        float lineEmptyWait = (timerLimitLineBreak - emptyTextTimerStart) * lineEmptyRatio;
        return lineEmptyWait;
    }
    //Get the lookup table to be used for the beginning of each line.
    float[] GetLineBreakTable()
    {
        string lineBreakTableString = introLineTimers.ToString();
        string[] lineBreakTableSplit = lineBreakTableString.Split("\n");
        float[] lineBreakTable = new float[lineBreakTableSplit.Length];
        for(int i = 0; i < lineBreakTableSplit.Length; i++)
        {
            try
            {
                lineBreakTable[i] = float.Parse(lineBreakTableSplit[i], CultureInfo.InvariantCulture);
            }
            catch
            {
                Debug.Log("Format error in lookup table!!!! Line : " + i);
            }
        }
        return lineBreakTable;
    }
    void MakeGameLogoVisible()
    {
        EmptyTextBox();

        if(timerStart >= 214.7f)
        {
            gameLogoSprite.SetActive(true);
            imageNotVisible = false;
        }
    }
}
