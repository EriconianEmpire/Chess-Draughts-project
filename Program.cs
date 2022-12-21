
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;

while (true)
{
    Console.WriteLine("Would you like to play chess or draughts");
    string temp = (Console.ReadLine() + " ").ToLower();
    if (temp == "draughts ")
    {
        draughts();
    }
}

//Draughts ------------------------------------------------------------------------------------

void draughts()
{
    piece[,] board = generateBoard();
    board = populateBoardDraughts(board);
    displayBoard(board);

    player player1 = new player(false, 12);
    player player2 = new player(true, 12);

    bool gameEnd = false;
    while (gameEnd == false)
    {
        Turn(board, player1);
    }
}

static piece[,] Turn(piece[,] board, player player)
{
    bool turnOver = false;
    while (turnOver == false)
    {
        try
        {
            Console.WriteLine("Which piece would you like to move?");
            int selectedPiece = convertMove(Console.ReadLine());

            //check piece is selected
            if (board[selectedPiece / 10, selectedPiece % 10] == null)
            {
                throw new Exception("No piece selected");
            }

            Console.WriteLine("Where would you like to move it?");
            int desiredMove = convertMove(Console.ReadLine());

            
            int difference = desiredMove - selectedPiece;

            if (checkValid(board, selectedPiece, desiredMove, player, difference) == true)
            {
                try
                {
                    
                    
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Invalid input");
            Console.WriteLine(e);
        }
    }
    return board;
}

static int convertMove(string preMove)
{
    char a = preMove[0];
    char b = preMove[1];

    int a1 = Convert.ToInt32(a - 97);
    int b1 = Convert.ToInt32(b - 49);

    int newMove = (a1 * 10) + b1;
    return newMove;
}

static piece[,] populateBoardDraughts(piece[,] board)
{
    int i = 1;
    for (int y = 0; y < 8; y++)
    {
        for (int x = 0; x < 8; x++)
        {
            if (i % 2 == 0)
            {
                if (y <= 2)
                {

                    piece WhitePiece = new piece(false);
                    board[x, y] = WhitePiece;
                }
                else if (y >= 5)
                {
                    piece BlackPiece = new piece(true);
                    board[x, y] = BlackPiece;
                }
            }
            i++;
        }
        i--;
    }

    return board;
}

//Generic functions ---------------------------------------------------------------------------

static piece[,] generateBoard()
{
    piece[,] board = new piece[8, 8];
    return board;
}

static bool checkValid(piece[,] board ,int selectedPiece, int desiredMove, player player, int difference)
{
    //selected piece
    piece selected = board[selectedPiece / 10, selectedPiece % 10];

    //desired move space
    bool blankMove = false;
    try
    {
        piece desired = board[desiredMove / 10, desiredMove % 10];
    }
    catch
    {
        blankMove = true;
    }

    //Check slected piece is on players team
    if (selected.returnTeam == player.returnTeam)
    {
        Console.WriteLine("Selected Piece not on your team");
        return false;
    }
    //check piece types
    if (selected.isDraughts() == true)
    {
        if (blankMove == false)
        {
            Console.WriteLine("Desired space is already occupied");
            return false;
        }

        int[] possibleMoves = new int[22,]
        if ()
        return true;
    }

    return false;
}

static void displayBoard(piece[,] board)
{
    for (int y = 0; y < 8; y++)
    {
        for (int x = 0; x < 8; x++)
        {
            Console.Write("|");
            if (board[x, y] == null)
            {
                Console.Write(" ");
            } 
            else if (board[x, y].Team == false)
            {
                Console.Write("x");
            }
            else if (board[x, y].Team == true)
            {
                Console.Write("O");
            }
        }
        Console.Write("|\n");
    }
}

//Classes -------------------------------------------------------------------------------------

public class player
{
    public bool Team = false;
    protected int Score = -1;
    protected bool Win = false;
    public player(bool team, int score)
    {
        Team = team;
        Score = score;
    }

    public void subtractScore(int amount)
    {
        Score = Score - amount;
    }

    public int returnScore()
    {
        return Score;
    }

    public bool returnTeam()
    {
        return Team;
    }

    public bool checkWin()
    {
        return Win;
    }

    public void giveWin()
    {
        Win = true;
    }
}

public class piece
{
    public bool Team = false;
    protected bool Draughts = true;
    public piece(bool team)
    {
        Team = team;
    }

    public bool isDraughts()
    {
        return Draughts;
    }

    public bool returnTeam()
    {
        return Team;
    }
}

public class chessPiece : piece
{
    protected string Name = "";
    protected string Type = "";

    public chessPiece(bool team, string name, string type) : base(team)
    {
        Name = name;
        Type = type;
        Draughts = false;
    }

    public string returnName()
    {
        return Name;
    }

    public string returnType()
    {
        return Type;
    }
}
