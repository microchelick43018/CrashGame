using System;
using System.Collections.Generic;
using System.Threading;

namespace CrashGameSimulation
{
    class Program
    {
        private static double numberOfGames = 10E6;
        private static readonly object lockObject = new object();
        private static List<Result> results = new List<Result>();

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            const int ThreadsCount = 10;
            Thread[] threads = new Thread[ThreadsCount];

            for (int i = 0; i < ThreadsCount; i++)
            {
                double startMultiplier = 1.01 + i * 108.0 / ThreadsCount;
                threads[i] = new Thread(() => SimulateGames(startMultiplier));
                threads[i].Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            // Вывод результатов в порядке возрастания множителя
            results.Sort((a, b) => a.PlayerMultiplier.CompareTo(b.PlayerMultiplier));
            foreach (var result in results)
            {
                Console.WriteLine(result.Message);
            }

            Console.ReadKey();
        }

        private static void SimulateGames(double startMultiplier)
        {
            Random random = new Random();

            for (double playerMultiplier = startMultiplier; playerMultiplier < startMultiplier + 10.8; playerMultiplier += 0.25)
            {
                double totalBet = 0;
                double totalWin = 0;

                for (int i = 0; i < numberOfGames; i++)
                {
                    double bet = 1;
                    totalBet += bet;

                    double crashPoint = GetCrashPoint(random);

                    if (playerMultiplier <= crashPoint)
                    {
                        double win = bet * playerMultiplier;
                        totalWin += win;
                    }
                }

                double rtp = (totalWin / totalBet) * 100;

                lock (lockObject)
                {
                    results.Add(new Result
                    {
                        PlayerMultiplier = playerMultiplier,
                        Message = $"После {numberOfGames} игр с выбором игрока {playerMultiplier}x  RTP составил: {rtp:N4}%"
                    });
                }
            }
        }

        private static double GetCrashPoint(Random random)
        {
            double rand = random.NextDouble() * (1 - (1.0 / 110)) + (1.0 / 110);
            double crashPoint = Math.Pow();
            return Math.Floor(crashPoint * 100) / 100;
        }

        private static double GetIntervalCoefficient(double crashPoint)
        {

            if (crashPoint >= 1.01 && crashPoint < 1.05) return 0.92;
            if (crashPoint >= 1.05 && crashPoint < 1.1) return 0.93;
            if (crashPoint >= 1.1 && crashPoint < 1.25) return 0.9;
            if (crashPoint >= 1.25 && crashPoint < 1.5) return 0.91;
            if (crashPoint >= 1.5 && crashPoint < 2) return 0.94;
            if (crashPoint >= 2 && crashPoint < 3) return 0.98;
            if (crashPoint >= 3 && crashPoint < 5) return 0.96;
            if (crashPoint >= 5 && crashPoint < 10) return 0.9;
            if (crashPoint >= 10 && crashPoint <= 108) return 1;

            throw new ArgumentException("Invalid player multiplier value.");
        }
    }

    class Result
    {
        public double PlayerMultiplier { get; set; }
        public string Message { get; set; }
    }
}
