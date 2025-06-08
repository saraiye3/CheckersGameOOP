using System;

namespace ex02
{
    internal class CheckersBoard
    {
        private GamePiece?[,] m_board;
        private int m_boardSize;

        public struct BoardPoint
        {
            public int X { get; set; }
            public int Y { get; set; }

            public BoardPoint(int i_x, int i_y)
            {
                X = i_x;
                Y = i_y;
            }
        }

        public GamePiece?[,] Board
        {
            get { return this.m_board; }
        }

        public int Size
        {
            get { return this.m_boardSize; }
        }

        public int minBorder
        {
            get { return 0; }
        }

        public int maxBorder
        {
            get { return this.m_boardSize - 1; }
        }

        public CheckersBoard(int i_size)
        {
            this.m_boardSize = i_size;
            this.m_board = new GamePiece?[i_size, i_size];
            this.InitializeBoard();
        }

        public void InitializeBoard()
        {
            int middleBoard = (int)Math.Ceiling(this.m_boardSize / 3.0);
            for(int i = 0; i < this.m_boardSize; ++i)
            {
                if(i == middleBoard || i == middleBoard + 1)
                {
                    continue;
                }

                for(int j = 0; j < this.m_boardSize; ++j)
                {
                    if ((i % 2 == 0 && j % 2 != 0) || (i % 2 != 0 && j % 2 == 0))
                    {
                        if (i > middleBoard)
                        {
                            this.m_board[i, j] = new GamePiece(new CheckersBoard.BoardPoint(i, j), GamePiece.pieceSign.X);
                        }
                        else
                        {
                            this.m_board[i, j] = new GamePiece(new CheckersBoard.BoardPoint(i, j), GamePiece.pieceSign.O);
                        }
                    }
                }
            }
        }

        public void RemoveGamePieceFromBoard(GamePiece i_gamePieceToRemove)
        {
            int x = i_gamePieceToRemove.Point.X;
            int y = i_gamePieceToRemove.Point.Y;
            this.m_board[x, y] = null;
        }

        public void MovePieceOnBoard(GamePiece i_gamePieceToMove, CheckersBoard.BoardPoint i_destination)
        {
            int currentX = i_gamePieceToMove.Point.X;
            int currentY = i_gamePieceToMove.Point.Y;
      
            this.m_board[currentX, currentY] = null;
            i_gamePieceToMove.Point = i_destination;
            this.m_board[i_destination.X, i_destination.Y] = i_gamePieceToMove;
        }
    }

}

     

