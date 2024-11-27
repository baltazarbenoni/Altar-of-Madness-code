using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
//C 2024 Daniel Snapir alias Baltazar Benoni

public class IntroText : MonoBehaviour
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
    [SerializeField] private float timerLimitSpace;
    [SerializeField] private float timerLimitPerioid;
    [SerializeField] private float timerLimitLineBreak;
    [SerializeField] private float timerLimitLetter;
    private bool lineWaitComplete;
    private bool charWaitComplete;
    private bool lineEmptied;
    
    
       void Start()
    {
        textBoxContent = GetComponent<Text>();
        IntroLoreText = introLoreAsset.ToString();
        introLines = IntroLoreText.Split("\n");
        lineNum = 0;
        charNum = 0;
        selectedLine = introLines[lineNum];
        lineEmptied = false;
        lineWaitComplete = false;
        charWaitComplete = true;
        introLineTimersTable = GetLineBreakTable();
    }

    void Update()
    {
        ProcessText();
        
    }

    void ProcessText()
    {
        if(charNum > selectedLine.Length)
        {
            lineNum++;
            lineWaitComplete = false;
            charWaitComplete = true;
            charNum = 0;
        }
        if(!lineWaitComplete)
        {
            timerLimitLineBreak = GetLineBreakTimer();
            if(!lineEmptied) { EmptyTextBox(); }
            lineWaitComplete = Time.timeSinceLevelLoad >= timerLimitLineBreak;
        }

        if(lineWaitComplete)
        {
            ProcessLine();
        }
    }

    void ProcessLine()
    {
        AssignNewLineAndChar();

        if(lineNum >= introLines.Length) { return; }

        if(!charWaitComplete)
        {
            charWaitComplete = Time.timeSinceLevelLoad - timerStart >= GetTimeLimit(lineNum);
        }

        if(charWaitComplete)
        {
            WriteOneLetter(selectedChar);
            charWaitComplete = false;
            charNum++;
            timerStart = Time.timeSinceLevelLoad;
        }
    }

    void WriteOneLetter(char selectedChar)
    {
        textBoxContent.text += selectedChar.ToString();
    }
    void EmptyTextBox()
    {
        if(Time.timeSinceLevelLoad >= timerLimitLineBreak / 2)
        { lineEmptied = true; textBoxContent.text = "";}
    }

    void AssignNewLineAndChar()
    {
        selectedLine = introLines[lineNum];
        selectedChar = selectedLine[charNum];
    }
    float GetTimeLimit(int a)
    {
        selectedChar = selectedLine[a];
        if(selectedChar == ' ')
        { return timerLimitSpace; }
        else if(selectedChar == '.')
        { return timerLimitPerioid; }
        else
        { return timerLimitLetter; }
    }

    float GetLineBreakTimer()
    {
        float thisValue = introLineTimersTable[lineNum];
        return thisValue;
    }
    float[] GetLineBreakTable()
    {
        string lineBreakTableString = introLineTimers.ToString();
        string[] lineBreakTableSplit = lineBreakTableString.Split("\n");
        float[] lineBreakTable = new float[lineBreakTableSplit.Length];

        for(int i = 0; i < lineBreakTableSplit.Length; i++)
        {
            try
            {
                lineBreakTable[i] = float.Parse(lineBreakTableSplit[i]);
            }
            catch
            {
                Debug.Log("Format error in lookup table!!!! Line : " + i);
            }
        }
        return lineBreakTable;
    }
}
