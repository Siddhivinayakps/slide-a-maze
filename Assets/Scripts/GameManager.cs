using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Game State
    enum GameState { Solved, Shuffling, InPlay }
    GameState State;

    // Game Configs
    private int ShuffleCount = 1;
    private int BlocksPerLine = 1;
    private int SecondsRemaining = 60;

    // Image Sprite To Divide
    public Sprite ImageSprite;

    // Empty Block
    private Block EmptyBlock;
    private int EmptyRow, EmptyColumn;

    // Block Matrix
    Block[,] Blocks;
    Sprite[,] ImageSlices;

    // Score Remaining Text Object
    public Text TimeRemaining;

    //Previous Random 
    private int PreviousRandom = -4;

    //Game Over Panel
    public GameObject GameOverPanel;
    public Text TryAgain;
    public Text LevelWon;
    
    //Game Over Text
    public Text GameOverText;

    //LevelText
    public Text LevelText;

    public int CurrentLevel;

    //Image Assets for each Level
    Dictionary<int, string> LevelImageName = new Dictionary<int, string>(){
        {1,"Images/Level-1"},
        {2,"Images/Level-2"},
        {3,"Images/Level-3"},
        {4,"Images/Level-4"},
        {5,"Images/Level-5"},
        {6,"Images/Level-6"},
    };


    // Start is called before the first frame update
    void Start()
    {
        LoadImageForLevel();
        CreatePuzzle();
        StartCoroutine(Shuffle());
    }

    // Update is called once per frame
    void Update()
    {

    }


    //Puzzle creation
    private void CreatePuzzle()
    {
        //Disable Game Over Panel
        GameOverPanel.SetActive(false);

        Image[] imageComponentBlock = new Image[BlocksPerLine * BlocksPerLine];
        
        Blocks = new Block[BlocksPerLine, BlocksPerLine];
        ImageSlices = ImageSlicer.GetSpriteSlices(ImageSprite, BlocksPerLine);

        //Auto Grid Layout For Column Arranging
        AutoGridLayout autoGridLayout = transform.Find("GameFieldParent").Find("GameField").GetComponent<AutoGridLayout>();
        autoGridLayout._column = BlocksPerLine;

        //Add Outline
        //transform.Find("GameFieldParent").Find("GameField").GetComponent<Outline>().enabled = true;

        //Creating Image GameObject Component
        for (int i = 0; i< BlocksPerLine * BlocksPerLine; i++)
        {
            GameObject newObj = new GameObject("Block");
            Image newImage = newObj.AddComponent<Image>();
            imageComponentBlock[i] = newImage;
            newObj.GetComponent<RectTransform>().SetParent(transform.Find("GameFieldParent").Find("GameField"));
        }

        //Creating grid with block component.
        for (int row = 0; row < BlocksPerLine; row++)
        {
            for (int column = 0; column < BlocksPerLine; column++)
            {
                foreach (Image singleImage in imageComponentBlock)
                {
                    if (singleImage.gameObject.GetComponent<Block>() == null)
                    {
                        //Adding Block Script To Game Object
                        Block block = singleImage.gameObject.AddComponent<Block>();
                        block.SetImage(ImageSlices[row, column]);
                        block.OnBlockPressed += MoveBlockByPlayer;
                        block.Number = 1;

                        block.Row = row;
                        block.Column = column;

                        if (row == BlocksPerLine - 1 && column == BlocksPerLine - 1)
                        {
                            block.Number = 0;
                            EmptyBlock = block;
                            EmptyRow = row;
                            EmptyColumn = column;
                        }
                        Blocks[row, column] = block;
                        break;
                    }
                }
            }
        }
    }

    //Move of player
    void MoveBlockByPlayer(Block blockToMove)
    {
        if (State == GameState.InPlay)
        {
            MoveBlock(blockToMove);
            CheckSolved();
        }
    }

    //Logic to swap blocks
    bool MoveBlock(Block blockToMove)
    {
        if ((Mathf.Abs(EmptyBlock.Row - blockToMove.Row) == 1 && Mathf.Abs(EmptyBlock.Column - blockToMove.Column) == 0) || (Mathf.Abs(EmptyBlock.Column - blockToMove.Column) == 1 && Mathf.Abs(EmptyBlock.Row - blockToMove.Row) == 0))
        {
            Block newBlock = EmptyBlock;
            EmptyBlock = blockToMove;
            blockToMove = newBlock;
            blockToMove.SetImage(EmptyBlock.BlockImage.sprite);
            blockToMove.Number = 1;
            EmptyBlock.Number = 0;
            Blocks[EmptyBlock.Row, EmptyBlock.Column] = EmptyBlock;
            Blocks[blockToMove.Row, blockToMove.Column] = blockToMove;
            return true;
        }
        return false;
    }


    //Start Shuffle
    IEnumerator Shuffle()
    {
        State = GameState.Shuffling;
        while (ShuffleCount > 0)
        {
            yield return new WaitForSeconds(0.1f);
            ShuffleMove();
        }

    }


    //Shuffle Logic
    void ShuffleMove()
    {
        /// <summary>
        ///  1 - Top
        /// -1 - Down
        ///  2 - Right
        /// -2 - Left
        /// </summary>

        int[] moveDirection = { 1, -1, 2, -2 };
        Block blockToMove;
        
        int randomIndex = Random.Range(0, moveDirection.Length);
        for (int i = 0; i < moveDirection.Length; i++)
        {
            int EmptyBlockRow = EmptyBlock.Row;
            int EmptyBlockColumn = EmptyBlock.Column;
            int randomValue = moveDirection[ (randomIndex + i) % moveDirection.Length];
            if (randomValue != (PreviousRandom * -1))
            {
                switch (randomValue.ToString())
                {
                    case "1":
                        EmptyBlockRow -= 1;
                        break;

                    case "-1":
                        EmptyBlockRow += 1;

                        break;

                    case "2":
                        EmptyBlockColumn += 1;
                        break;

                    case "-2":
                        EmptyBlockColumn -= 1;
                        break;

                    default:
                        break;

                }

                if (EmptyBlockColumn >= 0 && EmptyBlockColumn < BlocksPerLine && EmptyBlockRow >= 0 && EmptyBlockRow < BlocksPerLine)
                {
                    blockToMove = Blocks[EmptyBlockRow, EmptyBlockColumn];
                    MoveBlock(blockToMove);
                    PreviousRandom = randomValue;
                    ShuffleCount--;
                    if (ShuffleCount == 0)
                    {
                        State = GameState.InPlay;
                        StartCoroutine(StartCountdown());
                    }
                    break;
                }
            }
        }
    }

    //To check solved
    private bool CheckSolved()
    {
        bool solved = true;
        foreach(Block block in Blocks)
        {
            if(ImageSlices[block.Row,block.Column] != block.BlockImage.sprite && block.number != 0)
            {
                solved = false;
                break;
            }
        }

        if (solved)
        {
            State = GameState.Solved;
            StartCoroutine(ShowLastPiece());
        }
        return solved;
    }

    //To add last piece on completion
    IEnumerator ShowLastPiece()
    {
        while (true)
        {
            Blocks[EmptyRow, EmptyColumn].SetImage(ImageSlices[EmptyRow, EmptyColumn]);
            Blocks[EmptyRow, EmptyColumn].Number = 1;
            yield return new WaitForSeconds(0.2f);
            ScoreTracker.Instance.CurrentLevel = CurrentLevel + 1;
            GameOverText.text = "You Won";
            LevelText.gameObject.SetActive(true);
            LevelWon.text = ToRoman(CurrentLevel);
            TryAgain.text = "Next Level";
            GameOverPanel.SetActive(true);
        }
    }

    //To restart
    public void NewGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //Timer to check
    public IEnumerator StartCountdown()
    {
        while (SecondsRemaining >= 0)
        {
            yield return new WaitForSeconds(1.0f);
            SecondsRemaining--;
            if (SecondsRemaining == 0)
            {
                SecondsRemaining = 0;
                GameOverText.text = "Game Over";
                LevelText.gameObject.SetActive(false);
                TryAgain.text = "Try Again";
                GameOverPanel.SetActive(true);
            }
            TimeRemaining.text = SecondsRemaining.ToString();
        }
    }

    private void LoadImageForLevel()
    {
        try
        {
            CurrentLevel = ScoreTracker.Instance.CurrentLevel;
            ImageSprite = Resources.Load<Sprite>(LevelImageName[ScoreTracker.Instance.CurrentLevel]);
        }
        catch (KeyNotFoundException exception)
        {
            Debug.Log(exception.Data);
            CurrentLevel = 1;
            ImageSprite = Resources.Load<Sprite>(LevelImageName[1]);
            ScoreTracker.Instance.CurrentLevel = CurrentLevel;
        }

        LevelText.text = ToRoman(CurrentLevel);
        BlocksPerLine = CurrentLevel + 3;
        ShuffleCount = CurrentLevel * 20;
        SecondsRemaining = CurrentLevel * 60;
        TimeRemaining.text = SecondsRemaining.ToString();
    }

    public static string ToRoman(int number)
    {
        if ((number < 0) || (number > 3999)) throw new System.ArgumentOutOfRangeException("insert value betwheen 1 and 3999");
        if (number < 1) return string.Empty;
        if (number >= 1000) return "M" + ToRoman(number - 1000);
        if (number >= 900) return "CM" + ToRoman(number - 900);
        if (number >= 500) return "D" + ToRoman(number - 500);
        if (number >= 400) return "CD" + ToRoman(number - 400);
        if (number >= 100) return "C" + ToRoman(number - 100);
        if (number >= 90) return "XC" + ToRoman(number - 90);
        if (number >= 50) return "L" + ToRoman(number - 50);
        if (number >= 40) return "XL" + ToRoman(number - 40);
        if (number >= 10) return "X" + ToRoman(number - 10);
        if (number >= 9) return "IX" + ToRoman(number - 9);
        if (number >= 5) return "V" + ToRoman(number - 5);
        if (number >= 4) return "IV" + ToRoman(number - 4);
        if (number >= 1) return "I" + ToRoman(number - 1);
        throw new System.ArgumentOutOfRangeException("something bad happened");
    }
}
