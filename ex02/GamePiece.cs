using System.Collections.Generic;

namespace ex02
{
    internal class GamePiece
    {
        public enum pieceSign { X = 'X', O = 'O', U = 'U', K = 'K' };
        private CheckersBoard.BoardPoint m_point;
        private pieceSign m_sign;
        private bool m_isKing;
        List<Move> m_PossibleMoves;

        public GamePiece(CheckersBoard.BoardPoint i_point, pieceSign i_sign)
        {
            this.m_point = i_point;
            this.m_sign = i_sign;
            this.m_PossibleMoves = new List<Move>();
        }

        public pieceSign Sign
        {
            get { return this.m_sign; }
            set {  this.m_sign = value; }
        }

        public bool IsKing
        {
            get { return this.m_isKing; }
            set { this.m_isKing = value; }
        }

        public CheckersBoard.BoardPoint Point
        {
            get { return this.m_point; }
            set { this.m_point = value; }
        }

        public List<Move> PossibleMoves
        {
            get { return this.m_PossibleMoves; }
        }

        public void UpdateListOfPossibleMoves(Move i_possibleMove)
        {
            this.m_PossibleMoves.Add(i_possibleMove);
        }
    }
}
