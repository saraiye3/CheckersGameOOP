using System;
using System.Collections.Generic;

namespace ex02
{
    internal class GameConfig
    {
        public enum eGameStatus { GameIsOnGoing, FirstPlayerWins, SecondPlayerWins, Draw };
        public enum eBoardSize { Six = 6, Eight = 8, Ten = 10};
        public const int k_numOfInitialGamePieces = 12;
        private CheckersBoard m_gameBoard;
        private Player m_firstPlayer;
        private Player m_secondPlayer;
        private Player m_currentPlayer;
        private Player m_otherPlayer;
        private eGameStatus m_gameStastus = eGameStatus.GameIsOnGoing;
        private static readonly Random s_Random = new Random();

        public GameConfig(CheckersBoard i_gameBoard, Player i_firstPlayer, Player i_secondPlayer)
        {
            this.m_gameBoard = i_gameBoard;
            this.m_firstPlayer = i_firstPlayer;
            this.m_secondPlayer = i_secondPlayer;
        }

        public CheckersBoard Gameboard
        {
            get { return this.m_gameBoard; }
            set { this.m_gameBoard = value; }
        }

        public Player CurrentPlayer
        {
            get { return this.m_currentPlayer; }
        }

        public Player OtherPlayer
        {
            get { return this.m_otherPlayer; }
        }

        public eGameStatus GameStatus
        {
            get { return this.m_gameStastus; }
            set { this.m_gameStastus = value; }
        }

        public Player FirstPlayer
        {
            get { return this.m_firstPlayer; }
            set { this.m_firstPlayer = value; }
        }

        public Player SecondPlayer
        {
            get { return this.m_secondPlayer; }
            set { this.m_secondPlayer = value; }
        }

        public static bool IsValidBoardSize(int i_boardSize)
        {
            return (i_boardSize == (int)eBoardSize.Six || i_boardSize == (int)eBoardSize.Eight || i_boardSize == (int)eBoardSize.Ten); 
        }

        private void removeGamePiece(GamePiece i_gamePieceToRemove, Player i_playerWhoOwnsGamePiece)
        {
            Player.RemoveGamePieceFromSet(i_gamePieceToRemove, i_playerWhoOwnsGamePiece);
            this.m_gameBoard.RemoveGamePieceFromBoard(i_gamePieceToRemove);
        }

        private void checkIfKing(GamePiece i_lastPlayedGamePiece)
        {
            if (!(i_lastPlayedGamePiece.IsKing))
            {
                GamePiece.pieceSign signOfLastPlayedGamePiece = i_lastPlayedGamePiece.Sign;
                int currentPositionX = i_lastPlayedGamePiece.Point.X;

                if ((signOfLastPlayedGamePiece == GamePiece.pieceSign.X && currentPositionX == this.m_gameBoard.minBorder)
                    || (signOfLastPlayedGamePiece == GamePiece.pieceSign.O && currentPositionX == this.m_gameBoard.maxBorder))
                {
                    setKing(i_lastPlayedGamePiece);
                }
            }

        }

        private void setKing(GamePiece i_lastPlayedGamePiece)
        {
            i_lastPlayedGamePiece.Sign = i_lastPlayedGamePiece.Sign == GamePiece.pieceSign.X ?
                GamePiece.pieceSign.K : GamePiece.pieceSign.U;
            i_lastPlayedGamePiece.IsKing = true;
            this.m_currentPlayer.NumOfKings++;
        }

        private bool isAValidGamePieceToMove(GamePiece i_gamePieceToMove)
        {
            bool isPieceValid;

            if (this.m_currentPlayer.TypeOfPlayer == Player.ePlayerType.X)
            {
                isPieceValid = (i_gamePieceToMove.Sign == GamePiece.pieceSign.X) || (i_gamePieceToMove.Sign == GamePiece.pieceSign.K);
            }
            else
            {
                isPieceValid = (i_gamePieceToMove.Sign == GamePiece.pieceSign.O) || (i_gamePieceToMove.Sign == GamePiece.pieceSign.U);
            }

            return isPieceValid;
        }

