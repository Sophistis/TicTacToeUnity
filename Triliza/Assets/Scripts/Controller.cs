using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class Controller : MonoBehaviour
{

    public int Turn; //0 is for x and 1 for O
    public int TurnCount; //counts how many turns are played
    public Sprite[] playerIcons; //0 is for x and 1 is for o
    public Button[] availableSpaces;
    public int[] markedSpaces; //Which space was marked by which player
    public Text winnerText; //Text component of the winner text
    public GameObject[] winningLine; //hold all the different lines where is winner
    public GameObject winningPanel; //a panel to prevent touching more buttons
    public int xScore; // score of X
    public int oScore; // score of O
    public Text xScoreText; //score on screen for X
    public Text oScoreText; //Score on screen for O
    public Button[] Level;  //For easy medium and hard
    public int LevelNumber; //the number of the level 0 for easy 1 for medium 2 for hard

    // Start is called before the first frame update
    void Start() //Sets the game
    {
        Level[0].interactable = false;
        LevelNumber = 0; 
        GameSetup();
    }
    void Update()
    {
        if (Turn==1)
        {
            int flag = 0; //checking if O move is done
            for (int i = 0; i < markedSpaces.Length; i++)
            {
                if ((markedSpaces[i] < 0) && (flag == 0))
                {
                    if (LevelNumber == 0)//For dummies, radomly plays
                    {
                        int Rand = Random.Range(0, 9); // Random from 0 to 8
                        if (markedSpaces[Rand] < 0) // Checks if the button is already played
                        {
                            StartCoroutine(WaitFunction(Rand));
                            flag = 1; //the move is done so it is X turns
                        }
                        else
                        {
                            StartCoroutine(WaitFunction(i)); //Waits a bit to check for the winner
                            flag = 1;
                        }
                    }
                    else //for medium and hard level
                    {
                        for (int r=0; r<markedSpaces.Length; r++)  //calls function to check all empty buttons
                        { 
                            int RecommendedPosition = NotEasyLevel(); // the function returns a position 
                            if ((RecommendedPosition >= 0) && (markedSpaces[RecommendedPosition] < 0))
                            {
                                StartCoroutine(WaitFunction(RecommendedPosition));
                                flag = 1;
                            }
                        }
                        int Rand = Random.Range(0, 9); //if there is not something to block or win a random button is plaid
                        if (markedSpaces[Rand] < 0)
                        {
                            StartCoroutine(WaitFunction(Rand));
                            flag = 1;
                        }
                        else
                        {
                            StartCoroutine(WaitFunction(i));
                            flag = 1;
                        }
                    }
                }
            }
        }   
    }
    void GameSetup() //Sets up the game and buttons
    {
        Turn = 0;
        TurnCount = 0; 
        for (int i = 0; i < availableSpaces.Length; i++) 
        {
            availableSpaces[i].interactable = true;
            availableSpaces[i].GetComponent<Image>().sprite = null;
        }
        for (int i = 0; i < markedSpaces.Length; i++) 
        {
            markedSpaces[i] = -100; //a drastic number for not causing other logic errors
        }
    }   
    public void ButtonClick(int ButtonNumber) //it is called for every button that is clicked
    {
        if (Turn == 0)
        {
            markedSpaces[ButtonNumber] = Turn + 100; //adding more to avoid causing more logic errors
        }
        else
        {
            markedSpaces[ButtonNumber] = Turn + 200; //adding more to avoid causing more logic errors
        }
        availableSpaces[ButtonNumber].image.sprite = playerIcons[Turn]; //shows X or O image on the button
        availableSpaces[ButtonNumber].interactable = false;
        TurnCount++;
        if (TurnCount > 3) // after 3 turns there is a winner
        {
            bool isWinner = WinnerCheck();
            if ((TurnCount==9)&& (isWinner == false)) //if there is no winner, game is Tie
            {
                Cat();
            }
        }
        if (Turn==0) 
        {
           Turn = 1;
        }
        else 
        {
            Turn = 0;
        }
    }
    public IEnumerator WaitFunction(int i) //Wait function for O AI. To avoid playing and checking for winner at the same time.
    {
        yield return new WaitForSeconds(0.1f);
        if (winningPanel.activeInHierarchy == true) 
        {
            yield break;
        }
            if (Turn == 1)
        {
            ButtonClick(i);
        }
    }
    bool WinnerCheck() //checks for winner
    {
        int s1 = markedSpaces[0] + markedSpaces[1] + markedSpaces[2]; //these are all the winning possible lines in Tic Tac Toe
        int s2 = markedSpaces[3] + markedSpaces[4] + markedSpaces[5];
        int s3 = markedSpaces[6] + markedSpaces[7] + markedSpaces[8];
        int s4 = markedSpaces[0] + markedSpaces[3] + markedSpaces[6];
        int s5 = markedSpaces[1] + markedSpaces[4] + markedSpaces[7];
        int s6 = markedSpaces[2] + markedSpaces[5] + markedSpaces[8];
        int s7 = markedSpaces[0] + markedSpaces[4] + markedSpaces[8];
        int s8 = markedSpaces[2] + markedSpaces[4] + markedSpaces[6];
        var solutions = new int[] { s1, s2, s3, s4, s5, s6, s7, s8 };
        for (int i =0; i<solutions.Length; i++)
        {
            if ((solutions[i] == 300)||(solutions[i] == 603))  //specific values for each winner
            {
                WinnerDisplay(i);
                return true;
            }
        }
        return false;
    }
    void WinnerDisplay(int indexIn) //When the is a winner, a display shows on the grid to make buttons inactive
    {
        winningPanel.gameObject.SetActive(true);
        if (Turn == 0)
        {
            xScore++;
            xScoreText.text = xScore.ToString();
            winnerText.text = "Winner!"; 

        }
        else
        {
            oScore++;
            oScoreText.text = oScore.ToString();
            winnerText.text = "Loser!";
        }
        winningLine[indexIn].SetActive(true);
    }
    public void Reset() //Resets the game but no scores
    {
        GameSetup();
        for (int i = 0; i < winningLine.Length; i++)
        {
            winningLine[i].SetActive(false);
        }
        winningPanel.SetActive(false);
    }
    public void RestartAll() //Restart everything in the game. Deletes scores
    {
        Reset();
        xScore = 0;
        oScore = 0;
        xScoreText.text = "0";
        oScoreText.text = "0";
    }
    public void ButtonManager(int ButtonNumber) //deactivates and activates buttons for Levels.
    {
        Reset();
        Level[ButtonNumber].interactable = false;
        LevelNumber = ButtonNumber;
        for (int i=0; i<Level.Length; i++)
        {
            if (i != ButtonNumber)
            {
                Level[i].interactable = true;
            }
        }
    }
    void Cat() //function when there is no winner
    {
        winningPanel.SetActive(true);
        winnerText.text = "CAT!";
    }
    public int NotEasyLevel() //for medium and hard level
    {
        int[,,] tGrid = //3D array to keep the player buttons and the position
        {
            {{markedSpaces[0],0}, {markedSpaces[1],1}, {markedSpaces[2],2} },
            {{markedSpaces[3],3}, {markedSpaces[4],4}, {markedSpaces[5],5} },
            {{markedSpaces[6],6}, {markedSpaces[7],7}, {markedSpaces[8],8} },
            {{markedSpaces[0],0}, {markedSpaces[3],3}, {markedSpaces[6],6} },
            {{markedSpaces[1],1}, {markedSpaces[4],4}, {markedSpaces[7],7} },
            {{markedSpaces[2],2}, {markedSpaces[5],5}, {markedSpaces[8],8} },
            {{markedSpaces[0],0}, {markedSpaces[4],4}, {markedSpaces[8],8} },
            {{markedSpaces[2],2}, {markedSpaces[4],4}, {markedSpaces[6],6} }
        };
        if (LevelNumber == 1) //medium level that blocks Xs for a triplet
        {
            for (int i = 0; i < 8; i++)
            {
                int checkflag = 0;
                int positionflag = 0;
                for (int y = 0; y < 3; y++)
                {
                    if (tGrid[i, y, 0] == 100) //checks if there are 2 Xs in a rοw in order to block them
                    {
                        checkflag = checkflag + 1;
                    }
                    else
                    {
                        positionflag = y;
                    }
                }
                if (checkflag == 2)
                {

                    if (tGrid[i, positionflag, 0] < 0)
                    {

                        return tGrid[i, positionflag, 1];
                    }
                }
            }
            return -100; //returns this value if there are not 2 Xs in a rοw
        }
        else //hard level that blocks Xs and also tries to win
        {
            for (int i = 0; i < 8; i++)
            {
                int checkflagX = 0;
                int checkflagO = 0;
                int positionflag = 0;
                for (int y = 0; y < 3; y++)
                {
                    if (tGrid[i, y, 0] == 201) //Checks if there are 2 Os in a rοw
                    {
                        checkflagX = checkflagX + tGrid[i, y, 0];
                    }
                    else if(tGrid[i, y, 0] == 100)  //blocks Xs in a row
                    {
                        checkflagO = checkflagO + tGrid[i, y, 0];
                    }
                    else
                    {
                        positionflag = y;
                    }
                    
                }
                if (checkflagX == 402) //Returns the position if Os are going to win
                {
                    if (tGrid[i, positionflag, 0] < 0) //checks if the button is empty
                    {
                        return tGrid[i, positionflag, 1];
                    }
                }else if (checkflagO == 200) //Returns the position to block Xs
                {
                    if (tGrid[i, positionflag, 0] < 0)
                    {
                        return tGrid[i, positionflag, 1];
                    }
                }
            }

            return -100; // returns this value if there is nothing to block or win
        }
    }
}
