using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GTA_5_RP_Bot
{
    public partial class Helper
    {
        private static readonly Random Random = new();

        public static bool IsValidJson(string _)
        {
            if (string.IsNullOrWhiteSpace(_))
            {
                return false;
            }

            _ = _.Trim();

            if ((_.StartsWith("{") && _.EndsWith("}")) ||
                (_.StartsWith("[") && _.EndsWith("]")))
            {
                try
                {
                    JToken.Parse(_);

                    return true;
                }
                catch (JsonReaderException)
                {
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return false;
        }

        public static (int Index, ConsoleKey Key) Table(int Index, string Start, List<string> Selection, int Position, params ConsoleKey[] Skip)
        {
            Console.CursorVisible = false;

            const byte O = 1;
            const byte L = 8;

            ConsoleKey? Key = null;

            do
            {
                for (int i = 0; i < Selection.Count; i++)
                {
                    Console.SetCursorPosition(L, Position + (i / O));

                    if (i == Index)
                    {
                        if (string.IsNullOrEmpty(Selection[i]))
                        {
                            switch (Key)
                            {
                                case ConsoleKey.UpArrow:
                                    Index -= O;
                                    i -= 2;

                                    break;

                                case ConsoleKey.DownArrow:
                                    Index += O;

                                    break;
                            }
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Magenta;

                            Console.Write($"{Start} {Selection[i]}");
                        }
                    }
                    else
                    {
                        Console.Write(string.Empty.PadLeft(Start.Length + 1) + Selection[i]);
                    }

                    Console.ResetColor();
                };

                var Read = Console.ReadKey(true);

                switch (Read.Key)
                {
                    case ConsoleKey.UpArrow:
                        Key = ConsoleKey.UpArrow;

                        if (Index < O)
                        {
                            Index += Selection.Count - 1;
                        }
                        else
                        {
                            Index -= O;
                        }

                        break;

                    case ConsoleKey.DownArrow:
                        Key = ConsoleKey.DownArrow;

                        if (Index + O < Selection.Count)
                        {
                            Index += O;
                        }
                        else
                        {
                            Index = 0;
                        }

                        break;

                    case ConsoleKey.Tab:
                    case ConsoleKey.F5:
                    case ConsoleKey.Escape:
                        if (Skip.Contains(Read.Key))
                        {
                            return (0, Read.Key);
                        }

                        break;

                    case ConsoleKey.Oem3:
                    case ConsoleKey.OemMinus:
                    case ConsoleKey.OemPlus:
                        if (Skip.Contains(Read.Key))
                        {
                            return (Index, Read.Key);
                        }

                        break;

                    case ConsoleKey.Enter:
                        Console.CursorVisible = true;

                        return (Index, Read.Key);
                }
            }
            while (true);
        }

        public static bool Read(out string Line)
        {
            Line = string.Empty;

            int Index = 0;

            do
            {
                var Read = Console.ReadKey(true);

                switch (Read.Key)
                {
                    case ConsoleKey.Escape:
                        return false;

                    case ConsoleKey.Enter:
                        return true;

                    case ConsoleKey.Backspace:
                        if (Index > 0)
                        {
                            Line = Line.Remove(Line.Length - 1);

                            Console.Write(Read.KeyChar);
                            Console.Write(' ');
                            Console.Write(Read.KeyChar);

                            Index--;
                        }

                        break;

                    default:
                        Line += Read.KeyChar;

                        Console.Write(Read.KeyChar);

                        Index++;

                        break;
                }
            }
            while (true);
        }

        public static DateTime Dump()
        {
            var N = Date();

            var X = N.Date.AddHours(7);

            if (N > X)
            {
                X = X.AddDays(1);
            }

            return X;
        }

        public static TimeSpan? Latency(string Value)
        {

        LATENCY:

            Console.Clear();
            Console.Write(Value);

            if (Read(out string N))
            {
                if (double.TryParse(N, out double X) && X > 0)
                {
                    Console.Clear();
                    Console.WriteLine("\n\n");

                    var Selection = new List<string>
                    {
                        $"Час{(X == 1 ? "" : X < 5 ? "а" : "ов")}",
                        $"Минут{(X == 1 ? "у" : X < 5 ? "ы" : "")}",
                        $"Секунд{(X == 1 ? "у" : X < 5 ? "ы" : "")}"
                    };

                    var Case = Table(0, ">", Selection, Console.CursorTop - 1, ConsoleKey.Escape);

                    if (Case.Key == ConsoleKey.Escape) return null;

                    if (Case.Key == ConsoleKey.Enter)
                    {
                        TimeSpan? Latency = null;

                        switch (Case.Index)
                        {
                            case 0:
                                Latency = TimeSpan.FromHours(X);

                                break;

                            case 1:
                                Latency = TimeSpan.FromMinutes(X);

                                break;
                            case 2:
                                Latency = TimeSpan.FromSeconds(X);

                                break;
                        }

                        return Latency;
                    }
                }
                else
                {
                    goto LATENCY;
                }
            }

            return null;
        }

        public static string Time(TimeSpan TS)
        {
            string F = $"hh\\:mm\\:ss";

            if (TS.Days > 0) F = F.Insert(0, "dd\\.");

            return TS.ToString(F);
        }

        public static DateTime Date(long N)
        {
            long MS = new DateTimeOffset(2015, 1, 1, 0, 0, 0, 0, TimeSpan.Zero).ToUnixTimeMilliseconds();
            long TZ = N >> 22;

            var DATETIME = DateTimeOffset.FromUnixTimeMilliseconds(MS + TZ);
            var LOCAL = DATETIME.ToLocalTime();

            return LOCAL.DateTime;
        }

        public static DateTimeOffset Date()
        {
            var NOW = DateTimeOffset.UtcNow;
            var TS = TimeSpan.FromHours(3);

            return NOW.ToOffset(TS);
        }

        public static double Average(int Count)
        {
            return Math.Ceiling(60d / Count)
                + 1d
                + Random.NextDouble();
        }
    }
}