        public bool IsMoveValid(Move i_move, out Move o_validatedMove)
        {
            bool isMoveValid = false;
            o_validatedMove = null;

            if(isPointInBoard(i_move.StartPoint) && isPointInBoard(i_move.EndPoint))
            {
                GamePiece gamePieceToMove = this.m_gameBoard.Board[i_move.StartPoint.X, i_move.StartPoint.Y];
                if(gamePieceToMove != null)
                {
                    if (isAValidGamePieceToMove(gamePieceToMove))
                    {
                        checkForPossibleMovesForGamePiece(gamePieceToMove);
                        foreach (Move move in gamePieceToMove.PossibleMoves)
                        {
                            if ((move.EndPoint.X == i_move.EndPoint.X && move.EndPoint.Y == i_move.EndPoint.Y) &&
                                (move.StartPoint.X == i_move.StartPoint.X && move.StartPoint.Y == i_move.StartPoint.Y))
                            {
                                o_validatedMove = move;
                                isMoveValid = true;
                                break;
                            }
                        }

                        if (isMoveValid)
                        {
                            checkForPossibleMovesForPlayer(this.m_currentPlayer);
                            this.m_currentPlayer.UpdatePossibleMoves();
                            bool canSkip = canPlayerSkip(this.m_currentPlayer);
                            if (canSkip)
                            {
                                isMoveValid = o_validatedMove.IsSkippingMove;
                            }

                        }

                    }

                }
            }

            return isMoveValid;
        }

        public void MakeAMove(Move i_move, out Move o_possibleFollowingSkippingMove)
        {
            GamePiece currentGamePiece;
            o_possibleFollowingSkippingMove = null;

            this.m_gameBoard.MovePieceOnBoard(i_move.GamePieceToMove, i_move.EndPoint);
            if (i_move.IsSkippingMove)
            {
                removeGamePiece(i_move.CapturedGamePiece, this.m_otherPlayer);
            }

            i_move.GamePieceToMove.PossibleMoves.Clear();
            this.m_currentPlayer.ClearPossiblemoves();
            currentGamePiece = this.Gameboard.Board[i_move.EndPoint.X, i_move.EndPoint.Y];
            checkIfKing(currentGamePiece);
            if (i_move.IsSkippingMove && isSkippingPossibleForGamePiece(currentGamePiece))
            {
                o_possibleFollowingSkippingMove = currentGamePiece.PossibleMoves[0];
            }
            else
            {
                CheckGameStatus();
                if(this.m_gameStastus == eGameStatus.GameIsOnGoing)
                {
                    switchTurns();
                }
           
            }

        }

        private static void chooseARandomMove(List<Move> moves, out Move o_currentMove)
        {
            int moveNumber;

            moveNumber = s_Random.Next(moves.Count);
            o_currentMove = moves[moveNumber];
        }

        private bool checkForPossibleMovesForGamePiece(GamePiece i_currentGamePiece)
        {
            i_currentGamePiece.PossibleMoves.Clear();
            bool isSkippingPossible = false;

            isSkippingPossible = isSkippingPossibleForGamePiece(i_currentGamePiece);
            if (isSkippingPossible)
            {
                return true;
            }

            bool isThereAPossibleMove = false;

            isThereAPossibleMove = checkForStandardMovesForGamePiece(i_currentGamePiece);

            return isThereAPossibleMove;
        }

