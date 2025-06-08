namespace ex02
{
    internal class Move
    {
        public GamePiece? GamePieceToMove { get; }
        public CheckersBoard.BoardPoint StartPoint { get; }
        public CheckersBoard.BoardPoint EndPoint { get; }
        public bool IsSkippingMove { get; }
        public GamePiece? CapturedGamePiece { get; }

        public Move(CheckersBoard.BoardPoint i_startPoint, CheckersBoard.BoardPoint i_endPoint, GamePiece i_gamePiece = null, bool i_isSkippingMove = false, GamePiece? i_capturedPiecePoint = null)
        {
            StartPoint = i_startPoint;
            EndPoint = i_endPoint;
            GamePieceToMove = i_gamePiece;
            IsSkippingMove = i_isSkippingMove;
            CapturedGamePiece = i_capturedPiecePoint;
        }
    }
}
