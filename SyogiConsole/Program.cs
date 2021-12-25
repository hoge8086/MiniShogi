using System;

namespace SyogiConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //try
            //{
            //    GameType gameType = GameType.AnimalShogi;
            //    if(args.Length > 0)
            //    {
            //        if (args[0] == "animal")
            //        {
            //            gameType = GameType.AnimalShogi;
            //        }
            //        else if (args[0] == "55")
            //        {
            //            gameType = GameType.FiveFiveShogi;
            //        }
            //    }

            //    var factory = new GameFactory();
            //    var game = factory.Create(gameType);

            //    for (; ; )
            //    {
            //        Console.WriteLine(game.ToString());

            //        if (game.State.IsEnd)
            //        {
            //            Console.WriteLine("--------------");
            //            Console.WriteLine("勝者:" + game.State.GameResult.Winner.ToString());
            //            break;
            //        }

            //        var moves = game.CreateAvailableMoveCommand();
            //        for (int i = 0; i < moves.Count; i++)
            //        {
            //            Console.WriteLine(i.ToString() + ":" + moves[i].FindFromKoma(game.State).KomaType.Id + ":" + moves[i].ToString());
            //        }
            //        Console.Write(">");
            //        var cmd = Console.ReadLine();
            //        int val = int.Parse(cmd);
            //        game.Play(moves[val]);
            //    }
            //}
            //catch(Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}
        }
    }
}