        private bool isSkippingPossibleForGamePiece(GamePiece i_currentGamePiece)
        {
            bool canSkip = false;
            GamePiece.pieceSign currentSign = i_currentGamePiece.Sign;
            CheckersBoard.BoardPoint currentPosition = i_currentGamePiece.Point;
            List<List<CheckersBoard.BoardPoint>> possibleSkips = new List<List<CheckersBoard.BoardPoint>>();

            switch (currentSign)
            {
                case GamePiece.pieceSign.X:
                    findPossibleSkipsForX(currentPosition, possibleSkips);
                    break;
                case GamePiece.pieceSign.O:
                    findPossibleSkipsForO(currentPosition, possibleSkips);
                    break;
                case GamePiece.pieceSign.K:
                case GamePiece.pieceSign.U:
                    findPossibleSkipsForX(currentPosition, possibleSkips);
                    findPossibleSkipsForO(currentPosition, possibleSkips);
                    break;
            }

            foreach (List<CheckersBoard.BoardPoint> possibleSkip in possibleSkips)
            {
                CheckersBoard.BoardPoint destination = possibleSkip[0];
                CheckersBoard.BoardPoint possibleEnemyPos = possibleSkip[1];

                if ((isPointInBoard(destination) && isPointEmpty(destination)) && checkIfEnemyExists(currentSign, possibleEnemyPos))
                {
                    canSkip = true;
                    GamePiece capturedGamePiece = this.m_gameBoard.Board[possibleEnemyPos.X, possibleEnemyPos.Y];
                    Move move = new Move(currentPosition, destination, i_currentGamePiece, canSkip, capturedGamePiece);
                    i_currentGamePiece.UpdateListOfPossibleMoves(move);
                }
            }

            return canSkip;
        }

        private static void findPossibleSkipsForXOrO(CheckersBoard.BoardPoint i_currentPosition, List<List<CheckersBoard.BoardPoint>> i_possibleSkips, int xDirection, int yDirection)
        {
            CheckersBoard.BoardPoint destination;
            CheckersBoard.BoardPoint enemyPos;

            // skip to the right
            destination = new CheckersBoard.BoardPoint(i_currentPosition.X + xDirection * 2, i_currentPosition.Y + yDirection * 2);
            enemyPos = new CheckersBoard.BoardPoint(i_currentPosition.X + xDirection, i_currentPosition.Y + yDirection);
            List<CheckersBoard.BoardPoint> possibleSkipToTheRight = new List<CheckersBoard.BoardPoint> { destination, enemyPos };
            i_possibleSkips.Add(possibleSkipToTheRight);

            // skip to the left
            destination = new CheckersBoard.BoardPoint(i_currentPosition.X + xDirection * 2, i_currentPosition.Y - yDirection * 2);
            enemyPos = new CheckersBoard.BoardPoint(i_currentPosition.X + xDirection, i_currentPosition.Y - yDirection);
            List<CheckersBoard.BoardPoint> possibleSkipToTheLeft = new List<CheckersBoard.BoardPoint> { destination, enemyPos };
            i_possibleSkips.Add(possibleSkipToTheLeft);
        }

        private static void findPossibleSkipsForX(CheckersBoard.BoardPoint i_currentPosition, List<List<CheckersBoard.BoardPoint>> i_possibleSkips)
        {
            findPossibleSkipsForXOrO(i_currentPosition, i_possibleSkips, -1, 1);
        }

        private static void findPossibleSkipsForO(CheckersBoard.BoardPoint i_currentPosition, List<List<CheckersBoard.BoardPoint>> i_possibleSkips)
        {
            findPossibleSkipsForXOrO(i_currentPosition, i_possibleSkips, 1, 1);
        }

        private bool isPointInBoard(CheckersBoard.BoardPoint i_point)
        {
            bool isPointInBoard = true;

            if (i_point.X < this.m_gameBoard.minBorder || i_point.X > this.m_gameBoard.maxBorder)
            {
                isPointInBoard = false;
            }
            else if (i_point.Y < this.m_gameBoard.minBorder || i_point.Y > this.m_gameBoard.maxBorder)
            {
                isPointInBoard = false;
            }

            return isPointInBoard;
        }

