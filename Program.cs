
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using System.Transactions;

while (true)
{
    Console.WriteLine("Would you like to play chess or draughts");
    string temp = (Console.ReadLine() + " ").ToLower();
    if (temp == "draughts ")
    {
        draughts();
    }
    else if (temp == "chess ")
    {
        chess();
    }
}

//Draughts ------------------------------------------------------------------------------------

void draughts()
{
    piece[,] board = generateBoard();
    board = populateBoardDraughts(board);
    displayBoard(board);

    player player1 = new player(false, 12, "Player 1");
    player player2 = new player(true, 12, "Player 2");

    bool gameEnd = false;
    while (gameEnd == false)
    {
        //player 1 turn
        TurnDraughts(board, player1, player2);
        if (player2.returnScore() == 0)
        {
            player1.giveWin();
            gameEnd = true;
            break;
        }

        //player 2 turn
        TurnDraughts(board, player2, player1);
        if (player1.returnScore() == 0)
        {
            player2.giveWin();
            gameEnd = true;
            break;
        }
    }

    winner(player1, player2);
}

static piece[,] TurnDraughts(piece[,] board, player player, player playerOther)
{
    bool turnOver = false;
    while (turnOver == false)
    {
        Console.WriteLine("\n{0}'s turn", player.returnName());
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

            int[] difference = findDifference(selectedPiece, desiredMove);

            if (checkValid(board, selectedPiece, desiredMove, player, difference, true) == true)
            {
                try
                {
                    //Move the piece
                    board[desiredMove / 10, desiredMove % 10] = board[selectedPiece / 10, selectedPiece % 10];
                    board[selectedPiece / 10, selectedPiece % 10] = null;

                    //Check if the piece has been upgraded into a king
                    checkIfUpgrade(board, desiredMove);

                    //check if an enemy piece has been destroyed
                    int midpoint = findMidpoint(selectedPiece, desiredMove);
                    if (checkAttackMove(selectedPiece, midpoint) == true)
                    {
                        board[midpoint / 10, midpoint % 10] = null;
                        playerOther.subtractScore(1);

                        if (canContinue(board, player) == false)
                        {
                            turnOver = true;
                        }
                    }
                    else
                    {
                        turnOver = true;
                    }

                    displayBoard(board);
                }
                catch
                {
                    Console.WriteLine("Move failed: Unknown error");
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Invalid input");
            //Console.WriteLine(e);
        }
    }
    return board;
}

static void checkIfUpgrade(piece[,] board, int desiredMove)
{
    if (desiredMove % 10 == 0)
    {
        board[desiredMove / 10, desiredMove % 10].upgrade();
        board[desiredMove / 10, desiredMove % 10].changeSymbol("X");
    }
}

static bool canContinue(piece[,] board, player player)
{
    for (int y = 0; y < 8; y++)
    {
        for (int x = 0; x < 8; x++)
        {
            try
            {
                if (board[x, y] != null)
                {
                    if (board[x, y].returnTeam() == player.returnTeam())
                    {
                        int[] difference = new int[2];

                        difference[0] = 2;
                        difference[1] = 2;
                        if ((x + difference[0] >= 0 && x + difference[0] < 8) && (y + difference[1] >= 0 && y + difference[1] < 8))
                        {
                            if (checkValid(board, (x * 10) + y, ((x + difference[0]) * 10) + y + difference[1], player, difference, false) == true)
                            {
                                return true;
                            }
                        }

                        difference[0] = 2;
                        difference[1] = -2;
                        if ((x + difference[0] >= 0 && x + difference[0] < 8) && (y + difference[1] >= 0 && y + difference[1] < 8))
                        {
                            if (checkValid(board, (x * 10) + y, ((x + difference[0]) * 10) + y + difference[1], player, difference, false) == true)
                            {
                                return true;
                            }
                        }

                        difference[0] = -2;
                        difference[1] = 2;
                        if ((x + difference[0] >= 0 && x + difference[0] < 8) && (y + difference[1] >= 0 && y + difference[1] < 8))
                        {
                            if (checkValid(board, (x * 10) + y, ((x + difference[0]) * 10) + y + difference[1], player, difference, false) == true)
                            {
                                return true;
                            }
                        }

                        difference[0] = -2;
                        difference[1] = -2;
                        if ((x + difference[0] >= 0 && x + difference[0] < 8) && (y + difference[1] >= 0 && y + difference[1] < 8))
                        {
                            if (checkValid(board, (x * 10) + y, ((x + difference[0]) * 10) + y + difference[1], player, difference, false) == true)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            catch { }
        }
    }

    return false;
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

                    piece BluePiece = new piece(true, "O");
                    board[x, y] = BluePiece;
                }
                else if (y >= 5)
                {
                    piece RedPiece = new piece(false, "O");
                    board[x, y] = RedPiece;
                }
            }
            i++;
        }
        i--;
    }

    return board;
}

static int findMidpoint(int selectedPiece, int desiredMove)
{
    int[] difference = findDifference(selectedPiece, desiredMove);

    int xSelect = selectedPiece / 10;
    int ySelect = selectedPiece % 10;

    int xMid = xSelect + (difference[0] / 2);
    int yMid = ySelect + (difference[1] / 2);

    int midpoint = (xMid * 10) + yMid;
    return midpoint;
}

static bool checkAttackMove(int selectedPiece, int midpoint)
{
    if (selectedPiece == midpoint)
    {
        return false;
    }
    return true;
}

static bool checkDraughtsValid(piece[,] board, int selectedPiece, int midpoint, int[] difference, bool blankMove, player player, piece selected, bool errorMessage)
{
    //See if the move is an attack move or not
    bool attackMove = checkAttackMove(selectedPiece, midpoint);

    if (blankMove == false)
    {
        if (errorMessage == true)
        {
            Console.WriteLine("Desired space is already occupied");
        }
        return false;
    }

    if (attackMove == false)
    {
        
        //Check that move is a possible option
        if (difference[0] == 1 || difference[0] == -1)
        {
            if (player.returnTeam() == false)
            {
                if (difference[1] == -1)
                {
                    return true;
                }
                if (difference[1] == 1 && selected.isUpgraded() == true)
                {
                    return true;
                }
            }

            if (player.returnTeam() == true)
            {
                if (difference[1] == 1)
                {
                    return true;
                }
                if (difference[1] == -1 && selected.isUpgraded() == true)
                {
                    return true;
                }
            }
            if (errorMessage == true)
            {
                Console.WriteLine("Invalid move");
            }
            return false;
        }

        if (errorMessage == true)
        {
            Console.WriteLine("Invalid move");
        }
        return false;
    }

    if (attackMove == true)
    {
        if (difference[0] == 2 || difference[0] == -2)
        {
            if (player.returnTeam() == false)
            {
                if (difference[1] == -2)
                {
                    if (board[midpoint / 10, midpoint % 10].returnTeam() == true)
                    {
                        return true;
                    }
                }
                if (difference[1] == 2 && selected.isUpgraded() == true)
                {
                    if (board[midpoint / 10, midpoint % 10].returnTeam() == true)
                    {
                        return true;
                    }
                }
            }

            if (player.returnTeam() == true)
            {
                if (difference[1] == 2)
                {
                    if (board[midpoint / 10, midpoint % 10].returnTeam() == false)
                    {
                        return true;
                    }
                }
                if (difference[1] == -2 && selected.isUpgraded() == true)
                {
                    if (board[midpoint / 10, midpoint % 10].returnTeam() == false)
                    {
                        return true;
                    }
                }
            }
        }
        if (errorMessage == true)
        {
            Console.WriteLine("Invalid move");
        }
        return false;
    }
    if (errorMessage == true)
    {
        Console.WriteLine("Invalid move");
    }
    return false;
}

//Chess ---------------------------------------------------------------------------------------

void chess()
{
    chessPiece[,] board = createBoardChess();
    

    player player1 = new player(false, -1, "Player 1");
    player player2 = new player(true, -1, "Player 2");

    bool gameEnd = false;
    while (gameEnd == false)
    {
        //player 1 turn
        turnChess(board, player1, player2);
        if (player1.checkWin() == true)
        {
            gameEnd = true;
            break;
        }

        //player 2 turn
        turnChess(board, player2, player1);
        if (player2.checkWin() == true)
        {
            gameEnd = true;
            break;
        }
    }

    winner(player1, player2);
}

static chessPiece[,] turnChess(chessPiece[,] board, player player, player playerOther)
{
    displayBoard(board);
    bool turnOver = false;
    while (turnOver == false)
    {
        Console.WriteLine("\n{0}'s turn", player.returnName());
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

            int[] difference = findDifference(selectedPiece, desiredMove);

            if (checkValid(board, selectedPiece, desiredMove, player, difference, true) == true)
            {
                if (checkValidChess(board, selectedPiece, desiredMove, player, difference, true) == true)
                {
                    try
                    {
                        //Check if the game has been won
                        if (board[desiredMove / 10, desiredMove % 10] != null)
                        {
                            if (board[desiredMove / 10, desiredMove % 10].returnTeam() == playerOther.returnTeam() && board[desiredMove / 10, desiredMove % 10].returnType() == "king")
                            {
                                player.giveWin();
                            }
                        }

                        //Move the piece
                        board[desiredMove / 10, desiredMove % 10] = board[selectedPiece / 10, selectedPiece % 10];
                        board[selectedPiece / 10, selectedPiece % 10] = null;

                        //Will upgrade piece if it can 
                        upgradeQueen(board, desiredMove, player);

                        turnOver = true;
                    }
                    catch
                    {
                        Console.WriteLine("Move failed: Unknown error");
                    }
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

static chessPiece[,] createBoardChess()
{
    chessPiece[,] board = new chessPiece[8, 8];

    board = row1(true, 0, board);
    board = row2(true, 1, board);

    board = row1(false, 7, board);
    board = row2(false, 6, board);

    static chessPiece[,] row1(bool team, int row, chessPiece[,] board)
    {
        board[0, row] = new chessPiece(team, "H", "Castle", "castle");
        board[7, row] = new chessPiece(team, "H", "Castle", "castle");

        board[1, row] = new chessPiece(team, "R", "Knight", "knight");
        board[6, row] = new chessPiece(team, "R", "Knight", "knight");

        board[2, row] = new chessPiece(team, "I", "Bishop", "bishop");
        board[5, row] = new chessPiece(team, "I", "Bishop", "bishop");

        board[3, row] = new chessPiece(team, "Q", "Queen", "queen");
        board[4, row] = new chessPiece(team, "K", "King", "king");

        return board;
    }

    static chessPiece[,] row2(bool team, int row, chessPiece[,] board)
    {
        for (int x = 0; x < 8; x++)
        {
            board[x, row] = new chessPiece(team, "o", "pawn", "pawn");
        }

        return board;
    }

    return board;
}

static bool checkValidChess(chessPiece[,] board, int selectedPiece, int desiredMove, player player, int[] difference, bool errorMessage)
{
    chessPiece selected = board[selectedPiece / 10, selectedPiece % 10];

    if (selected.returnType() == "pawn")
    {
        int multiplier = 1; //used to determine whether the pawn can move 2 squares
        int direction = 1;

        if (selected.returnMoveNum() == 0)
        {
            multiplier = 2;
        }

        if (selected.returnTeam() == false)
        {
            direction = -1;
        }

        if (board[desiredMove / 10, desiredMove % 10] == null) // Not taking a piece
        {
            if (difference[0] == 0)
            {
                if (difference[1] == 1 * direction || difference[1] == 1 * direction * multiplier)
                {
                    return true;
                }
            }
        }
        else if (board[desiredMove / 10, desiredMove % 10].returnTeam() != player.returnTeam()) //Taking a piece
        {
            if (difference[0] == 1 || difference[0] == -1)
            {
                if (difference[1] == 1 * direction)
                {
                    return true;
                }
            }
        }

        if (errorMessage == true)
        {
            Console.WriteLine("Invalid Move");
        }
        return false;
    }else
    {
        for (int i = 0; i < selected.returnMoves().GetLength(0); i++)
        {
            if (difference[0] == selected.returnMoves()[i,0] && difference[1] == selected.returnMoves()[i, 1])
            {
                if (board[desiredMove / 10, desiredMove % 10] == null)
                {
                    return true;
                }

                if (board[desiredMove / 10, desiredMove % 10].returnTeam() != player.Team)
                {
                    return true;
                }
            }

            if (selected.returnMoves()[i,0] % 9 == 0 || selected.returnMoves()[i,1] % 9 == 0)
            {
                if (longMove(board, selected, player, selectedPiece, desiredMove, false, i) == true)
                {
                    return true;
                }
            }
        }
    }

    Console.WriteLine("Invalid move");
    return false;
}

static bool longMove(chessPiece[,] board, chessPiece selected, player player, int selectedPiece, int desiredMove, bool errorMessage, int positionInMoveQueue)
{
    int x = 0;
    int y = 0;

    int[] selectedList = new int[2];
    selectedList[0] = selectedPiece / 10;
    selectedList[1] = selectedPiece % 10;

    if (selected.returnMoves()[positionInMoveQueue, 0] % 9 == 0)
    {
        x = selected.returnMoves()[positionInMoveQueue, 0] / 9;
    }

    if (selected.returnMoves()[positionInMoveQueue, 1] % 9 == 0)
    {
        y = selected.returnMoves()[positionInMoveQueue, 1] / 9;
    }

    while (true)
    {
        selectedList[0] += x;
        selectedList[1] += y;

        //check if out of bouns
        if (selectedList[0] < 0 || selectedList[0] > 7)
        {
            if (errorMessage == true)
            {
                Console.WriteLine("Move Invalid (Long-Move) (Move out of bounds)");
            }
            return false;
        }
        if (selectedList[1] < 0 || selectedList[1] > 7)
        {
            if (errorMessage == true)
            {
                Console.WriteLine("Move Invalid (Long-Move) (Move out of bounds)");
            }
            return false;
        }

        //check if this is the move desired
        if (desiredMove / 10 == selectedList[0] && desiredMove % 10 == selectedList[1])
        {
            if (board[selectedList[0], selectedList[1]] == null)
            {
                return true;
            }

            if (board[selectedList[0], selectedList[1]].returnTeam() != player.returnTeam())
            {
                return true;
            }
        }

        //check not blocked
        if (board[selectedList[0], selectedList[1]] != null)
        {
            if (errorMessage == true)
            {
                Console.WriteLine("Move Invalid (Long-Move)");
            }
            return false;
        }
    }
}

static chessPiece[,] upgradeQueen(chessPiece[,] board, int desiredMove, player player) //To finish =================================================================================
{
    return board;
}

//Generic functions ---------------------------------------------------------------------------

static int convertMove(string preMove)
{
    preMove = preMove.ToLower();

    char a = preMove[0];
    char b = preMove[1];

    int a1 = Convert.ToInt32(a - 97);
    int b1 = Convert.ToInt32(b - 49);

    int newMove = (a1 * 10) + b1;
    return newMove;
}

static void winner(player player1, player player2)
{
    if (player1.checkWin() == true)
    {
        Console.WriteLine("Player 1 wins!");
    }
    if (player2.checkWin() == true)
    {
        Console.WriteLine("Player 2 wins!");
    }
}

static piece[,] generateBoard()
{
    piece[,] board = new piece[8, 8];
    return board;
}

static bool checkValid(piece[,] board ,int selectedPiece, int desiredMove, player player, int[] difference, bool errorMessage)
{
    //selected piece
    piece selected = board[selectedPiece / 10, selectedPiece % 10];

    //desired move space
    piece desired = null;
    bool blankMove = false;
    try
    {
        if (board[desiredMove / 10, desiredMove % 10] == null)
        {
            blankMove = true;
        }
        else
        {
            desired = board[desiredMove / 10, desiredMove % 10];
        }
    }
    catch { return false; }

    //Check the piece isn't being moved to the same place it started
    if (selectedPiece == desiredMove)
    {
        if (errorMessage == true)
        {
            Console.WriteLine("You cannot move to the space you already occupy");
        }
        return false;
    }

    //Check slected piece is on players team
    if (selected.returnTeam() != player.returnTeam())
    {
        if (errorMessage == true)
        {
            Console.WriteLine("Selected Piece not on your team");
        }
        return false;
    }
    //check piece types
    if (selected.isDraughts() == true)
    {
        int midpoint = findMidpoint(selectedPiece, desiredMove);
        return checkDraughtsValid(board, selectedPiece, midpoint, difference, blankMove, player, selected, errorMessage);
    }

    return true;
}

static int[] findDifference(int selectedPiece, int desiredMove)
{
    int[] difference = new int[2];

    difference[0] = (desiredMove / 10) - (selectedPiece / 10);
    difference[1] = (desiredMove % 10) - (selectedPiece % 10);

    return difference;
}

static void displayBoard(piece[,] board)
{
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine("| |A|B|C|D|E|F|G|H|");
    for (int y = 0; y < 8; y++)
    {
        Console.Write("|{0}", y + 1);
        for (int x = 0; x < 8; x++)
        {
            Console.Write("|");
            if (board[x, y] == null)
            {
                Console.Write(" ");
            } 
            else if (board[x, y].Team == false)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("{0}", board[x, y].returnSymbol());
                Console.ForegroundColor = ConsoleColor.White;
            }
            else if (board[x, y].Team == true)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("{0}", board[x, y].returnSymbol());
                Console.ForegroundColor = ConsoleColor.White;
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
    protected string Name = "player";
    public player(bool team, int score, string name)
    {
        Team = team;
        Score = score;
        Name = name;
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

    public string returnName()
    {
        return Name;
    }
}

public class piece
{
    public bool Team = false;
    protected bool Draughts = true;
    protected bool Upgraded = false;
    protected string Symbol = " ";
    public piece(bool team, string symbol)
    {
        Team = team;
        Symbol = symbol;
    }

    public bool isDraughts()
    {
        return Draughts;
    }

    public bool returnTeam()
    {
        return Team;
    }

    public void upgrade()
    {
        Upgraded = true;
    }

    public bool isUpgraded()
    {
        return Upgraded;
    }

    public string returnSymbol()
    {
        return Symbol;
    }

    public void changeSymbol(string NewSymbol) 
    {
        if (NewSymbol != null)
        {
            Symbol = NewSymbol;
        }
        else
        {
            Console.WriteLine("Error, null symbol change");
        }
    }
}

public class chessPiece : piece
{
    protected string Name = "";
    protected string Type = "";
    protected int[,] PossibleMoves = null;
    protected int moveNum = 0;

    public chessPiece(bool team, string symbol, string name, string type) : base(team, symbol)
    {
        Name = name;
        Type = type;
        Draughts = false;

        if (type == "king")
        {
            PossibleMoves = new int[8, 2]
            {
                {1, 1}, {1, 0}, {1, -1}, {0, 1}, {0, -1}, {-1, 1}, {-1, 0}, {-1, -1}
            };
        } else if (type == "queen")
        {
            PossibleMoves = new int[8, 2]
            {
                {9, 9}, {9, 0}, {9, -9}, {0, 9}, {0, -9}, {-9, 9}, {-9, 0}, {-9, -9}
            };
        } else if (type == "kight")
        {
            PossibleMoves = new int[8, 2]
            {
                {2, 1}, {2, -1}, {-2, 1}, {-2, -1}, {1, 2}, {1, -2}, {-1, 2}, {-1, -2}
            };
        } else if (type == "bishop")
        {
            PossibleMoves = new int[4, 2]
            {
                {9, 9}, {9, -9}, {-9, 9}, {-9, -9}
            };
        } else if (type == "castle")
        {
            PossibleMoves = new int[4, 2]
            {
                {9, 0}, {0, 9}, {0, -9}, {-9, 0}
            };
        }
    }

    public int[,] returnMoves()
    {
        return PossibleMoves;
    }

    public string returnName()
    {
        return Name;
    }

    public string returnType()
    {
        return Type;
    }

    public void incrementMoveNum()
    {
        moveNum++;
    }

    public int returnMoveNum()
    {
        return moveNum;
    }
}
