
using System;
using System.IO;
using System.Collections.Generic;
namespace Tictactoe
{
    public class Board
    {
        private string[,] gameBoard;
        public Board()
        {
            gameBoard = new string[3, 3];
            InitializeBoard();
        }
        private void InitializeBoard()
        {
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    gameBoard[row, col] = "-";
                }
            }
        }
        public void CreateBoard()
        {
            Console.WriteLine("    0   1   2");
            Console.WriteLine("  +---+---+---+");
            for (int row = 0; row < 3; row++)
            {
                Console.Write(row + " ");
                for (int col = 0; col < 3; col++)
                {
                    Console.Write("| " + gameBoard[row, col] + " ");
                }
                Console.WriteLine("|");
                Console.WriteLine("  +---+---+---+");
            }
        }
        public void UpdateSymbolOnBoard(int row, int col, string symbol)
        {
            gameBoard[row, col] = symbol;
        }
        public bool IsFull()
        {
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    if (gameBoard[row, col] == "-")
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public bool IsAvailable(int row, int col)
        {
            int numRows = gameBoard.GetLength(0);
            int numCols = gameBoard.GetLength(1);

            if (row < 0 || row >= numRows || col < 0 || col >= numCols)
            {
                return false;
            }

            return gameBoard[row, col] == "-";
        }
        public bool CheckWin()
        {
            for (int row = 0; row < 3; row++)
            {
                if (gameBoard[row, 0] != "-" &&
                    gameBoard[row, 0] == gameBoard[row, 1] &&
                    gameBoard[row, 1] == gameBoard[row, 2])
                {
                    return true;
                }
            }

            // Check columns for win
            for (int col = 0; col < 3; col++)
            {
                if (gameBoard[0, col] != "-" &&
                    gameBoard[0, col] == gameBoard[1, col] &&
                    gameBoard[1, col] == gameBoard[2, col])
                {
                    return true;
                }
            }

            // Check diagonals for win
            if (gameBoard[0, 0] != "-" &&
                gameBoard[0, 0] == gameBoard[1, 1] &&
                gameBoard[1, 1] == gameBoard[2, 2])
            {
                return true;
            }
            if (gameBoard[0, 2] != "-" &&
                gameBoard[0, 2] == gameBoard[1, 1] &&
                gameBoard[1, 1] == gameBoard[2, 0])
            {
                return true;
            }

            // No win found
            return false;
        }
        public bool CheckDraw()
        {
            {
                return IsFull() && !CheckWin();
            }
        }
    }
    public abstract class Player
    {
        public string Name { get; set; }
        public string Symbol { get; set; }
        public abstract void Move(Board board);
    }
    public class Human : Player
    {
        public override void Move(Board board)
        {
            Console.WriteLine("{0}'s turn ({1}):", Name, Symbol);
            int row, col;

            do
            {
                Console.Write("Enter row (0-2): ");
                bool isValidRow = int.TryParse(Console.ReadLine(), out row);
                Console.Write("Enter column (0-2): ");
                bool isValidCol = int.TryParse(Console.ReadLine(), out col);

                if (!isValidRow || !isValidCol)
                {
                    Console.WriteLine("Invalid input. Please enter a number between 0 and 2.");
                    continue;
                }

                if (!board.IsAvailable(row, col))
                {
                    Console.WriteLine("That position is already occupied. Please choose another position.");
                    continue;
                }

                break;
            } while (true);

            board.UpdateSymbolOnBoard(row, col, Symbol);
        }

    }
    public class Computer : Player
    {
        private Random random;

        public Computer()
        {
            random = new Random();
        }

        public override void Move(Board board)
        {
            
        }
    }
    public class SaveFile
    {
        public SaveFile() { }

        public void SaveGameResults(List<string> gameResults)
        {
            string filePath = @"D:\TictAcToe\gameResults.txt";

            try
            {
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    foreach (string result in gameResults)
                    {
                        sw.WriteLine(result);
                    }
                }
                Console.WriteLine("Game results saved successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving game results: {ex.Message}");
            }
        }
    }
    public class Game
    {
        private Board board;
        private Player player1;
        private Player player2;
        private static int numberOfPlays = 0;
        public int Draws { get; set; }
        private int player1Score = 0;
        private int player2Score = 0;
        public int GetPlayerScore(Player player)
        {
            return player == player1 ? player1Score : player2Score;
        }
        public Game(Board board, Player player1, Player player2)
        {
            this.board = board;
            this.player1 = player1;
            this.player2 = player2;
        }
        private List<string> gameResults = new List<string>();

        public void AddGameResult(string result, int numberOfPlays)
        {

            gameResults.Add(string.Format("{0} ({1} plays)", result, numberOfPlays));
            SaveFile saveFile = new SaveFile();
            saveFile.SaveGameResults(gameResults);
        }

        public static void IncrementNumberOfPlays()
        {
            numberOfPlays++;
        }

        public List<string> GetGameResults()
        {
            return gameResults;
        }

        public void GameLoop()
        {
            bool player1Turn = true;

            while (!board.IsFull() && !board.CheckWin())
            {
                Console.Clear();
                board.CreateBoard();

                if (player1Turn)
                {
                    player1.PlayerMove(board);
                    player1Score++;
                }
                else
                {
                    player2.PlayerMove(board);
                    player2Score++;
                }

                player1Turn = !player1Turn;
                IncrementNumberOfPlays();
            }

            Console.Clear();
            board.CreateBoard();

            if (board.CheckWin())
            {
                Console.WriteLine("Congratulations! {0} won the game.", player1Turn ? player2.Name : player1.Name);
                AddGameResult(string.Format("Winner: {0}", player1Turn ? player2.Name : player1.Name), numberOfPlays);
            }
            else
            {
                Console.WriteLine("It's a draw!");
                AddGameResult("Draw", numberOfPlays);
            }

        }
        public class Program
        {
            static void Main(string[] args)
            {
                bool playAgain = true;

                do
                {
                    Console.WriteLine("Welcome to Tic Tac Toe game");
                    Console.WriteLine("1. Player vs. Player");
                    Console.WriteLine("2. Player vs. CPU");
                  
                    Console.WriteLine("4. Quit");

                    bool validChoice;
                    int choice;
                    do
                    {
                        Console.Write("Please enter your choice (1-4): ");
                        validChoice = Int32.TryParse(Console.ReadLine(), out choice);
                        if (!validChoice || choice < 1 || choice > 4)
                        {
                            Console.WriteLine("Invalid choice. Please enter a number between 1 and 4.");
                        }
                    } while (!validChoice || choice < 1 || choice > 4);

                    if (choice == 1)
                    {
                        Console.Write("Enter Player 1's name: ");
                        string name1 = Console.ReadLine();
                        Console.Write("Enter Player 1's symbol (X or O): ");
                        string symbol1 = Console.ReadLine().ToUpper();

                        Console.Write("Enter Player 2's name: ");
                        string name2 = Console.ReadLine();
                        string symbol2 = (symbol1 == "X") ? "O" : "X";

                        Board board = new Board();
                        Player player1 = new Human { Name = name1, Symbol = symbol1 };
                        Player player2 = new Human { Name = name2, Symbol = symbol2 };
                        Game game = new Game(board, player1, player2);
                        game.GameLoop();
                    }
                    else if (choice == 2)
                    {
                        Console.Write("Enter your name: ");
                        string name = Console.ReadLine();
                        Console.Write("Enter your symbol (X or O): ");
                        string symbol = Console.ReadLine().ToUpper();

                        Board board = new Board();
                        Player player1 = new Human { Name = name, Symbol = symbol };
                        Player player2 = new Computer { Name = "Computer", Symbol = (symbol == "X") ? "O" : "X" };
                        Game game = new Game(board, player1, player2);
                        game.GameLoop();
                    }
                   
                    
                    else if (choice == 4)
                    {
                        return;
                    }

                    Console.Write("Do you want to play again? (Y/N) ");
                    string playAgainChoice = Console.ReadLine();
                    if (playAgainChoice.ToUpper() != "Y")
                    {
                        playAgain = false;
                    }

                } while (playAgain);
            }
        }





    }
}







