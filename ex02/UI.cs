using System;

namespace ex02
{
    internal class UI
    {
        private enum eRematch { Yes = 1, No = 0};
        private const string k_computersName = "computer";

        private static void createGame(out GameConfig i_game, string i_firstPlayersName, string i_secondPlayersName, int i_boardSize, int i_numOfHumanPlayers)
        {
            CheckersBoard myGameboard;
            Player firstPlayer;
            Player secondPlayer;
            bool isHuman = true;

            myGameboard = new CheckersBoard(i_boardSize);
            firstPlayer = new Player(Player.ePlayerType.O, i_firstPlayersName, isHuman, GameConfig.k_numOfInitialGamePieces, myGameboard.Board, i_boardSize);
            isHuman = i_numOfHumanPlayers == 2;
            secondPlayer = new Player(Player.ePlayerType.X, i_secondPlayersName, isHuman, GameConfig.k_numOfInitialGamePieces, myGameboard.Board, i_boardSize);
            i_game = new GameConfig(myGameboard, firstPlayer, secondPlayer);
        }

        private static void getInputFromUser(out string o_firstPlayersName, out string o_secondPlayersName, out int o_boardSize, out int o_numOfHumanPlayers)
        {
            getPlayersName(out o_firstPlayersName);

            Console.WriteLine("Enter gameboard size (options are 6, 8, or 10):");
            while (!int.TryParse(Console.ReadLine(), out o_boardSize) || !GameConfig.IsValidBoardSize(o_boardSize))
            {
                Console.WriteLine("Invalid gameboard size. Enter a valid size (6, 8, or 10):");
            }

            Console.WriteLine("Enter 1 to play against the computer or 2 to play against a friend:");
            while (!int.TryParse(Console.ReadLine(), out o_numOfHumanPlayers) || (o_numOfHumanPlayers != 1 && o_numOfHumanPlayers != 2))
            {
                Console.WriteLine("Invalid choice");
                Console.WriteLine("Enter 1 to play against the computer or 2 to play against a friend:");
            }

            if (o_numOfHumanPlayers == 2)
            {
               getPlayersName(out o_secondPlayersName);
            }
            else
            {
                o_secondPlayersName = k_computersName;
            }
        }

        private static void getPlayersName(out string o_playersName)
        {
            Console.WriteLine("Enter your name:");
            o_playersName = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(o_playersName))
            {
                Console.WriteLine("Invalid name. Please enter a valid name:");
                o_playersName = Console.ReadLine();
            }
        }

        private static void printBoard(GamePiece?[,] i_board, int i_size)
        {
            string columnLabels = "   ";
            for (int i = 0; i < i_size; ++i)
            {
                columnLabels += (char)('a' + i) + "   ";
            }

            Console.WriteLine(columnLabels);

            string separatorLine = "  ================================= ";
            int separatorLength = 3 + i_size * 4;

            separatorLine = new string('=', separatorLength);
            Console.WriteLine(separatorLine);
            for (int i = 0; i < i_size; ++i)
            {
                char rowLabel = (char)('A' + i);

                Console.Write(rowLabel + "| ");
                for (int j = 0; j < i_size; ++j)
                {
                    if (i_board[i, j] == null)
                    {
                        Console.Write("  | ");
                    }
                    else
                    {
                        Console.Write(i_board[i, j].Sign + " | ");
                    }
                }

                Console.WriteLine();
                Console.WriteLine(separatorLine);
            }
        }

        private static void getPlayersMove(out Move o_playersMove, out string o_input)
        {
            CheckersBoard.BoardPoint currentPosition;
            CheckersBoard.BoardPoint destination;

            while (true)
            {
                Console.WriteLine("Please enter your move in the format of ROWcol>ROWcol:");
                o_input = Console.ReadLine();
                if(o_input == "Q")
                {
                    o_playersMove = null;
                    return;
                }

                if (o_input.Length == 5 &&
                    char.IsLetter(o_input[0]) && char.IsLower(o_input[1]) && o_input[2] == '>' &&
                    char.IsLetter(o_input[3]) && char.IsLower(o_input[4]))
                {
                    break;
                }

                Console.WriteLine("Invalid input");
            }

            currentPosition = new CheckersBoard.BoardPoint(o_input[0] - 'A', o_input[1] - 'a');
            destination = new CheckersBoard.BoardPoint(o_input[3] - 'A', o_input[4] - 'a');
            o_playersMove = new Move(currentPosition, destination);
        }

