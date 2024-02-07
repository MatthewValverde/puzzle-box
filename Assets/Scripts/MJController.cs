using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class MorajaiController : MonoBehaviour
{
    // When adding a new Public Variable, you must also add it to the MJColorCodeHelper class in the Editor folder.
    // Otherwise it will not be visible in the Inspector
    [Space]
    public string colorCode = "0000000000000";
    public bool useVisualColorTool = false;
    [Space]
    public MjColors[] colorOrder = new MjColors[13];
    public MjColorDefinitions colorDefinitions;
    public MjCornerDefinitions colorToCornerAssociations;
    public MjSoundFX audioClips;
    public GameObject buttonsContainer;
    public GameObject buttonTestPanel;
    public bool showDebug = false;
    public bool useMouseEvents = false;
    public float buttonPressDepth = 0.72f;
    public float cornerPressDepth = 0.15f;
    [HideInInspector]
    public bool hasSolved = false;
    [Space]
    private AudioSource audioSource;
    GameObject[,] buttons;
    GameObject[] buttonsIndexer;
    GameObject[] cornersIndexer;
    Dictionary<MjColors, Color> colorDictionary;
    Animator animator;
    MjButtonColor[] buttonColorDefinitions = new MjButtonColor[9];
    MjCornerSymbols[] cornerSymbolsAssociations = new MjCornerSymbols[8];
    string[] cornerColorCode = new string[4];
    string[] buttonColorCode = new string[9];
    MjGridPosition[] cornerButtonGridPositions = {
                    new MjGridPosition(2, 2),
                    new MjGridPosition(0, 2),
                    new MjGridPosition(0, 0),
                    new MjGridPosition(2, 0)
                };

    void Start()
    {
        buttonColorDefinitions = new MjButtonColor[]
        {
            new MjButtonColor(MjColors.Black, colorDefinitions.black),
            new MjButtonColor(MjColors.Red, colorDefinitions.red),
            new MjButtonColor(MjColors.Green, colorDefinitions.green),
            new MjButtonColor(MjColors.Blue, colorDefinitions.blue),
            new MjButtonColor(MjColors.Yellow, colorDefinitions.yellow),
            new MjButtonColor(MjColors.Orange, colorDefinitions.orange),
            new MjButtonColor(MjColors.Purple, colorDefinitions.purple),
            new MjButtonColor(MjColors.Pink, colorDefinitions.pink),
            new MjButtonColor(MjColors.White, colorDefinitions.white),
            new MjButtonColor(MjColors.Gray, colorDefinitions.gray)
        };

        cornerSymbolsAssociations = new MjCornerSymbols[]
        {
            new MjCornerSymbols(MjColors.Black, colorToCornerAssociations.black),
            new MjCornerSymbols(MjColors.Red, colorToCornerAssociations.red),
            new MjCornerSymbols(MjColors.Green, colorToCornerAssociations.green),
            new MjCornerSymbols(MjColors.Yellow, colorToCornerAssociations.yellow),
            new MjCornerSymbols(MjColors.Orange, colorToCornerAssociations.orange),
            new MjCornerSymbols(MjColors.Purple, colorToCornerAssociations.purple),
            new MjCornerSymbols(MjColors.Pink, colorToCornerAssociations.pink),
            new MjCornerSymbols(MjColors.White, colorToCornerAssociations.white)
        };

        for (int i = 0; i < cornerSymbolsAssociations.Length; i++)
        {
            for (int j = 0; j < cornerSymbolsAssociations[i].gameObject.transform.childCount; j++)
                cornerSymbolsAssociations[i].gameObject.transform.GetChild(j).gameObject.SetActive(false);
            cornerSymbolsAssociations[i].gameObject.SetActive(false);
        }

        SeperateColorCode();

        if (showDebug)
        {
            if (buttonTestPanel) buttonTestPanel.SetActive(true);
        }
        else
        {
            if (buttonTestPanel) buttonTestPanel.SetActive(false);
        }

        animator = GetComponent<Animator>();
        audioSource = gameObject.AddComponent<AudioSource>();
        InitializeGrid();
        SetUpCorners();
    }

    public void TriggerButtonClick(int buttonIndex)
    {
        buttonsIndexer[buttonIndex].GetComponent<MJBoxButtonHandler>().TriggerClick();
    }

    public void TriggerCornerClick(int cornerIndex)
    {
        StartCoroutine(PlaySoundFx(audioClips.cornerClick));
        MJCornerButtonHandler cornerHandler = cornersIndexer[cornerIndex].GetComponent<MJCornerButtonHandler>();
        cornerHandler.TriggerClick();
        GameObject btn = buttons[cornerHandler.gridPosition.row, cornerHandler.gridPosition.col];
        if (btn.GetComponent<MJBoxButtonHandler>().colorName == cornerHandler.cornerColorToBe)
        {
            cornerHandler.SetColorTint(new MjButtonColor(cornerHandler.cornerColorToBe, colorDictionary[cornerHandler.cornerColorToBe]));
        }
        else
        {
            foreach (GameObject corner in cornersIndexer)
            {
                corner.GetComponent<MJCornerButtonHandler>().SetColorTint(new MjButtonColor(MjColors.Gray, colorDictionary[MjColors.Gray]));
            }
            ResetGrid();
        }
        CheckCorners(true);
    }

    public void TriggerReset()
    {
        ResetGrid();
    }

    public void TriggerColorUpdate(string code)
    {
        for (int i = 0; i < cornerSymbolsAssociations.Length; i++)
        {
            for (int j = 0; j < cornerSymbolsAssociations[i].gameObject.transform.childCount; j++)
            {
                cornerSymbolsAssociations[i].gameObject.transform.GetChild(j).gameObject.SetActive(false);
            }
            cornerSymbolsAssociations[i].gameObject.SetActive(false);
        }

        colorCode = code;
        SeperateColorCode();

        SetUpCorners();
        ResetGrid(true);
    }

    void SeperateColorCode()
    {
        cornerColorCode = new string[4];
        buttonColorCode = new string[9];
        colorCode = colorCode.Length > 13 ? colorCode.Substring(0, 13) : colorCode.PadRight(13, '0');

        char[] colorCodeArray = StringToArray(colorCode);
        int tCounter = 0;
        for (int i = 0; i < 13; i++)
        {
            if (i < 4)
            {
                cornerColorCode[i] = colorCodeArray[i].ToString();
            }
            else
            {
                buttonColorCode[tCounter] = colorCodeArray[i].ToString();
                tCounter++;
            }
        }
    }

    void SetUpCorners()
    {
        cornersIndexer = new GameObject[4];
        for (int i = 0; i < cornerColorCode.Length; i++)
        {
            MjColors enumLabel = GetEnumFromCode(cornerColorCode[i]);
            if (enumLabel == MjColors.Blue || enumLabel == MjColors.Gray) enumLabel = MjColors.Red;
            foreach (MjCornerSymbols cornerSymbol in cornerSymbolsAssociations)
            {
                if (cornerSymbol.name == enumLabel)
                {
                    cornerSymbol.gameObject.SetActive(true);
                    GameObject corner = cornerSymbol.gameObject.transform.GetChild(i).gameObject;
                    MJCornerButtonHandler cornerHandler = corner.GetComponent<MJCornerButtonHandler>();
                    cornerHandler.index = i;
                    cornerHandler.gridPosition = cornerButtonGridPositions[i];
                    cornerHandler.cornerColorToBe = enumLabel;
                    cornerHandler.useMouseEvents = useMouseEvents;
                    cornerHandler.SetColorTint(new MjButtonColor(MjColors.Gray, colorDictionary[MjColors.Gray]));
                    cornerHandler.tweenDepth = cornerPressDepth;
                    cornerHandler.SubscribeToOnCornerDown(TriggerCornerClick);
                    corner.SetActive(true);
                    cornersIndexer[i] = corner;
                }
            }
        }
    }

    void InitializeGrid()
    {
        buttons = new GameObject[3, 3];
        buttonsIndexer = new GameObject[9];
        colorDictionary = new Dictionary<MjColors, Color>();

        for (int i = 0; i < buttonColorDefinitions.Length; i++)
        {
            colorDictionary.Add(buttonColorDefinitions[i].name, buttonColorDefinitions[i].color);
        }

        for (int j = 0; j < buttonsContainer.transform.childCount; j++)
        {
            GameObject button = buttonsContainer.transform.GetChild(j).gameObject;
            if (button.GetComponent<MJBoxButtonHandler>() == null) continue;
            int row = j / 3;
            int column = j % 3;
            buttons[row, column] = button;
            buttonsIndexer[j] = button;
            MJBoxButtonHandler boxHandler = button.GetComponent<MJBoxButtonHandler>();
            MjColors enumLabel = GetEnumFromCode(buttonColorCode[j]);
            if (colorDictionary.TryGetValue(enumLabel, out Color color))
            {
                boxHandler.SetColorTint(new MjButtonColor(enumLabel, color));
            }
            boxHandler.index = j;
            boxHandler.useMouseEvents = useMouseEvents;
            boxHandler.gridPosition.row = row;
            boxHandler.gridPosition.col = column;
            boxHandler.tweenDepth = buttonPressDepth;

            boxHandler.SubscribeToOnButtonDown(OnButtonPress);
            boxHandler.SubscribeToOnButtonHold(OnButtonHold);
        }
    }

    void ResetGrid(bool isColorChange = false)
    {
        hasSolved = false;
        if (!isColorChange) StartCoroutine(PlaySoundFx(audioClips.reset));
        for (int j = 0; j < buttonsContainer.transform.childCount; j++)
        {
            GameObject button = buttonsContainer.transform.GetChild(j).gameObject;
            if (button.GetComponent<MJBoxButtonHandler>() == null) continue;
            MJBoxButtonHandler boxHandler = button.GetComponent<MJBoxButtonHandler>();
            MjColors enumLabel = GetEnumFromCode(buttonColorCode[j]);
            if (colorDictionary.TryGetValue(enumLabel, out Color color))
            {
                boxHandler.SetColorTint(new MjButtonColor(enumLabel, color));
            }
        }

        foreach (GameObject corner in cornersIndexer)
        {
            corner.GetComponent<MJCornerButtonHandler>().SetColorTint(new MjButtonColor(MjColors.Gray, colorDictionary[MjColors.Gray]));
        }
    }

    void OnButtonHold(MjGridPosition position)
    {
        //print("RESET");
        hasSolved = false;
        ResetGrid();
    }

    void OnButtonPress(MjGridPosition position)
    {
        StartCoroutine(PlaySoundFx(audioClips.click));

        MjColors currentColor = buttons[position.row, position.col].GetComponent<MJBoxButtonHandler>().colorName;

        switch (currentColor)
        {
            case MjColors.Black:
                SetMaterialColor(position, MjColors.Green);
                break;
            case MjColors.Green:
                if (IsEntireRowOrColumnColor(position, MjColors.Green))
                    SetMaterialColor(position, MjColors.Blue);
                else
                    SetMaterialColor(position, MjColors.Red);
                break;
            case MjColors.Red:
                SetMaterialColor(position, MjColors.Black);
                break;
            case MjColors.Blue:
                SetMaterialColor(position, MjColors.Black);
                ChangeAdjacentColorsForBlue(position);
                break;
            case MjColors.Purple:
                if (IsEntireRowOrColumnColor(position, MjColors.Purple))
                    SetMaterialColor(position, MjColors.White);
                else
                    SetMaterialColor(position, MjColors.Black);
                break;
            case MjColors.White:
                if (IsOnEdge(position))
                    SwapWithOppositeTile(position);
                break;
            case MjColors.Pink:
                RotateSurroundingColorsClockwise(position);
                break;
            case MjColors.Yellow:
                MjColors centerColor = buttons[1, 1].GetComponent<MJBoxButtonHandler>().colorName;
                SetAdjacentColors(position, centerColor);
                break;
            case MjColors.Orange:
                SwapWithAboveTile(position);
                break;
            case MjColors.Gray:
                // Gray does nothing
                break;
            default:
                break;
        }

        CheckCorners();
    }

    void SetMaterialColor(MjGridPosition position, MjColors colorName)
    {
        if (colorDictionary.TryGetValue(colorName, out Color color))
        {
            buttons[position.row, position.col].GetComponent<MJBoxButtonHandler>().SetColorTint(new MjButtonColor(colorName, color));
        }
    }

    void CheckCorners(bool countToWin = false)
    {
        int cornerCountToWin = 0;
        foreach (GameObject corner in cornersIndexer)
        {
            MJCornerButtonHandler cornerHandler = corner.GetComponent<MJCornerButtonHandler>();
            GameObject btn = buttons[cornerHandler.gridPosition.row, cornerHandler.gridPosition.col];
            if (btn.GetComponent<MJBoxButtonHandler>().colorName != cornerHandler.colorName)
            {
                cornerHandler.SetColorTint(new MjButtonColor(MjColors.Gray, colorDictionary[MjColors.Gray]));
            }
            else
            {
                cornerCountToWin++;
            }
        }

        if (!countToWin) return;
        if (cornerCountToWin == cornersIndexer.Length)
        {
            hasSolved = true;
            MoraJaiBoxSolvedEventHandler();
            if (animator) animator.SetTrigger("Open");
            StartCoroutine(PlaySoundFx(audioClips.open, 0.5f));
        }
    }

    bool IsEntireRowOrColumnColor(MjGridPosition position, MjColors colorName)
    {
        bool entireRow = true;
        bool entireColumn = true;

        for (int i = 0; i < 3; i++)
        {
            if (buttons[position.row, i].GetComponent<MJBoxButtonHandler>().colorName != colorName)
                entireRow = false;
            if (buttons[i, position.col].GetComponent<MJBoxButtonHandler>().colorName != colorName)
                entireColumn = false;
        }

        return entireRow || entireColumn;
    }

    void ChangeAdjacentColorsForBlue(MjGridPosition position)
    {
        // Define offsets for adjacent positions (up, down, left, right)
        // int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };

        for (int k = 0; k < 4; k++)
        {
            // int adjRow = position.row + dx[k];
            int adjCol = position.col + dy[k];

            if (/*adjRow >= 0 && adjRow < 3 &&*/ adjCol >= 0 && adjCol < 3)
            {
                MjColors adjacentColor = buttons[position.row/*adjRow*/, adjCol].GetComponent<MJBoxButtonHandler>().colorName;
                if (adjacentColor == MjColors.Red)
                    SetMaterialColor(new MjGridPosition(position.row/*adjRow*/, adjCol), MjColors.Purple);
                else if (adjacentColor != MjColors.White)
                    SetMaterialColor(new MjGridPosition(position.row/*adjRow*/, adjCol), MjColors.Blue);
            }
        }
    }

    bool IsOnEdge(MjGridPosition position)
    {
        return position.row == 0 || position.row == 2 || position.col == 0 || position.col == 2;
    }

    void SwapWithOppositeTile(MjGridPosition position)
    {
        int oppX = 2 - position.row;
        int oppY = 2 - position.col;
        MjColors tempColorFrom = buttons[position.row, position.col].GetComponent<MJBoxButtonHandler>().colorName;
        MjColors tempColorTo = buttons[oppX, oppY].GetComponent<MJBoxButtonHandler>().colorName;
        SetMaterialColor(new MjGridPosition(oppX, oppY), tempColorFrom);
        SetMaterialColor(position, tempColorTo);
    }

    void SetAdjacentColors(MjGridPosition position, MjColors colorName)
    {
        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };

        for (int k = 0; k < 4; k++)
        {
            int adjRow = position.row + dx[k];
            int adjCol = position.col + dy[k];

            if (adjRow >= 0 && adjRow < 3 && adjCol >= 0 && adjCol < 3)
            {
                SetMaterialColor(new MjGridPosition(adjRow, adjCol), colorName);
            }
        }
    }

    void SwapWithAboveTile(MjGridPosition position)
    {
        if (position.row > 0)
        {
            MjColors aboveColor = buttons[position.row - 1, position.col].GetComponent<MJBoxButtonHandler>().colorName;
            MjColors pressedColor = buttons[position.row, position.col].GetComponent<MJBoxButtonHandler>().colorName;
            SetMaterialColor(new MjGridPosition(position.row - 1, position.col), pressedColor);
            SetMaterialColor(new MjGridPosition(position.row, position.col), aboveColor);
        }
    }

    void RotateSurroundingColorsClockwise(MjGridPosition position)
    {
        // Offsets for all eight surrounding positions: TL, T, TR, R, BR, B, BL, L
        int[] dx = { -1, -1, -1, 0, 1, 1, 1, 0 };
        int[] dy = { -1, 0, 1, 1, 1, 0, -1, -1 };

        MjColors[] surroundingColors = new MjColors[8];
        int validColorsCount = 0;

        // Storing the colors of surrounding buttons
        for (int k = 0; k < 8; k++)
        {
            int adjRow = position.row + dx[k];
            int adjCol = position.col + dy[k];

            if (adjRow >= 0 && adjRow < 3 && adjCol >= 0 && adjCol < 3)
            {
                surroundingColors[validColorsCount++] = buttons[adjRow, adjCol].GetComponent<MJBoxButtonHandler>().colorName;
            }
        }

        // Rotating colors clockwise
        if (validColorsCount > 0)
        {
            MjColors lastColor = surroundingColors[validColorsCount - 1];
            for (int k = validColorsCount - 1; k > 0; k--)
            {
                surroundingColors[k] = surroundingColors[k - 1];
            }
            surroundingColors[0] = lastColor;

            // Apply the rotated colors
            int colorIndex = 0;
            for (int k = 0; k < 8; k++)
            {
                int adjRow = position.row + dx[k];
                int adjCol = position.col + dy[k];

                if (adjRow >= 0 && adjRow < 3 && adjCol >= 0 && adjCol < 3)
                {
                    SetMaterialColor(new MjGridPosition(adjRow, adjCol), surroundingColors[colorIndex++]);
                }
            }
        }
    }

    MjColors GetEnumFromCode(string code)
    {
        switch (code)
        {
            case "0": return MjColors.Black;
            case "1": return MjColors.Red;
            case "2": return MjColors.Green;
            case "3": return MjColors.Blue;
            case "4": return MjColors.Yellow;
            case "5": return MjColors.Orange;
            case "6": return MjColors.Purple;
            case "7": return MjColors.Pink;
            case "8": return MjColors.White;
            case "9": return MjColors.Gray;
            default: return MjColors.Gray;
        }
    }

    IEnumerator PlaySoundFx(AudioClip clip, float secondsToWait = 0)
    {
        yield return new WaitForSeconds(secondsToWait);

        if (clip && audioSource)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    char[] StringToArray(string str)
    {
        return str.ToCharArray();
    }

    public delegate void MoraJaiBoxSolvedEvent();
    public event MoraJaiBoxSolvedEvent MoraJaiSolved;
    public void MoraJaiBoxSolvedEventHandler()
    {
        MoraJaiSolved?.Invoke();
    }

    public void UCC()
    {
        colorCode = "";
        foreach (var label in colorOrder)
        {
            colorCode += (int)label;
        }
        SeperateColorCode();
    }

    public void UCO()
    {
        if (colorCode.Length != colorOrder.Length)
        {
            if (colorCode.Length > 12) colorCode = colorCode.Substring(0, 13);
        }
        for (int i = 0; i < colorCode.Length; i++)
        {
            if (int.TryParse(colorCode[i].ToString(), out int colorIndex) && Enum.IsDefined(typeof(MjColors), colorIndex))
            {
                colorOrder[i] = (MjColors)colorIndex;
            }
            else
            {
                Debug.LogError($"Invalid label code at position {i}: {colorCode[i]}");
                return;
            }
        }
    }

    public void TestColorCodeTriggerUpdate(InputField inField)
    {
        TriggerColorUpdate(inField.text);
    }
}