        private bool checkIfEnemyExists(GamePiece.pieceSign i_currentSign, CheckersBoard.BoardPoint i_point)
        {
            bool isEnemy = false;
            GamePiece possibleEnemy = this.m_gameBoard.Board[i_point.X, i_point.Y];

            if (possibleEnemy != null)
            {
                if (i_currentSign == GamePiece.pieceSign.X || i_currentSign == GamePiece.pieceSign.K)
                {
                    isEnemy = (possibleEnemy.Sign == GamePiece.pieceSign.O) || (possibleEnemy.Sign == GamePiece.pieceSign.U);
                }
                else if (i_currentSign == GamePiece.pieceSign.O || i_currentSign == GamePiece.pieceSign.U)
                {
                    isEnemy = (possibleEnemy.Sign == GamePiece.pieceSign.X) || (possibleEnemy.Sign == GamePiece.pieceSign.K);
                }
            }

            return isEnemy;
        }

        private bool isPointEmpty(CheckersBoard.BoardPoint i_point)
        {
            return this.m_gameBoard.Board[i_point.X, i_point.Y] == null;
        }

        private static void findPossibleStandardMovesForXOrO(CheckersBoard.BoardPoint i_currentPosition, List<CheckersBoard.BoardPoint> i_possibleDestinations, int xDirection, int yDirection)
        {
            CheckersBoard.BoardPoint destinationA;
            CheckersBoard.BoardPoint destinationB;

            destinationA = new CheckersBoard.BoardPoint(i_currentPosition.X + xDirection, i_currentPosition.Y + yDirection);
            i_possibleDestinations.Add(destinationA);
            destinationB = new CheckersBoard.BoardPoint(i_currentPosition.X + xDirection, i_currentPosition.Y - yDirection);
            i_possibleDestinations.Add(destinationB);
        }

        private static void findPossibleStandardMovesForX(CheckersBoard.BoardPoint i_currentPosition, List<CheckersBoard.BoardPoint> i_possibleDestinations)
        {
            findPossibleStandardMovesForXOrO(i_currentPosition, i_possibleDestinations, -1, 1);
        }

        private static void findPossibleStandardMovesForO(CheckersBoard.BoardPoint i_currentPosition, List<CheckersBoard.BoardPoint> i_possibleDestinations)
        {
            findPossibleStandardMovesForXOrO(i_currentPosition, i_possibleDestinations, 1, 1);
        }


        private bool checkForStandardMovesForGamePiece(GamePiece i_currentGamePiece)
        {
            bool canMove = false;
            GamePiece.pieceSign currentSign = i_currentGamePiece.Sign;
            CheckersBoard.BoardPoint currentPosition = i_currentGamePiece.Point;
            List<CheckersBoard.BoardPoint> possibleDestinations = new List<CheckersBoard.BoardPoint>();

            switch (currentSign)
            {
                case GamePiece.pieceSign.X:
                    findPossibleStandardMovesForX(currentPosition, possibleDestinations);
                    break;
                case GamePiece.pieceSign.O:
                    findPossibleStandardMovesForO(currentPosition, possibleDestinations);
                    break;
                case GamePiece.pieceSign.K:
                case GamePiece.pieceSign.U:
                    findPossibleStandardMovesForX(currentPosition, possibleDestinations);
                    findPossibleStandardMovesForO(currentPosition, possibleDestinations);
                    break;
            }

            foreach (CheckersBoard.BoardPoint destination in possibleDestinations)
            {
                if (isPointInBoard(destination) && isPointEmpty(destination))
                {
                    canMove = true;
                    bool isSkippingMove = false;
                    Move move = new Move(currentPosition, destination, i_currentGamePiece, isSkippingMove);
                    i_currentGamePiece.UpdateListOfPossibleMoves(move);
                }
            }

            return canMove;
        }

        private void checkForPossibleMovesForPlayer(Player i_currentPlayer)
        {
            foreach (GamePiece gamePiece in i_currentPlayer.SetOfGamePieces)
            {
                checkForPossibleMovesForGamePiece(gamePiece);
            }
        }

        private static bool canPlayerSkip(Player i_currentPlayer)
        {
            bool canSkip = false;

            foreach (Move move in i_currentPlayer.PossibleMoves)
            {
                if (move.IsSkippingMove)
                {
                    canSkip = true;
                    break;
                }

            }

            return canSkip;
        }