        private static void render(bool i_shouldClearScreen = false, bool i_shouldPrintBoard = false, GamePiece?[,] i_board = null, int i_size = 0)
        {
            if(i_shouldClearScreen)
            {
                Ex02.ConsoleUtils.Screen.Clear();
            }
            
            if(i_shouldPrintBoard)
            {
                printBoard(i_board, i_size);
            }
            
        }

        public static void StartProgram()
        {
            string firstPlayersName;
            string secondPlayersName;
            int boardSize;
            int numOfHumanPlayers;
            GameConfig myGame;
            bool shouldPrintBoard = true;

            getInputFromUser(out firstPlayersName, out secondPlayersName, out boardSize, out numOfHumanPlayers);
            createGame(out myGame, firstPlayersName, secondPlayersName, boardSize, numOfHumanPlayers);
            myGame.SetPlayers();
            render(i_shouldPrintBoard: shouldPrintBoard, i_board: myGame.Gameboard.Board, i_size: myGame.Gameboard.Size);
            runGame(myGame);
        }

        private static void runGame(GameConfig i_game)
        {
            string input = string.Empty;
            Move playersChoiceOfMove;
            Move playersValidatedMove;
            bool wasQPressed = false;
            bool shouldPrintBoard = true;

            while (i_game.GameStatus == GameConfig.eGameStatus.GameIsOnGoing)
            {
                i_game.ClearPlayersMovesForNextTurn();
                if (i_game.CurrentPlayer.IsHumanPlayer)
                {
                    while(true)
                    {
                        UI.getPlayersMove(out playersChoiceOfMove, out input);
                        if (playersChoiceOfMove == null) 
                        {
                            wasQPressed = true;
                            endGame(i_game, wasQPressed);
                            return;
                        }
                        else if (i_game.IsMoveValid(playersChoiceOfMove, out playersValidatedMove))
                        {
                            break;
                        }

                        Console.WriteLine("Invalid move");
                    }

                }
                else
                {
                    i_game.GetComputersMove(out playersValidatedMove);
                    printComputersTurn(playersValidatedMove);
                }

                while(true)
                {
                    i_game.MakeAMove(playersValidatedMove, out Move possibleFollowingSkippingMove);
                    if(possibleFollowingSkippingMove != null)
                    {
                        playersValidatedMove = possibleFollowingSkippingMove;
                    }
                    else
                    {
                        break;
                    }

                }

                bool shouldClearScreen = i_game.OtherPlayer.IsHumanPlayer;
                render(shouldClearScreen, shouldPrintBoard, i_game.Gameboard.Board, i_game.Gameboard.Size);
                printHumanPlayersMove(i_game, input);
            }

            endGame(i_game, wasQPressed);
        }

        private static void endGame(GameConfig i_game, bool i_wasQPressed)
        {
            bool shouldClearScreen = true;

            render(i_shouldClearScreen: shouldClearScreen);
            printGameResult(i_game, i_wasQPressed);
            if (!i_wasQPressed && i_game.GameStatus != GameConfig.eGameStatus.Draw)
            {
                i_game.UpdateGamescore();
            }

            if(!i_wasQPressed)
            {
                printGamescore(i_game);
            }
          
            checkForARematch(i_game);
        }

        private static void checkForARematch(GameConfig i_game)
        {
            string input;

            while (true)
            {
                Console.WriteLine("Press 1 if you wish to play again, otherwise press 0");

                input = Console.ReadLine();
                if (int.TryParse(input, out int usersAnswer))
                {
                    if (usersAnswer == (int)eRematch.Yes)
                    {
                        resetGame(i_game);
                        runGame(i_game);
                        break;
                    }
                    else if(usersAnswer == (int)eRematch.No)
                    {
                        Console.WriteLine("Thank you for playing. Goodbye!");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Invalid input");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid number.");
                }

            }

        }

