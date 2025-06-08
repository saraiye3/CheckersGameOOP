using System.Collections.Generic;

namespace ex02
{
    internal class Player
    {
        public enum ePlayerType { X = 'X', O = 'O' };
        private readonly string r_playerName;
        private readonly ePlayerType r_typeOfPlayer;
        private readonly bool r_isHumanPlayer;
        private List<GamePiece> m_setOfGamePieces;
        private int m_numOfGamePieces;
        private int m_numOfKings;
        private List<Move> m_possibleMoves;
        private List<Move> m_possibleSkippingMoves;
        private int m_gameScore;

        public Player(ePlayerType i_typeOfPlayer, string i_playerName, bool i_isPlayerHuman, int i_numOfGamePieces, GamePiece?[,] i_board, int i_boardSize)
        {
            this.r_typeOfPlayer = i_typeOfPlayer;
            this.r_playerName = i_playerName;
            this.r_isHumanPlayer = i_isPlayerHuman;
            this.m_numOfGamePieces = i_numOfGamePieces;
            this.m_setOfGamePieces = new List<GamePiece>(this.m_numOfGamePieces);
            InitializeSetOfGamePieces(i_board, i_boardSize);
            this.m_possibleMoves = new List<Move>();
            this.m_possibleSkippingMoves = new List<Move>();
        }

        public List<GamePiece> SetOfGamePieces
        {
            get { return this.m_setOfGamePieces; }
        }

        public ePlayerType TypeOfPlayer
        {
            get { return this.r_typeOfPlayer; }
        }

        public int Gamescore
        {
            get { return this.m_gameScore; }
            set { this.m_gameScore = value; }
        }

        public bool IsHumanPlayer
        {
            get { return this.r_isHumanPlayer; }
        }

        public List<Move> PossibleMoves
        {
            get { return this.m_possibleMoves; }
        }

        public List<Move> PossibleSkippingMoves
        {
            get { return this.m_possibleSkippingMoves; }
        }

        public string Name
        {
            get { return this.r_playerName; }
        }

        public int NumOfGamePieces
        {
            get { return this.m_numOfGamePieces; }
            set { this.m_numOfGamePieces = value; }
        }

        public int NumOfKings
        {
            get { return this.m_numOfKings; }
            set { this.m_numOfKings = value; }
        }

        public void InitializeSetOfGamePieces(GamePiece?[,] i_board, int i_boardSize)
        {
            this.m_setOfGamePieces.Clear();
            for (int i = 0; i < i_boardSize; ++i)
            {
                for (int j = 0; j < i_boardSize; ++j)
                {
                    if (i_board[i, j] != null && i_board[i, j].Sign == (GamePiece.pieceSign)this.r_typeOfPlayer)
                    {
                        this.m_setOfGamePieces.Add(i_board[i, j]);
                    }
                }
            }
        }

        public static void RemoveGamePieceFromSet(GamePiece i_gamePieceToRemove, Player i_playerWhoOwnsGamePiece)
        {
            i_playerWhoOwnsGamePiece.SetOfGamePieces.Remove(i_gamePieceToRemove);
            i_playerWhoOwnsGamePiece.NumOfGamePieces--;
            if(i_gamePieceToRemove.IsKing)
            {
                i_playerWhoOwnsGamePiece.NumOfKings--;
            }
        }

        public List<Move> UpdatePossibleMoves()
        {
            foreach (GamePiece gamePiece in this.SetOfGamePieces)
            {
                foreach (Move move in gamePiece.PossibleMoves)
                {
                    this.m_possibleMoves.Add(move);
                }
            }

            return this.m_possibleMoves;
        }

        public void ClearPossiblemoves()
        {
            this.m_possibleMoves.Clear();
        }

        public List<Move> UpdatePossibleSkippingMoves()
        {
            foreach (Move move in this.m_possibleMoves)
            {
                if (move.IsSkippingMove)
                {
                    this.m_possibleSkippingMoves.Add(move);
                }
            }

            return this.m_possibleSkippingMoves;
        }

        public void ClearPossibleSkippingmoves()
        {
            this.m_possibleSkippingMoves.Clear();
        }
    }
}