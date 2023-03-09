namespace Othelo
{
    public enum Stone
    {
        None,
        Black, 
        White,
    }

    public class GameLogic
    {
        private const int _maxRow = 8;
        private const int _maxColumn = 8;

        // 先行：黒、後攻：白
        private Stone? _nextPlayer;

        // ボードの現在の状態
        private Stone?[,]? _currentBoard;

        private Stone?[,] MakeInitialBoard()
        {
            return new Stone?[,]
            {
                {Stone.White, Stone.White, Stone.White, Stone.None, Stone.None, Stone.None, Stone.None, Stone.None, },
                {Stone.White, Stone.White, Stone.White, Stone.None, Stone.None, Stone.None, Stone.None, Stone.None, },
                {Stone.White, Stone.White, Stone.White, Stone.None, Stone.None, Stone.None, Stone.None, Stone.None, },
                {Stone.White, Stone.White, Stone.White, Stone.Black, Stone.White, Stone.None, Stone.None, Stone.None, },
                {Stone.White, Stone.White, Stone.White, Stone.White, Stone.Black, Stone.None, Stone.None, Stone.None, },
                {Stone.White, Stone.White, Stone.White, Stone.None, Stone.None, Stone.None, Stone.None, Stone.None, },
                {Stone.White, Stone.White, Stone.White, Stone.None, Stone.None, Stone.None, Stone.None, Stone.None, },
                {Stone.White, Stone.White, Stone.White, Stone.None, Stone.None, Stone.None, Stone.None, Stone.None, },
            };
            //return new Stone?[,]
            //{
            //    {Stone.None, Stone.None, Stone.None, Stone.None, Stone.None, Stone.None, Stone.None, Stone.None, },
            //    {Stone.None, Stone.None, Stone.None, Stone.None, Stone.None, Stone.None, Stone.None, Stone.None, },
            //    {Stone.None, Stone.None, Stone.None, Stone.None, Stone.None, Stone.None, Stone.None, Stone.None, },
            //    {Stone.None, Stone.None, Stone.None, Stone.Black, Stone.White, Stone.None, Stone.None, Stone.None, },
            //    {Stone.None, Stone.None, Stone.None, Stone.White, Stone.Black, Stone.None, Stone.None, Stone.None, },
            //    {Stone.None, Stone.None, Stone.None, Stone.None, Stone.None, Stone.None, Stone.None, Stone.None, },
            //    {Stone.None, Stone.None, Stone.None, Stone.None, Stone.None, Stone.None, Stone.None, Stone.None, },
            //    {Stone.None, Stone.None, Stone.None, Stone.None, Stone.None, Stone.None, Stone.None, Stone.None, },
            //};
        }
        
        public GameLogic()
        {
            _nextPlayer = Stone.Black;
            _currentBoard = MakeInitialBoard();
        }

        public Stone?[,] GetCurrentBoard()
        {
            if (_currentBoard is null)
                throw new InvalidDataException("盤面が空です");

            return _currentBoard;
        }

        public Stone GetNextPlayer()
        {
            return _nextPlayer ?? throw new InvalidDataException("次のプレイヤーが空です"); ;
        }

        public void StartGame()
        {
            _nextPlayer = Stone.Black;
            _currentBoard = MakeInitialBoard();
        }

        public void DiscardGame()
        {

        }

        public bool GetCanPut(int row, int column)
        {
            if (_currentBoard is null)
                throw new InvalidDataException("盤面が空です");

            if (_currentBoard[row, column] != Stone.None)
                return false;

            return GetTurnOverrableCoordinates(row,column).Count() > 0;
        }

        // 石を置く
        public void Put(int row, int column)
        {
            if (_currentBoard is null || _nextPlayer is null)
                throw new InvalidOperationException("不正なデータ");

            // ひっくり返せる石をひっくり返す
            var turnover = GetTurnOverrableCoordinates(row, column);
            foreach (var pos in turnover)
            {
                _currentBoard[pos.Row, pos.Column] = _nextPlayer;
            }

            // 石を置く
            _currentBoard[row, column] = _nextPlayer;
        }

        public void PlayerChange()
        {
            // ひっくり返す
            _nextPlayer = (_nextPlayer == Stone.Black) ? Stone.White : Stone.Black;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row">次のプレイヤーが石を置く場所(x)</param>
        /// <param name="column">次のプレイヤーが石を置く場所(y)</param>
        /// <returns>
        /// 引数の場所に置いたときに、ひっくり返せる石のリスト
        /// </returns>
        /// <exception cref="InvalidDataException"></exception>
        public HashSet<(int Row, int Column)> GetTurnOverrableCoordinates(int row, int column)
        {
            if (_currentBoard is null)
                throw new InvalidDataException("盤面が空です");

            if (_nextPlayer is null)
                throw new InvalidDataException("次のプレイヤーが空です");

            if (_currentBoard[row, column] != Stone.None)
                throw new InvalidDataException("その位置はすでに石が置かれています");

            var turnOverable = new HashSet<(int, int)>();

            turnOverable = turnOverable.Concat(searchTurnOverrableCoordinates(row, column, true, null)).ToHashSet();//下
            turnOverable = turnOverable.Concat(searchTurnOverrableCoordinates(row, column, false, null)).ToHashSet();//上
            turnOverable = turnOverable.Concat(searchTurnOverrableCoordinates(row, column, null, true)).ToHashSet();//右
            turnOverable = turnOverable.Concat(searchTurnOverrableCoordinates(row, column, null, false)).ToHashSet();//左
            turnOverable = turnOverable.Concat(searchTurnOverrableCoordinates(row, column, true, true)).ToHashSet();//右下
            turnOverable = turnOverable.Concat(searchTurnOverrableCoordinates(row, column, false, false)).ToHashSet();//左上
            turnOverable = turnOverable.Concat(searchTurnOverrableCoordinates(row, column, true, false)).ToHashSet();//左下
            turnOverable = turnOverable.Concat(searchTurnOverrableCoordinates(row, column, false, true)).ToHashSet();//右上

            return turnOverable;
        }

        // 
        private HashSet<(int Row, int Column)> searchTurnOverrableCoordinates(int row, int column, bool? directionRow, bool? directionColumn)
        {
            var turnOverable = new HashSet<(int, int)>();

            if (_currentBoard is null)
                throw new InvalidDataException("盤面が空です");

            if (_nextPlayer is null)
                throw new InvalidDataException("次のプレイヤーが空です");

            if ((directionRow is null) && (directionColumn is null))
                throw new InvalidOperationException("検索方向を指定してください。");


            // 右方向に検索
            int firstRound = 1;
            for (int i = firstRound; i < _maxRow; i++)
            {
                var difrow = directionRow == null ? 0 : (i * (directionRow == true ? 1 : -1));
                var difcolmn = directionColumn == null ? 0 : (i * (directionColumn == true ? 1 : -1));

                if (
                    ((directionRow == true) && (row + difrow >= _maxRow))
                    || ((directionRow == false) && (row + difrow <= 0))
                    || ((directionColumn == true) && (column + difcolmn >= _maxColumn))
                    || ((directionColumn == false) && (column + difcolmn <= 0))
                    )
                {
                    turnOverable.Clear();
                    break;//端まで検索したら終了
                }

                if (_currentBoard[row + difrow, column + difcolmn] == Stone.None)
                {
                    turnOverable.Clear();
                    break;// 隣に石がない
                }

                if (_currentBoard[row + difrow, column + difcolmn] == _nextPlayer)
                {
                    // 自分と同じ色の場合

                    if (i == firstRound)
                    {
                        // 一個となりが自分と同じ色→ひっくり返せるものはない
                        break;
                    }
                    else
                    {
                        // 二週目以降→ひっくり返せる石確定
                        break;
                    }
                }
                else
                {
                    // 自分の色と同じでない場合→ひっくり返せるかも候補を保存
                    turnOverable.Add((row + difrow, column + difcolmn));
                }
            }
            return turnOverable;
        }
    }




}