        public void GetComputersMove(out Move o_computersMove)
        {
            List<Move> possibleMoves;
            List<Move> possibleSkippingMoves;
            List<Move> moves;

            checkForPossibleMovesForPlayer(this.m_currentPlayer);
            possibleMoves = this.m_currentPlayer.UpdatePossibleMoves();
            possibleSkippingMoves = this.m_currentPlayer.UpdatePossibleSkippingMoves();
            moves = possibleSkippingMoves.Count > 0 ? possibleSkippingMoves : possibleMoves;
            chooseARandomMove(moves, out o_computersMove);
            this.m_currentPlayer.ClearPossibleSkippingmoves();
        }

        public void SetPlayers()
        {
            this.m_currentPlayer = this.m_firstPlayer;
            this.m_otherPlayer = this.m_secondPlayer;
        }

        private void switchTurns()
        {
            Player temporaryPlayer = this.m_currentPlayer;
            this.m_currentPlayer = this.m_otherPlayer;
            this.m_otherPlayer = temporaryPlayer;
        }

        public void ClearPlayersMovesForNextTurn()
        {
            this.m_currentPlayer.ClearPossiblemoves();
            this.m_otherPlayer.ClearPossiblemoves();

        }

        public void CheckGameStatus()
        {

            int firstPlayerNumOfGamePieces = this.m_firstPlayer.NumOfGamePieces;
            int secondPlayerNumOfGamePieces = this.m_secondPlayer.NumOfGamePieces;

            if (firstPlayerNumOfGamePieces == 0)
            {
                this.m_gameStastus = eGameStatus.SecondPlayerWins;
            }
            else if (secondPlayerNumOfGamePieces == 0)
            {
                this.m_gameStastus = eGameStatus.FirstPlayerWins;
            }
            else
            {
                bool firstPlayerHasMoves = false;
                bool secondPlayerHasMoves = false;

                foreach (GamePiece gamePiece in m_firstPlayer.SetOfGamePieces)
                {
                    if (checkForPossibleMovesForGamePiece(gamePiece))
                    {
                        firstPlayerHasMoves = true;
                        break;
                    }
                }

                foreach (GamePiece gamePiece in m_secondPlayer.SetOfGamePieces)
                {
                    if (checkForPossibleMovesForGamePiece(gamePiece))
                    {
                        secondPlayerHasMoves = true;
                        break;
                    }
                }

                if (!firstPlayerHasMoves && !secondPlayerHasMoves)
                {
                    this.m_gameStastus = eGameStatus.Draw;
                }
                else if (!firstPlayerHasMoves)
                {
                    this.m_gameStastus = eGameStatus.SecondPlayerWins;
                }
                else if (!secondPlayerHasMoves)
                {
                    this.m_gameStastus = eGameStatus.FirstPlayerWins;
                }
            }

        }

        public void UpdateGamescore()
        {
            Player winner;
            Player loser;
            int winnersNumOfGamePiecesLeft;
            int losersNumOfGamePiecesLeft;
            int winnersNumOfKings;
            int losersNumOfKings;

            winner = this.m_gameStastus == eGameStatus.FirstPlayerWins ? this.m_firstPlayer : this.m_secondPlayer;
            loser = this.m_gameStastus == eGameStatus.FirstPlayerWins ? this.m_secondPlayer : this.m_firstPlayer;
            winnersNumOfKings = winner.NumOfKings;
            losersNumOfKings = loser.NumOfKings;
            winnersNumOfGamePiecesLeft = (winner.NumOfGamePieces - winnersNumOfKings) + winnersNumOfKings * 4;
            losersNumOfGamePiecesLeft = (loser.NumOfGamePieces - losersNumOfKings) + losersNumOfKings * 4;

            winner.Gamescore += Math.Abs(winnersNumOfGamePiecesLeft - losersNumOfGamePiecesLeft);
        }

    }
}