        private static void resetGame(GameConfig i_game)
        {
            bool shouldClearScreen = true;
            bool shouldPrintBoard = true;

            i_game.GameStatus = GameConfig.eGameStatus.GameIsOnGoing;
            i_game.Gameboard = new CheckersBoard(i_game.Gameboard.Size);
            i_game.FirstPlayer.InitializeSetOfGamePieces(i_game.Gameboard.Board, i_game.Gameboard.Size);
            i_game.SecondPlayer.InitializeSetOfGamePieces(i_game.Gameboard.Board, i_game.Gameboard.Size);
            i_game.FirstPlayer.NumOfGamePieces = GameConfig.k_numOfInitialGamePieces;
            i_game.SecondPlayer.NumOfGamePieces = GameConfig.k_numOfInitialGamePieces;
            i_game.FirstPlayer.NumOfKings = 0;
            i_game.SecondPlayer.NumOfKings = 0;
            i_game.SetPlayers();
            render(shouldClearScreen, shouldPrintBoard, i_game.Gameboard.Board, i_game.Gameboard.Size);
        }

        private static string convertMoveToInputFormat(Move i_move)
        {
            char currentRow = (char)('A' + i_move.StartPoint.X);
            char currentColumn = (char)('a' + i_move.StartPoint.Y);

            char destinationRow = (char)('A' + i_move.EndPoint.X);
            char destinationColumn = (char)('a' + i_move.EndPoint.Y);

            string formattedMove = string.Format("{0}{1}>{2}{3}", currentRow, currentColumn, destinationRow, destinationColumn);

            return formattedMove;
        }

        private static void printGamescore(GameConfig i_game)
        {
            string msg = string.Format(
                @"{0}'s score: {1}
{2}'s score: {3}",
                i_game.FirstPlayer.Name, i_game.FirstPlayer.Gamescore,
                i_game.SecondPlayer.Name, i_game.SecondPlayer.Gamescore);

            Console.WriteLine(msg);
        }

        private static void printComputersTurn(Move i_computerMove)
        {
            string formattedMove = convertMoveToInputFormat(i_computerMove);
            bool isClear = true;
            Console.WriteLine("Computer’s Turn (press ‘enter’ to see its move)");
            ConsoleKeyInfo key = Console.ReadKey();
            while (key.Key != ConsoleKey.Enter)
            {
                Console.WriteLine();
                Console.WriteLine("Invalid input");
                key = Console.ReadKey();
            }

            render(i_shouldClearScreen: isClear);  
            Console.WriteLine("computer's move was : {0}", formattedMove);
        }

        private static void printHumanPlayersMove(GameConfig i_game, string i_input)
        {
            string msg = string.Empty;

            if(i_game.OtherPlayer.IsHumanPlayer && i_game.CurrentPlayer.IsHumanPlayer)
            {
                msg = string.Format(
                   @"{0}'s move was ({1}) : {2}
{3}'s Turn ({4}) :",
                   i_game.OtherPlayer.Name, i_game.OtherPlayer.TypeOfPlayer, i_input,
                   i_game.CurrentPlayer.Name, i_game.CurrentPlayer.TypeOfPlayer);

            }
            else if(i_game.OtherPlayer.IsHumanPlayer)
            {
                msg = string.Format("{0}'s move was ({1}): {2}", i_game.OtherPlayer.Name, i_game.OtherPlayer.TypeOfPlayer, i_input);
            }

            Console.WriteLine(msg);          
        }

        private static void printGameResult(GameConfig i_game, bool i_wasQPressed)
        {
            string msg = string.Empty;

            if (i_wasQPressed)
            {
                msg = string.Format("{0} wins!", i_game.OtherPlayer.Name);
            }
            else if (i_game.GameStatus == GameConfig.eGameStatus.FirstPlayerWins)
            {
                msg = string.Format("{0} wins!", i_game.FirstPlayer.Name);
            }
            else if (i_game.GameStatus == GameConfig.eGameStatus.SecondPlayerWins)
            {
                msg = string.Format("{0} wins!", i_game.SecondPlayer.Name);
            }
            else
            {
                msg = string.Format("Draw");
            }

            Console.WriteLine(msg);
        }
    }
}