[Serializable]
public class MjButtonColor
{
    public MjColors name;
    public Color color;

    public MjButtonColor()
    {

    }
    public MjButtonColor(MjColors name, Color color)
    {
        this.name = name;
        this.color = color;
    }
}

[Serializable]
public class MjColorDefinitions
{
    public Color black = Color.black;
    public Color red = new Color(0.7f, 0, 0);
    public Color green = new Color(0, 0.7f, 0);
    public Color blue = new Color(0, 0.498f, 1f);
    public Color yellow = Color.yellow;
    public Color orange = new Color(1f, 0.647f, 0);
    public Color purple = new Color(0.6f, 0, 0.6f);
    public Color pink = new Color(1f, 0.753f, 0.796f);
    public Color white = Color.white;
    public Color gray = Color.gray;
}

[Serializable]
public class MjCornerDefinitions
{
    public GameObject black;
    public GameObject red;
    public GameObject green;
    public GameObject yellow;
    public GameObject orange;
    public GameObject purple;
    public GameObject pink;
    public GameObject white;
}

[Serializable]
public class MjSoundFX
{
    public AudioClip click;
    public AudioClip open;
    public AudioClip reset;
    public AudioClip cornerClick;
}

[Serializable]
public class MjCornerSymbols
{
    public MjColors name;
    public GameObject gameObject;
    public MjCornerSymbols(MjColors name, GameObject gameObject)
    {
        this.name = name;
        this.gameObject = gameObject;
    }
}

[Serializable]
public class MjGridPosition
{
    public int row;
    public int col;
    public MjGridPosition(int row, int col)
    {
        this.row = row;
        this.col = col;
    }
}

public enum MjColors
{
    Black,
    Red,
    Green,
    Blue,
    Yellow,
    Orange,
    Purple,
    Pink,
    White,
    Gray
}
