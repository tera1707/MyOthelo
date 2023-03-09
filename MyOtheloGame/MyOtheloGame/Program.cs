using Othelo;

namespace MyOtheloGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var logic = new GameLogic();

            var cb = logic.GetCurrentBoard();

            while (true)
            {
                Console.Clear();


                Console.Write("  ");
                for (var i = 0; i < cb.GetLength(0); i++)
                {
                    Console.Write($"{i} ");
                }
                Console.WriteLine($"");

                // 今の盤面を表示
                for (var i = 0; i < cb.GetLength(0); i++)
                {
                    Console.Write($"{i} ");
                    
                    for (var j = 0; j < cb.GetLength(1); j++)
                    {
                        var stone = cb[i, j] switch
                        {
                            Stone.Black => "〇",
                            Stone.White => "●",
                            _ => "－",
                        };
                        Console.Write(stone);
                    }
                    Console.WriteLine("");
                }

                // 次の人表示
                var next = logic.GetNextPlayer() == Stone.White ? "●" : "〇";
                Console.WriteLine($"次の人：{next}");

                // 入力を促す
                Console.WriteLine("石を置く場所を入力してください。");
                Console.WriteLine("x ?：");
                var x = Console.ReadLine();
                Console.WriteLine("y ?：");
                var y = Console.ReadLine();

                var column = 0;
                var row = 0;
                try
                {
                    column = int.Parse(x);
                    row = int.Parse(y);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("数字を入寮してください。");
                    continue;
                }


                if (!logic.GetCanPut(row, column))
                {
                    Console.WriteLine("そこはおけません。");
                    Thread.Sleep(1000);
                }
                else
                {
                    logic.Put(row, column);
                    logic.PlayerChange();
                }

            }














            Console.ReadLine();
        }
    }
}