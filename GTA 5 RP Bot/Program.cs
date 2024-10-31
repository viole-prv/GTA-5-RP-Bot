using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;

namespace GTA_5_RP_Bot
{
    public partial class Program
    {
        private static readonly string ConfigDirectory = "config";

        private static string ConfigFile = "";
        private static string StorageFile = "";

        private static IConfig? Config;
        private static IStorage? Storage;
        private static Telegram? Telegram;

#pragma warning disable CA2211 // Non-constant fields should not be visible
        public static bool Unique;
#pragma warning restore CA2211 // Non-constant fields should not be visible

        private static bool BAR_DAILY = false;

        private const string MUTE = "656508562553438219";

        private const uint ERROR_SLOW_MODE = 20016;
        private const uint ERROR_ACCESS = 50001;

        #region Message

        public enum EMessage : byte
        {
            DISABLED,
            ENABLED,
            MUTE
        }

        private static EMessage Message;

        #endregion

        #region Security

        public class ISecurity
        {
            public bool Active { get; set; }

            public Thread? Thread { get; set; }

            public DateTimeOffset? Date { get; set; }
            public TimeSpan? Header { get; set; }

            public void DoThread(TimeSpan X)
            {
                Thread = new Thread(() =>
                {
                    try
                    {
                        Date = Helper.Date().Add(X);

                        for (var N = Helper.Date(); N <= Date; N = N.AddSeconds(1))
                        {
                            Header = Date - N;

                            Thread.Sleep(1000);
                        }

                        Active = true;
                    }
                    catch (ThreadInterruptedException) { }
                    finally
                    {
                        Date = null;
                        Header = null;
                    }
                });

                Thread.Start();
            }
        }

        private static readonly ISecurity Security = new();

        #endregion

        #region User

        public class IUser
        {
            public string Token { get; set; }

            public bool Background { get; set; }
            public bool Message { get; set; }

            public Dictionary<string, bool> Security { get; set; }

            public IUser(string Token)
            {
                this.Token = Token;

                Background = true;
                Message = true;

                Security = new();
            }

            public string ID { get; set; } = "UNDEFINED";
            public string Login { get; set; } = "UNDEFINED";
            public string Name { get; set; } = "UNDEFINED";

            #region History

            public class IHistory
            {
                public string ID { get; set; }
                public long LastMessageID { get; set; }

                public IHistory(string ID, long LastMessageID)
                {
                    this.ID = ID;

                    Update(LastMessageID);
                }

                public DateTime Date { get; set; }

                public void Update(long LastMessageID)
                {
                    this.LastMessageID = LastMessageID;

                    Date = Helper.FromUnixTime(LastMessageID);
                }
            }

            public List<IHistory>? History { get; set; }

            #endregion

            public uint? Error { get; set; }

            public double? Time { get; set; }

            public override string ToString()
            {
                return $"[{(Background ? 'x' : ' ')}] {Name}{(Error.HasValue ? $" | ERROR: {Error}" : "")}";
            }
        }

        private static readonly List<IUser> UserList = new();
        private static string UserFile = "";

        #endregion

        #region Separate

        public class ISeparate
        {
            public string ID { get; set; }

            public ISeparate(string ID)
            {
                this.ID = ID;
            }

            public bool? Enabled { get; set; }

            public DateTimeOffset? Date { get; set; }

            #region Position 

            public enum EPosition : byte
            {
                ACTIVE,
                NEXT,
                INACTIVE,
                UNABLE,
                LIMITED
            }

            public EPosition Position { get; set; }

            #endregion

            public string? Content { get; set; }

            #region Car

            public class ICar
            {
                public string Key { get; set; }

                #region List

                public class IList
                {
                    [JsonProperty]
                    public string Name { get; set; }

                    [JsonProperty]
                    public string RL { get; set; } = "";

                    [JsonProperty]
                    public List<string> IDs { get; set; }

                    public enum ETuning : byte
                    {
                        NONE,
                        FT,
                        FFT
                    }

                    [JsonProperty]
                    public ETuning Tuning { get; set; } = ETuning.NONE;

                    [JsonProperty]
                    public string Image { get; set; } = "";

                    [JsonProperty]
                    public List<string> Description { get; set; } = new();

                    [JsonProperty]
                    public Dictionary<int, decimal> Dictionary { get; set; } = new();

                    [JsonIgnore]
                    public bool Any
                    {
                        get
                        {
                            return Dictionary.Any();
                        }
                    }

                    [JsonProperty]
                    public string Spoiler { get; set; } = "";

                    [JsonConstructor]
                    public IList(string Name, List<string> IDs)
                    {
                        Certificate ??= new();

                        foreach (string ID in IDs)
                        {
                            Certificate.Add(new(ID, Name));
                        }

                        this.Name = Name;
                        this.IDs = IDs;
                    }

                    #region Certificate

                    public class ICertificate
                    {
                        public string ID { get; set; }
                        public string Name { get; set; }
                        public bool Active { get; set; }

                        public ICertificate(string ID, string Name)
                        {
                            this.ID = ID;
                            this.Name = $"{ID} - {Name}";

                            Active = true;
                        }

                        public Thread? Thread { get; set; }

                        public DateTimeOffset? Date { get; set; }
                        public TimeSpan? Header { get; set; }

                        public void DoThread(TimeSpan X)
                        {
                            Thread = new Thread(() =>
                            {
                                try
                                {
                                    Active = false;
                                    Date = Helper.Date().Add(X);

                                    if (Storage is not null)
                                    {
                                        if (Storage.Separate.TryAdd(Name, Date.Value))
                                        {
                                            Storage.Save(StorageFile);
                                        }
                                    }

                                    for (var N = Helper.Date(); N <= Date; N = N.AddSeconds(1))
                                    {
                                        Header = Date - N;

                                        Thread.Sleep(1000);
                                    }

                                    if (Message > EMessage.DISABLED)
                                    {
                                        if (Telegram is not null)
                                        {
                                            _ = Telegram.SendMessage($"\"{Name}\" освободился!", Message == EMessage.MUTE);
                                        }
                                    }
                                }
                                catch (ThreadInterruptedException) { }
                                finally
                                {
                                    Active = true;
                                    Date = null;
                                    Header = null;

                                    if (Storage is not null)
                                    {
                                        if (Storage.Separate.ContainsKey(Name))
                                        {
                                            Storage.Separate.Remove(Name);
                                            Storage.Save(StorageFile);
                                        }
                                    }
                                }
                            });

                            Thread.Start();
                        }

                        public string Condition() => "[" + (Active ? '√' : 'x') + "]";

                        public string Value()
                        {
                            string T = "";

                            if (Header.HasValue)
                            {
                                T = $"~ {Helper.Time(Header.Value)}";
                            }

                            return T;
                        }

                        public string To(string X)
                        {
                            return string.Join(" ", new string[] { Condition(), X, Value() });
                        }
                    }

                    [JsonIgnore]
                    public List<ICertificate> Certificate { get; set; }

                    #endregion

                    public override string ToString()
                    {
                        string X = Name.ToUpper();

                        if (Certificate.Count == 1)
                        {
                            return Certificate[0].To(X);
                        }

                        return string.Join(" ", new string[] { "[" + (Certificate.Any(x => x.Active) ? Certificate.All(x => x.Active) ? '√' : ' ' : 'x') + "]", X });
                    }
                }

                public List<IList> List { get; set; }

                #endregion

                public ICar(string Key, List<IList> List)
                {
                    string N = Path.GetDirectoryName(Key)!;

                    foreach (var Value in List)
                    {
                        if (string.IsNullOrEmpty(Value.Image)) continue;

                        Value.Image = Path.Combine(N, Value.Image);
                    }

                    this.Key = Key;
                    this.List = List;
                }
            }

            public ICar? Car { get; set; }

            #endregion

            public List<string>? Image { get; set; }

            public string? GatherContent()
            {
                if (Car is not null)
                {
                    var List = Car.List
                        .Where(x => x.Any)
                        .ToList();

                    if (List.Count > 0)
                    {
                        List<string> Builder = new();

                        foreach (var T in List)
                        {
                            Builder.Add("⠀");
                            Builder.Add($"# :{(T.Certificate.Any(x => x.Active) ? "green" : "red")}_square: {T.Name}{(T.Tuning > 0 ? $" [{T.Tuning}]" : "")}{(string.IsNullOrEmpty(T.RL) ? "" : $" || {T.RL} ||")}");
                            Builder.Add("⠀");

                            #region Description

                            if (T.Description.Count > 0)
                            {
                                Builder.Add("```cs");

                                foreach (string T1 in T.Description)
                                {
                                    Builder.Add($" {T1}");
                                }

                                Builder.Add("```");
                            }

                            #endregion

                            #region Value

                            Builder.Add("```cs");

                            foreach (var Pair in T.Dictionary)
                            {
                                if (Pair.Key > 99 && Config!.ShouldSerializeX2()) continue;

                                Builder.Add($" {Pair.Key} {(Pair.Key == 1 ? "час" : Pair.Key < 5 ? "часа" : "часов")} - {Pair.Value}$");
                            }

                            Builder.Add("```");

                            #endregion
                        }

                        #region Spoiler

                        var Spoiler = List
                            .Select(x => x.Spoiler)
                            .ToList();

                        Spoiler.RemoveAll(x => string.IsNullOrEmpty(x));

                        if (Spoiler.Count > 0)
                        {
                            Builder.Add("⠀");
                            Builder.Add($"|| сдам, аренда, {string.Join(", ", Spoiler)} ||");
                        }

                        #endregion

                        return string.Join(Environment.NewLine, Builder);
                    }
                }

                if (string.IsNullOrEmpty(Content))
                {
                    return null;
                }

                return File.ReadAllText(Content);
            }

            public List<string>? GatherImage()
            {
                if (Car is not null)
                {
                    var List = Car.List
                        .Where(x => x.Any)
                        .ToList();

                    if (List.Count > 0)
                    {
                        var Builder = List
                            .Where(x => x.Certificate.Any(v => v.Active))
                            .Select(x => x.Image)
                            .ToList();

                        if (Builder.Count == 1)
                        {
                            Builder.RemoveAll(x => string.IsNullOrEmpty(x));

                            return Builder;
                        }
                    }
                }

                if (Image == null || Image.Count == 0)
                {
                    return null;
                }

                return Image;
            }

            public bool Go
            {
                get
                {
                    bool Has = Enabled.HasValue && Enabled.Value;
                    bool Any = Car is not null && Car.List.Where(x => x.Any).SelectMany(x => x.Certificate).Any(x => x.Active);

                    return Has || Any;
                }
            }

            public string Condition() => "[" + (Enabled.HasValue ? Enabled.Value ? '√' : 'x' : ' ') + "]";

            public string Value()
            {
                string T = "";

                if (Date.HasValue)
                {
                    var TS = Helper.Date() - Date.Value;

                    if (TS < TimeSpan.Zero)
                    {
                        T = $"~ {TS:hh\\:mm\\:ss}";
                    }
                }
                else if (Position > 0)
                {
                    T = $"- {Position}";
                }

                return T;
            }

            public override string ToString()
            {
                return string.Join(" ", new string[] { Condition(), ID, Value() });
            }
        }

        private static List<ISeparate> SeparateList = new();

        #endregion

        #region Notice

        public class INotice
        {
            public string Name { get; set; }
            public int Group { get; set; }

            public INotice(string Name, int Group)
            {
                this.Name = Name;
                this.Group = Group;
            }

            public TimeSpan? Time { get; set; }

            public INotice(string Name, TimeSpan? Time = null)
            {
                this.Name = Name;
                this.Time = Time;
            }

            public List<INotice> List { get; set; } = new();

            public bool AFTER_RESTART { get; set; }
            public bool SHOW_DATA { get; set; }
            public bool USE_WAIT { get; set; }

            #region Notification

            public enum INotification : byte
            {
                NONE,
                BEEP,
                MESSAGE
            }

            public INotification Notification { get; set; } = INotification.MESSAGE;

            #endregion

            #region Type

            public enum EType : byte
            {
                NONE,
                ACTIVE,
                WAIT
            }

            public EType Type { get; set; }

            public bool Active
            {
                get => Type == EType.ACTIVE;
            }

            #endregion

            public void Modify()
            {
                switch (Type)
                {
                    case EType.ACTIVE:

                        Factory();

                        Type = EType.NONE;

                        break;

                    case EType.NONE when USE_WAIT:

                        Type = EType.WAIT;

                        break;

                    default:

                        Type = EType.ACTIVE;

                        break;
                }
            }

            public CancellationTokenSource? Source { get; set; }

            public DateTimeOffset? Date { get; set; }
            public TimeSpan? Header { get; set; }

            private void Factory()
            {
                Source?.Cancel();

                Date = null;
                Header = null;
            }

            public string Condition()
            {
                char T = ' ';

                if (Recursion(Name, List).Any(x => x.Value.Active))
                {
                    T = ' ';
                }
                else
                {
                    switch (Type)
                    {
                        case EType.ACTIVE:

                            T = 'x';

                            break;

                        case EType.WAIT:

                            T = '∙';

                            break;

                        case EType.NONE:

                            T = '√';

                            break;
                    }
                }
                

                return "[" + T + "]";
            }

            public string Value()
            {
                if (Active)
                {
                    if (Header.HasValue)
                    {
                        return $"~ {Helper.Time(Header.Value)}";
                    }
                }
                    
                if (SHOW_DATA)
                {
                    if (Date.HasValue)
                    {
                        return $"- {Date.Value:hh:mm:ss}";
                    }
                }

                return "";
            }

            public override string ToString()
            {
                return string.Join(" ", new string[] { Condition(), Name, Value() });
            }
        }

        private static readonly List<INotice> NoticeList = new();

        private static Dictionary<string, INotice> NoticeDictionary
        {
            get
            {
                var Dictionary = new Dictionary<string, INotice>();

                foreach (var Notice in NoticeList)
                {
                    if (Notice.Time.HasValue)
                    {
                        Dictionary[Notice.Name] = Notice;
                    }

                    if (Notice.List.Count == 0) continue;

                    foreach (var T in Recursion(Notice.Name, Notice.List))
                    {
                        Dictionary[T.Key] = T.Value;
                    }
                }

                return Dictionary;
            }
        }

        public static Dictionary<string, INotice> Recursion(string X, List<INotice> NoticeList)
        {
            var Dictionary = new Dictionary<string, INotice>();

            foreach (var Notice in NoticeList)
            {
                string Name = $"{X} - {Notice.Name}";

                if (Notice.List.Count == 0)
                {
                    Dictionary[Name] = Notice;
                }
                else
                {
                    if (Notice.Time.HasValue)
                    {
                        Dictionary[Name] = Notice;
                    }

                    foreach (var T in Recursion(Name, Notice.List))
                    {
                        Dictionary[T.Key] = T.Value;
                    }
                }
            }

            return Dictionary;
        }

        #endregion

        #region Compensation

        public class ICompensation
        {
            public string Name { get; set; }
            public int Value { get; set; }

            public int Start { get; set; }
            public int Per { get; set; }

            public int Count { get; set; }

            public ICompensation(string Name, int Value, int Start = 0, int Per = 0, int Count = 1)
            {
                this.Name = Name;

                if (Config!.ShouldSerializeVIP()) Value *= 2;
                if (Config!.ShouldSerializeX2()) Value *= 2;

                this.Value = Value;

                this.Start = Start;
                this.Per = Per;

                this.Count = Count;
            }

            public int Index { get; set; }

            public Thread? Thread { get; set; }

            public DateTimeOffset? Date { get; set; }

            public void DoThread(DateTimeOffset X)
            {
                Thread = new Thread(() =>
                {
                    try
                    {
                        Date = X;

                        if (Storage is not null)
                        {
                            if (Storage.Compensation.TryAdd(Name, Date.Value))
                            {
                                Storage.Save(StorageFile);
                            }
                        }

                        for (var N = Helper.Date(); N <= Date; N = N.AddMinutes(1))
                        {
                            Thread.Sleep(60000);
                        }

                        Index = 0;
                    }
                    catch (ThreadInterruptedException) { }
                    finally
                    {
                        Date = null;

                        if (Storage is not null)
                        {
                            if (Storage.Compensation.ContainsKey(Name))
                            {
                                Storage.Compensation.Remove(Name);
                                Storage.Save(StorageFile);
                            }
                        }
                    }
                });

                Thread.Start();
            }

            public override string ToString()
            {
                int[] Array = new int[] { Start, Per }.Where(x => x > 0).ToArray();

                return $"[{(Index == Count ? '√' : 'x')}] {Name} - {Value} BP{(Array.Length == 0 ? "" : " = ")}{string.Join(" | ", Array.Select(N => N + "$"))}{(Count > 1 ? $" ({Index}/{Count})" : "")}";
            }
        }

        private static readonly List<ICompensation> CompensationList = new();

        #endregion

        #region Enumerator

        public class IEnumerator : System.Collections.IEnumerator
        {
            private readonly int[] List;

            public IEnumerator(int[] List)
            {
                this.List = List;
            }

            public int Position { get; set; }

            public bool MoveNext() => ++Position >= List.Length;

            public void Reset()
            {
                Position = 0;
            }

            object System.Collections.IEnumerator.Current => Current;

            public int Current
            {
                get => List[Position];
            }
        }


        #endregion

        [SupportedOSPlatform("Windows")]
        public static void Main(string[] A)
        {
            BAR();

            var Execute = Assembly.GetExecutingAssembly().GetName();

            if (Execute == null || string.IsNullOrEmpty(Execute.Name) || Execute.Version == null) return;

            if (!Directory.Exists(ConfigDirectory)) Directory.CreateDirectory(ConfigDirectory);

            #region Mutex

            var List = new List<string>
            {
                Execute.Name,
                Execute.Version.ToString()
            };

            if (A.Length > 0)
            {
                List.Add("X");
            }

            _ = new Mutex(true, string.Join('∙', List), out Unique);

            #endregion

            if (A.Length == 0 && Unique)
            {
                string[] Directories = Directory.GetDirectories(ConfigDirectory);

                if (Directories.Length == 0) return;

                foreach (var T in Directories)
                {
                    var Process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = Environment.ProcessPath,
                            UseShellExecute = true,
                            Arguments = Path.GetFileName(T)
                        }
                    };

                    Process.Start();
                }

                Environment.Exit(0);
            }
            else
            {
                string TEMP = Path.Combine(ConfigDirectory, A[0]);

                if (Directory.Exists(TEMP) && int.TryParse(Path.GetFileName(TEMP), out int Index))
                {
                    #region Config

                    ConfigFile = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        ConfigDirectory,
                        "config.json"
                    );

                    (string? ConfigErrorMessage, Config) = IConfig.Load(ConfigFile);

                    if (Config == null)
                    {
                        Console.WriteLine(ConfigErrorMessage);

                        return;
                    }

                    if (Config.ShouldSerializeTelegram())
                    {
                        Telegram = new Telegram(Config.Telegram.Token, Config.Telegram.ChatID);
                    }

                    #endregion

                    Directory.SetCurrentDirectory(TEMP);

                    #region Storage

                    StorageFile = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "!.json"
                    );

                    (string? StorageErrorMessage, Storage) = IStorage.Load(StorageFile);

                    if (Storage == null)
                    {
                        Console.WriteLine(StorageErrorMessage);

                        return;
                    }

                    #endregion

                    #region User

                    UserFile = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "!.txt"
                    );

                    if (!File.Exists(UserFile)) File.Create(UserFile);

                    string[] T = File.ReadAllLines(UserFile);

                    if (T.Length == 0) return;

                    foreach (string Token in T)
                    {
                        if (Regex.IsMatch(Token, @"^([a-zA-Z0-9_-]{24}\.[a-zA-Z0-9_-]{6}\.[a-zA-Z0-9_-]{38})$"))
                        {
                            UserList.Add(new IUser(Token));
                        }
                    }

                    if (UserList.Count == 0) return;

                    #endregion

                    int X = 100;
                    int Y = 100;

                    if (Index > 1)
                    {
                        X = Index * 180;
                    }

                    Native.SetWindowPos(
                        Native.GetConsoleWindow(),
                        IntPtr.Zero,
                        X, Y,
                        0, 0,
                        Native.SWP_NOSIZE
                    );

                    using (Bitmap Bitmap = new(32, 32))
                    {
                        var Icon = Properties.Resources.Icon;

                        using Graphics Graphics = System.Drawing.Graphics.FromImage(Bitmap);

                        Graphics.DrawIcon(Icon, 0, 0);
                        Graphics.DrawString(
                            Index.ToString(),
                            new Font("Calibri", 12, FontStyle.Regular),
                            Brushes.White,
                            new Rectangle(0, 0, Icon.Width, Icon.Height),
                            new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

                        IntPtr Hicon = Bitmap.GetHicon();

                        var Current = Process.GetCurrentProcess();

                        Native.SendMessage(Current.MainWindowHandle, Native.WM_SETICON, Native.ICON_SMALL, Hicon);
                        Native.SendMessage(Current.MainWindowHandle, Native.WM_SETICON, Native.ICON_BIG, Hicon);
                    }

                    Console.OutputEncoding = System.Text.Encoding.UTF8;

                    Init();
                }
            }


            Console.ReadLine();
        }

        private static void Init()
        {
            string[] Directories = Directory.GetDirectories(Directory.GetCurrentDirectory());

            if (Directories.Length == 0) return;

            foreach (string Directory in Directories)
            {
                string Name = Path.GetFileName(Directory);

                if (Regex.IsMatch(Name, @"^\d+$"))
                {
                    string[] Files = System.IO.Directory.GetFiles(Directory);

                    if (Files.Length == 0) continue;

                    var X = new ISeparate(Name);

                    foreach (string File in Files.Where(x => x.EndsWith(".txt")))
                    {
                        X.Content = File;
                    }

                    foreach (string File in Files.Where(x => x.EndsWith(".json")))
                    {
                        var CarList = JsonConvert.DeserializeObject<List<ISeparate.ICar.IList>>(System.IO.File.ReadAllText(File));

                        if (CarList == null) continue;

                        X.Car = new(File, CarList);
                    }

                    foreach (string File in Files.Where(x => x.EndsWith(".jpg") || x.EndsWith(".jpeg") || x.EndsWith(".png") || x.EndsWith(".webp") || x.EndsWith(".gif")))
                    {
                        X.Image ??= new();
                        X.Image.Add(File);
                    }

                    SeparateList.Add(X);
                }
            }

            var FileSystemWatcher = new FileSystemWatcher(Directory.GetCurrentDirectory())
            {
                NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastWrite
            };

            FileSystemWatcher.Changed += OnChanged;
            FileSystemWatcher.Created += OnCreated;
            FileSystemWatcher.Deleted += OnDeleted;
            FileSystemWatcher.Renamed += OnRenamed;

            Current = new ConcurrentDictionary<string, object>();

            FileSystemWatcher.EnableRaisingEvents = true;
            FileSystemWatcher.IncludeSubdirectories = true;

            if (SeparateList.Count == 0) return;

            Begin();

            foreach (string Value in SeparateList.Select(v => v.ID))
            {
                UserList.ForEach(x => x.Security.Add(Value, true));
            }

            new Thread(() => DoUser(UserList)).Start();
            new Thread(() => DoMessage(UserList)).Start();
            new Thread(() => DoSeparate(UserList)).Start();

            if (Unique)
            {
                for (int i = 1; i <= 3; i++)
                {
                    NoticeList.Add(new($"Аккаунт #{i}", 1)
                    {
                        List = new List<INotice>
                        {
                            new("Задание", TimeSpan.FromHours(2)),
                            new("Событие", TimeSpan.FromHours(5))
                            {
                                List = new List<INotice>
                                {
                                    new("Доставка горючего", TimeSpan.FromHours(5)) 
                                    {
                                        Notification = INotice.INotification.NONE 
                                    },
                                    new("Следуйте по маршруту", TimeSpan.FromHours(5)) 
                                    {
                                        Notification = INotice.INotification.NONE
                                    },

                                    new("Попал на риф", TimeSpan.FromHours(5)) 
                                    {
                                        Notification = INotice.INotification.NONE
                                    },
                                    new("Ритм праздника", TimeSpan.FromHours(5))
                                    {
                                        Notification = INotice.INotification.NONE
                                    },

                                    new("Частная жизнь", TimeSpan.FromHours(5))
                                    {
                                        Notification = INotice.INotification.NONE
                                    },
                                    new("Безопасное пространство", TimeSpan.FromHours(5))
                                    {
                                        USE_WAIT = true,

                                        Notification = INotice.INotification.NONE
                                    },

                                    new("Уборка озера", TimeSpan.FromHours(5))
                                    {
                                        Notification = INotice.INotification.NONE
                                    },
                                    new("Запал праздника", TimeSpan.FromHours(5))
                                    {
                                        Notification = INotice.INotification.NONE
                                    },

                                    new("Тестирование", TimeSpan.FromHours(5))
                                    {
                                        Notification = INotice.INotification.NONE
                                    },
                                    new("Богатый рацион", TimeSpan.FromHours(5))
                                    {
                                        USE_WAIT = true,

                                        Notification = INotice.INotification.NONE
                                    }
                                }
                            },
                            new("Неофициальная организация")
                            {
                                List = new List<INotice>
                                {
                                    new("Большой улов", TimeSpan.FromHours(24)) 
                                    {
                                        SHOW_DATA = true
                                    },

                                    new("Грандиозная уборка", TimeSpan.FromHours(26)) 
                                    {
                                        SHOW_DATA = true
                                    },
                                    new("Мясной день", TimeSpan.FromHours(26))
                                    {
                                        SHOW_DATA = true
                                    },

                                    new("Долгожданная встреча", TimeSpan.FromHours(20))
                                    {
                                        SHOW_DATA = true
                                    },
                                    new("Обновляем гардероб", TimeSpan.FromHours(20)) 
                                    {
                                        SHOW_DATA = true
                                    }
                                }}
                        }
                    });
                }

                NoticeList.Add(new("Преступный синдикат", 2)
                {
                    List = new List<INotice>
                    {
                        new("Угон авто", TimeSpan.FromHours(1.5)) 
                        {
                            AFTER_RESTART = true,
                            SHOW_DATA = true
                        },
                        new("Работа сутенёром", TimeSpan.FromHours(1.5)) 
                        {
                            AFTER_RESTART = true,
                            SHOW_DATA = true 
                        }
                    }
                });

                NoticeList.Add(new("Неофициальная организация", 2)
                {
                    Time = TimeSpan.FromHours(2),
                    SHOW_DATA = true,
                    List = new List<INotice>
                    {
                        new("Скользкая дорожка", TimeSpan.FromHours(3)) 
                        {
                            AFTER_RESTART = true,
                            SHOW_DATA = true
                        },
                        new("Мотивированное волонтерство", TimeSpan.FromHours(3))
                        {
                            AFTER_RESTART = true,
                            SHOW_DATA = true
                        },

                        new("Долгожданная встреча", TimeSpan.FromHours(4)) 
                        {
                            AFTER_RESTART = true,
                            SHOW_DATA = true 
                        },
                        new("Обновляем гардероб", TimeSpan.FromHours(4)) 
                        {
                            AFTER_RESTART = true,
                            SHOW_DATA = true 
                        }
                    }
                });


                NoticeList.AddRange(new List<INotice>
                {
                    new("Почта", 3) 
                    {
                        AFTER_RESTART = true,

                        Time = TimeSpan.FromMinutes(10), 
                        Notification = INotice.INotification.BEEP 
                    },
                    new("Работа", 3)
                    {
                        AFTER_RESTART = true,

                        Time = TimeSpan.FromMinutes(3), 
                        Notification = INotice.INotification.BEEP 
                    }
                });

                new Thread(() => DoNotice()).Start();

                CompensationList.AddRange(new List<ICompensation>
                {
                    new ("Купить 1 лотерейный билет через телефон", 1, 1500),
                    new ("Проехать 1 уличную гонку, созданную через телефон со ставкой от 1000$ (с 5-го уровня)", 1, 1000),
                    new ("Арендовать Киностудию (с наличием VIP предоставляется скидка)", 2, Config!.VIP switch
                    {
                        IConfig.EVIP.Gold => 5000,
                        IConfig.EVIP.Platinum => 2500,
                        _ => 10000
                    }),
                    new ("Добавить 5 видео в Кинотеатр (можно добавить 1 ролик 5 раз)", 1, 100, 50, 5),
                    new ("Выполнить 1 успешную тренировку в тире", 1, 500),
                    new ("Выиграть 3 игры на Арене Maze Bank со ставкой от 100$", 1, Per: 100, Count: 3),
                    new ("Выиграть 5 игр в Тренировочном комплексе (ТК) со ставкой от 100$", 1, Per: 100, Count: 5),
                    new ("Выиграть 1 гонку на картинге", 1, 500),
                    new ("Выполнить 10 действий на ферме (например, подоить корову)", 1, Count: 10),
                    new ("Выполнить 25 действий в порту", 2, Count: 25),
                    new ("Выполнить 25 действий на шахте", 2, Count: 25),
                    new ("Выполнить 25 действий на стройке", 2, Count: 25),
                    new ("Выполнить 2 рейса на любом маршруте, работая водителем автобуса", 2, Count: 2),
                    new ("Доставить 10 посылок, работая почтальоном", 1, Count: 10),
                    new ("Выполнить 20 подходов на любом тренажёре в спортивном зале или на спортплощадке", 1, Count: 20)
                });
            }

            new Thread(() => DoStorage()).Start();

            int Index = 0;

        RETRY:

            BAR_DAILY = false;

            Console.Clear();
            Console.WriteLine("\n\n");

            var Selection = new List<string>
            {
                "[ ] Список",
                "[ ] Пользователи",
                "",
                $"[{(Message == EMessage.ENABLED ? '√' : Message == EMessage.DISABLED ? 'x' : '∙')}] Сообщения",
                $"[{(Security.Header.HasValue ? '∙' : Security.Active ? '√' : 'x')}] Темы",
            };

            if (Unique)
            {
                Selection.AddRange(new string[]
                {
                    "",
                    "[ ] Уведомления",
                    "[ ] Вознаграждение"
                });
            }

            var Case = Helper.Table(Index, ">", Selection, Console.CursorTop - 1, ConsoleKey.F5, ConsoleKey.Oem3);

            if (Case.Key == ConsoleKey.F5) goto RETRY;

            if (Selection[Case.Index].Contains("Список"))
            {
                Index = 0;

                if (Case.Key == ConsoleKey.Enter)
                {

                GOTO_SEPARATE:

                    Console.Clear();
                    Console.WriteLine("\n\n");

                    Selection = SeparateList
                        .Select(x => x.ToString())
                        .ToList();

                    Case = Helper.Table(Index, ">", Selection, Console.CursorTop - 1, ConsoleKey.F5, ConsoleKey.Escape);

                    Index = Case.Index;

                    if (Case.Key == ConsoleKey.F5) goto GOTO_SEPARATE;

                    if (Case.Key == ConsoleKey.Escape) goto RETRY;

                    if (Case.Key == ConsoleKey.Enter)
                    {
                        var Separate = SeparateList[Case.Index];

                        if (Separate.Car == null)
                        {
                            Separate.Enabled = !Separate.Enabled;
                        }
                        else
                        {

                            Index = 0;

                        LIST:

                            Console.Clear();
                            Console.WriteLine("\n\n");

                            Selection = Separate.Car.List
                                .Select(x => x.ToString())
                                .ToList();

                            Case = Helper.Table(Index, ">", Selection, Console.CursorTop - 1, ConsoleKey.F5, ConsoleKey.Escape, ConsoleKey.OemMinus, ConsoleKey.OemPlus);

                            Index = Case.Index;

                            if (Case.Key == ConsoleKey.F5) goto LIST;

                            if (Case.Key == ConsoleKey.Escape) goto GOTO_SEPARATE;

                            var Car = Separate.Car.List[Case.Index];

                            if (Car.Certificate.Count == 1)
                            {
                                DO(Car.Certificate[0]);

                                goto LIST;
                            }
                            else
                            {
                                Index = 0;

                            CERTIFICATE:

                                Console.Clear();
                                Console.WriteLine("\n\n");

                                Selection = Car.Certificate
                                    .Select(x => x.To(x.ID))
                                    .ToList();

                                Case = Helper.Table(Index, ">", Selection, Console.CursorTop - 1, ConsoleKey.F5, ConsoleKey.Escape, ConsoleKey.OemMinus, ConsoleKey.OemPlus);

                                Index = Case.Index;

                                if (Case.Key == ConsoleKey.F5) goto CERTIFICATE;

                                if (Case.Key == ConsoleKey.Escape) goto LIST;

                                DO(Car.Certificate[Case.Index]);

                                goto CERTIFICATE;
                            }

                            void DO(ISeparate.ICar.IList.ICertificate Certificate)
                            {
                                if (Case.Key == ConsoleKey.OemMinus || Case.Key == ConsoleKey.OemPlus)
                                {
                                    if (Certificate.Date.HasValue)
                                    {
                                        var Choice = Helper.Choice($"У{(Case.Key == ConsoleKey.OemMinus ? "меньш" : Case.Key == ConsoleKey.OemPlus ? "велич" : "")}ить на: ");

                                        if (Choice.HasValue)
                                        {
                                            if (Case.Key == ConsoleKey.OemMinus)
                                            {
                                                Certificate.Date = Certificate.Date.Value.Add(-Choice.Value);
                                            }
                                            else if (Case.Key == ConsoleKey.OemPlus)
                                            {
                                                Certificate.Date = Certificate.Date.Value.Add(Choice.Value);
                                            }

                                            if (Storage is not null)
                                            {
                                                if (Storage.Separate.ContainsKey(Certificate.Name))
                                                {
                                                    Storage.Separate[Certificate.Name] = Certificate.Date.Value;
                                                    Storage.Save(StorageFile);
                                                }
                                            }
                                        }
                                    }

                                    return;
                                }

                                if (Case.Key == ConsoleKey.Enter)
                                {
                                    if (Certificate.Active)
                                    {
                                        Console.Clear();
                                        Console.WriteLine("\n\n");

                                        Selection = new List<string>
                                        {
                                            "Время",
                                            "Дата"
                                        };

                                        Case = Helper.Table(0, ">", Selection, Console.CursorTop - 1, ConsoleKey.Escape);

                                        if (Case.Key == ConsoleKey.Escape) return;

                                        if (Case.Key == ConsoleKey.Enter)
                                        {
                                            switch (Case.Index)
                                            {
                                                case 0:
                                                    var Choice = Helper.Choice("Аренда: ");

                                                    if (Choice.HasValue)
                                                    {
                                                        Certificate.DoThread(Choice.Value);
                                                    }

                                                    break;

                                                case 1:
                                                    Console.Clear();
                                                    Console.Write("Дата: ");

                                                    if (Helper.Read(out string N))
                                                    {
                                                        var Date = Helper.Date();

                                                        var T = new DateTimeOffset(
                                                            Date.Year,
                                                            01, 01,
                                                            00, 00, 00,
                                                            TimeSpan.FromHours(3)
                                                        );

                                                        var Match = Regex.Match(N, @"(?<HOUR>[01]\d|2[0-3]):(?<MINUTE>[0-5]\d)(?: (?<DAY>[0-3]\d)\.(?<MONTH>0[1-9]|1[0-2]))?");

                                                        if (Match.Success)
                                                        {
                                                            var _HOUR = Match.Groups["HOUR"];

                                                            if (_HOUR.Success && double.TryParse(_HOUR.Value, out double HOUR))
                                                            {
                                                                T = T.AddHours(HOUR);
                                                            }

                                                            var _MINUTE = Match.Groups["MINUTE"];

                                                            if (_MINUTE.Success && double.TryParse(_MINUTE.Value, out double MINUTE))
                                                            {
                                                                T = T.AddMinutes(MINUTE);
                                                            }

                                                            if (Match.Groups["OPTIONAL"].Success)
                                                            {
                                                                var _DAY = Match.Groups["DAY"];

                                                                if (_DAY.Success && double.TryParse(_DAY.Value, out double DAY))
                                                                {
                                                                    T = T.AddDays(DAY - 1);
                                                                }

                                                                var _MONTH = Match.Groups["MONTH"];

                                                                if (_MONTH.Success && int.TryParse(_MONTH.Value, out int MONTH))
                                                                {
                                                                    T = T.AddMonths(MONTH - 1);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                T = T.AddDays(Date.Day - 1);
                                                                T = T.AddMonths(Date.Month - 1);

                                                                if (T.Hour < Date.Hour)
                                                                {
                                                                    T = T.AddDays(1);
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            goto case 1;
                                                        }

                                                        Certificate.DoThread(T - Date);
                                                    }

                                                    break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (Certificate.Thread is not null)
                                        {
                                            try
                                            {
                                                Certificate.Thread.Interrupt();
                                                Certificate.Thread = null;
                                            }
                                            catch { }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    goto GOTO_SEPARATE;
                }
            }
            else if (Selection[Case.Index].Contains("Пользователи"))
            {
                Index = 0;

                if (Case.Key == ConsoleKey.Enter)
                {

                GOTO_USER:

                    Console.Clear();
                    Console.WriteLine("\n\n");

                    Selection = UserList
                        .Select(x => x.ToString())
                        .ToList();

                    Case = Helper.Table(Index, ">", Selection, Console.CursorTop - 1, ConsoleKey.F5, ConsoleKey.Escape);

                    Index = Case.Index;

                    if (Case.Key == ConsoleKey.F5) goto GOTO_USER;

                    if (Case.Key == ConsoleKey.Escape) goto RETRY;

                    if (Case.Key == ConsoleKey.Enter)
                    {
                        var User = UserList[Case.Index];

                        if (User.Background)
                        {
                            goto GOTO_USER;
                        }

                    USER:

                        Console.Clear();
                        Console.WriteLine("\n\n");

                        Selection = new List<string>
                        {
                            $"[{(User.Message ? '√' : 'x')}] Сообщения", "[ ] Темы"
                        };

                        Case = Helper.Table(0, ">", Selection, Console.CursorTop - 1, ConsoleKey.F5, ConsoleKey.Escape);

                        if (Case.Key == ConsoleKey.F5) goto USER;

                        if (Case.Key == ConsoleKey.Escape) goto GOTO_USER;

                        if (Case.Key == ConsoleKey.Enter)
                        {
                            switch (Case.Index)
                            {
                                case 0:
                                    User.Message = !User.Message;

                                    goto USER;

                                case 1:

                                    Console.Clear();
                                    Console.WriteLine("\n\n");

                                    Selection = User.Security
                                        .Select(x => $"[{(x.Value ? '√' : 'x')}] {x.Key}")
                                        .ToList();

                                    Case = Helper.Table(0, ">", Selection, Console.CursorTop - 1, ConsoleKey.F5, ConsoleKey.Escape);

                                    if (Case.Key == ConsoleKey.F5) goto case 1;

                                    if (Case.Key == ConsoleKey.Escape) goto USER;

                                    if (Case.Key == ConsoleKey.Enter)
                                    {
                                        foreach (var Thread in User.Security)
                                        {
                                            if (Selection[Case.Index].Contains(Thread.Key))
                                            {
                                                User.Security[Thread.Key] = !User.Security[Thread.Key];

                                                break;
                                            }
                                        }

                                    }

                                    goto case 1;
                            }
                        }
                    }

                    goto GOTO_USER;
                }
            }
            else if (Selection[Case.Index].Contains("Сообщения"))
            {
                Index = Case.Index;

                if (UserList.Any(x => x.Background)) goto RETRY;

                if (Case.Key == ConsoleKey.Enter)
                {
                    var List = Enum
                        .GetValues(typeof(EMessage))
                        .Cast<EMessage>()
                        .ToList();

                    int Value = List.IndexOf(Message) + 1;

                    Message = List.Count == Value
                        ? List[0]
                        : List[Value];
                }
            }
            else if (Selection[Case.Index].Contains("Темы"))
            {
                Index = Case.Index;

                if (UserList.Any(x => x.Background)) goto RETRY;

                if (Case.Key == ConsoleKey.Enter)
                {
                    if (Security.Active)
                    {
                        if (Security.Thread is not null)
                        {
                            try
                            {
                                Security.Thread.Interrupt();
                                Security.Thread = null;
                            }
                            catch { }
                        }

                        Security.Active = false;
                    }
                    else
                    {
                        Security.Active = true;
                    }
                }
                else
                {
                    if (Security.Active) goto RETRY;

                    var Choice = Helper.Choice("Через: ");

                    if (Choice.HasValue)
                    {
                        Security.DoThread(Choice.Value);
                    }
                }
            }
            else if (Selection[Case.Index].Contains("Уведомления"))
            {
                Index = 0;

                if (Case.Key == ConsoleKey.Enter)
                {
                    INotice? Value = null;

                    var List = NoticeList;
                    var History = new List<List<INotice>>();

                    List<string> Header = new();

                    IEnumerator? Enumerator = null;
                    int? Position = null;

                GOTO_NOTICE:

                    Console.Clear();
                    Console.WriteLine("\n\n");

                    int Max = List.Max(x => x.Group);

                    if (Max == 0)
                    {
                        Enumerator = null;
                    }
                    else
                    {
                        if (Enumerator == null)
                        {
                            Enumerator = new IEnumerator(new int[Max]
                                .Select((x, i) => i + 1)
                                .ToArray());

                            if (Position.HasValue)
                            {
                                Enumerator.Position = Position.Value;
                            }
                        }
                    }

                    var Cluster = List
                        .Where(x => Enumerator == null || Enumerator.Current == x.Group)
                        .ToList();

                    Selection = Cluster
                        .Select(x => x.ToString())
                        .ToList();

                    Case = Helper.Table(Index, ">", Selection, Console.CursorTop - 1, ConsoleKey.Tab, ConsoleKey.F5, ConsoleKey.Escape, ConsoleKey.OemMinus, ConsoleKey.OemPlus);

                    Index = Case.Index;

                    if (Case.Key == ConsoleKey.Tab)
                    {
                        if (Enumerator is not null)
                        {
                            if (Enumerator.MoveNext())
                            {
                                Enumerator.Reset();
                            }

                            Position = Enumerator.Position;
                        }

                        Index = 0;

                        goto GOTO_NOTICE;
                    }

                    if (Case.Key == ConsoleKey.F5) goto GOTO_NOTICE;

                    if (Case.Key == ConsoleKey.Escape)
                    {
                        if (History.Count == 0) goto RETRY;

                        List = History.Last();
                        History.Remove(List);

                        Header.Remove(Header.Last());

                        Index = 0;

                        goto GOTO_NOTICE;
                    }

                    var Notice = Cluster[Case.Index];

                    if (Case.Key == ConsoleKey.OemMinus || Case.Key == ConsoleKey.OemPlus)
                    {
                        if (Notice.Date.HasValue)
                        {
                            var Choice = Helper.Choice($"У{(Case.Key == ConsoleKey.OemMinus ? "меньш" : Case.Key == ConsoleKey.OemPlus ? "велич" : "")}ить на: ");

                            if (Choice.HasValue)
                            {
                                if (Case.Key == ConsoleKey.OemMinus)
                                {
                                    Notice.Date = Notice.Date.Value.Add(-Choice.Value);
                                }
                                else if (Case.Key == ConsoleKey.OemPlus)
                                {
                                    Notice.Date = Notice.Date.Value.Add(Choice.Value);
                                }

                                if (Storage is not null)
                                {
                                    string Name = string.Join(" - ", Header);

                                    if (Storage.Notice.ContainsKey(Name))
                                    {
                                        Storage.Notice[Name] = Notice.Date.Value;
                                        Storage.Save(StorageFile);
                                    }
                                }
                            }
                        }

                        Index = 0;

                        goto GOTO_NOTICE;
                    }

                    if (Case.Key == ConsoleKey.Enter)
                    {
                        if (Notice.List.Count == 0)
                        {
                            Notice.Modify();

                            if (Value == null || Value.List == null) goto GOTO_NOTICE;

                            if (Value.Time.HasValue)
                            {
                                if (Notice.Type < INotice.EType.WAIT)
                                {
                                    if (Value.Active)
                                    {
                                        int Count = Value.List.Count(x => x.Active);

                                        switch (Count)
                                        {
                                            case 0:

                                                Value.Modify();

                                                break;

                                            case > 1:

                                                Value.Date = Helper.Date().Add(Value.Time.Value);

                                                if (Storage is not null)
                                                {
                                                    string Name = string.Join(" - ", Header);

                                                    if (Storage.Notice.ContainsKey(Name))
                                                    {
                                                        Storage.Notice[Name] = Value.Date.Value;
                                                        Storage.Save(StorageFile);
                                                    }
                                                }

                                                break;
                                        }
                                    }
                                    else
                                    {
                                        if (Notice.Active)
                                        {
                                            Value.Modify();
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            Value = Notice;

                            History.Add(List);
                            List = Value.List;

                            Header.Add(Value.Name);

                            Index = 0;
                        }
                    }

                    goto GOTO_NOTICE;
                }
            }
            else if (Selection[Case.Index].Contains("Вознаграждение"))
            {
                Index = 0;

                if (Case.Key == ConsoleKey.Enter)
                {

                GOTO_DAILY:

                    BAR_DAILY = true;

                    Console.Clear();
                    Console.WriteLine("\n\n");

                    Selection = CompensationList
                        .Select(x => x.ToString())
                        .ToList();

                    Case = Helper.Table(Index, ">", Selection, Console.CursorTop - 1, ConsoleKey.F5, ConsoleKey.Escape, ConsoleKey.Enter, ConsoleKey.OemMinus, ConsoleKey.OemPlus);

                    Index = Case.Index;

                    if (Case.Key == ConsoleKey.F5) goto GOTO_DAILY;

                    if (Case.Key == ConsoleKey.Escape) goto RETRY;

                    var Compensation = CompensationList[Case.Index];

                    if (Case.Key == ConsoleKey.Enter)
                    {
                        Compensation.Index = Compensation.Count;

                        Compensation.DoThread(Helper.Tomorrow());
                    }
                    else if (Case.Key == ConsoleKey.OemMinus)
                    {
                        if (Compensation.Index > 0)
                        {
                            Compensation.Index -= 1;
                        }

                        if (Compensation.Index == 0)
                        {
                            if (Compensation.Thread is not null)
                            {
                                try
                                {
                                    Compensation.Thread.Interrupt();
                                    Compensation.Thread = null;
                                }
                                catch { }
                            }
                        }
                    }
                    else if (Case.Key == ConsoleKey.OemPlus)
                    {
                        if (Compensation.Index < Compensation.Count)
                        {
                            Compensation.Index += 1;
                        }

                        if (Compensation.Index == Compensation.Count)
                        {
                            Compensation.DoThread(Helper.Tomorrow());
                        }
                    }

                    goto GOTO_DAILY;
                }
            }

            goto RETRY;
        }

        private static void Begin()
        {
            foreach (var Separate in SeparateList)
            {
                if (Separate.Car == null)
                {
                    Separate.Enabled = true;
                }
                else
                {
                    if (Separate.Image is not null && Separate.Image.Count > 0)
                    {
                        foreach (var Car in Separate.Car.List)
                        {
                            if (string.IsNullOrEmpty(Car.Image)) continue;

                            Separate.Image.RemoveAll(Image => Image == Car.Image);
                        }
                    }
                }
            }

            SeparateList = SeparateList
                .OrderBy(x => x.Car == null)
                .ToList();
        }

        private static async void DoUser(List<IUser> UserList)
        {
            foreach (IUser User in UserList)
            {
                try
                {
                    var Me = await Discord.Me(User);

                    if (Me is not null)
                    {
                        User.ID = string.IsNullOrEmpty(Me.ID)
                            ? "NULL"
                            : Me.ID;

                        User.Login = string.IsNullOrEmpty(Me.Login)
                            ? "NULL"
                            : Me.Login;

                        User.Name = string.IsNullOrEmpty(Me.Name)
                            ? "NULL"
                            : Me.Name;

                        var Banner = await Discord.Banner(User);

                        if (Banner is not null && Banner.Member.Role.Contains(MUTE))
                        {
                            foreach (var Pair in User.Security)
                            {
                                User.Security[Pair.Key] = false;
                            }

                            User.Error = ERROR_ACCESS;
                        }
                    }
                }
                finally
                {
                    User.Background = false;
                }

                await Task.Delay(2500);
            }
        }

        private static async void DoMessage(List<IUser> UserList)
        {
            while (true)
            {
                if (UserList.Any(x => x.Message) && Message > EMessage.DISABLED)
                {
                    var List = UserList
                        .Where(x => x.Message)
                        .ToList();

                    foreach (var User in List)
                    {
                        var Response = await Discord.Message(User);

                        if (Response == null)
                        {
                            User.Message = false;
                        }
                        else
                        {
                            var History = Response
                                .Where(x => x.Type == 1)
                                .ToList();

                            if (History.Count > 0)
                            {
                                if (User.History == null)
                                {
                                    User.History = History
                                        .Select(x => new IUser.IHistory(x.ID, x.LastMessageID))
                                        .ToList();
                                }
                                else
                                {
                                    foreach (var X in History)
                                    {
                                        var T = User.History.FirstOrDefault(x => x.ID == X.ID);

                                        if (T == null)
                                        {
                                            User.History.Add(new IUser.IHistory(X.ID, X.LastMessageID));

                                            string TEXT = $"[{User.Name}] У Вас одно новое сообщение!";

                                            try
                                            {
                                                foreach (var Recipient in X.Recipient)
                                                {
                                                    var Seek = await Discord.Seek(Recipient.ID, User);

                                                    if (Seek == null) continue;

                                                    var Message = Seek.Message
                                                        .SelectMany(x => x)
                                                        .ToList();

                                                    if (Message.Count == 0) continue;

                                                    var Group = Message
                                                        .GroupBy(x => x.ID)
                                                        .Select(x => (x.Key, Date: x.Max(x => x.Date), Count: x.Count()))
                                                        .ToList();

                                                    if (Group.Count == 0) continue;

                                                    var Dictionary = new Dictionary<string, List<Discord.ISeekResponse.IMessage>>();

                                                    foreach (var Pair in Discord.GTA_5_RP)
                                                    {
                                                        foreach ((string Key, DateTime Date, int Count) in Group)
                                                        {
                                                            if (Pair.Value.Contains(Key))
                                                            {
                                                                if (Dictionary.ContainsKey(Pair.Key))
                                                                {
                                                                    Dictionary[Pair.Key].Add(new(Key, Date, Count));
                                                                }
                                                                else
                                                                {
                                                                    Dictionary.Add(Pair.Key, new List<Discord.ISeekResponse.IMessage> { new(Key, Date, Count) });
                                                                }
                                                            }
                                                        }
                                                    }

                                                    if (Dictionary.Count > 0)
                                                    {
                                                        TEXT += Environment.NewLine + string.Join("",
                                                            Dictionary
                                                                .Select(x => (
                                                                    x.Key,
                                                                    Count: x.Value.Sum(v => v.Count),
                                                                    Date: x.Value.Max(v => v.Date)
                                                                ))
                                                                .OrderBy(x => x.Date)
                                                                .Reverse()
                                                                .Select(x => $"{Environment.NewLine}{x.Key} ~ {x.Count} ({x.Date:MM.dd hh:mm})")
                                                                .ToList()
                                                            );
                                                    }
                                                }
                                            }
                                            finally
                                            {
                                                if (Telegram is not null)
                                                {
                                                    await Telegram.SendMessage(TEXT, Message == EMessage.MUTE);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (T.LastMessageID == X.LastMessageID) continue;

                                            var N = Helper.FromUnixTime(X.LastMessageID).Subtract(T.Date);

                                            if (N.TotalSeconds >= 2.5 * 60)
                                            {
                                                T.Update(X.LastMessageID);

                                                if (Telegram is not null)
                                                {
                                                    await Telegram.SendMessage($"[{User.Name}] Вы получили сообщение!", Message == EMessage.MUTE);
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            await Task.Delay(5000);
                        }
                    }

                    await Task.Delay(60 * 1000);
                }
                else
                {
                    await Task.Delay(1000);
                }
            }
        }

        private static async void DoSeparate(List<IUser> UserList)
        {
            var _ = SeparateList.Select(X =>
            {
                return Task.Run(async () =>
                {
                    while (true)
                    {
                        if (!Security.Active)
                        {
                            X.Date = null;
                            X.Position = ISeparate.EPosition.INACTIVE;

                            await Task.Delay(5 * 1000);

                            continue;
                        }

                        if (!X.Go)
                        {
                            X.Date = null;
                            X.Position = ISeparate.EPosition.UNABLE;

                            await Task.Delay(15 * 1000);

                            continue;
                        }

                        if (!UserList.Any(x => x.Security[X.ID]))
                        {
                            X.Date = null;
                            X.Position = ISeparate.EPosition.LIMITED;

                            await Task.Delay(30 * 1000);

                            continue;
                        }

                        var List = UserList
                            .OrderBy(x => x.Time)
                            .ToList();

                        double Average = Helper.Average(List.Count);

                        for (int i = 0; i < List.Count; i++)
                        {
                            if (!Security.Active) break;

                            if (!X.Go) break;

                            if (!List[i].Security[X.ID]) continue;

                            var Read = await Discord.ThreadRead(X, List[i]);

                            if (Read is not null)
                            {
                                var T = Read
                                    .Select((x, i) => (Value: x, Index: i))
                                    .Where(x => List[i].Login == x.Value.Author.Login)
                                    .ToList();

                                if (T.Any())
                                {
                                    if (T.Min(x => x.Index) <= 10)
                                    {
                                        List[i].Time = null;

                                        await Task.Delay(Delay(
                                            X,
                                            Average / 2
                                        ));

                                        continue;
                                    }
                                }
                            }

                            await Task.Delay(2500);

                            var Write = await Discord.ThreadWrite(X, List[i]);

                            if (Write == null)
                            {
                                List[i].Security[X.ID] = false;
                                List[i].Error = 0;
                            }
                            else
                            {
                                if (Write.Code.HasValue)
                                {
                                    if (Write.Code == ERROR_SLOW_MODE)
                                    {
                                        List[i].Time = Math.Ceiling(Write.Retry / 60f);

                                        if (List[i].Time < Average && List.Count - 1 > i)
                                        {
                                            X.Date = null;
                                            X.Position = ISeparate.EPosition.NEXT;

                                            await Task.Delay(30 * 1000);
                                        }
                                        else
                                        {
                                            double N = List.Where(x => x.Time.HasValue).Min(x => x.Time!.Value);

                                            await Task.Delay(Delay(
                                                X,
                                                N > Average
                                                    ? Average
                                                    : N
                                            ));
                                        }
                                    }
                                    else
                                    {
                                        List[i].Security[X.ID] = false;
                                        List[i].Error = Write.Code;

                                        if (Telegram is not null)
                                        {
                                            await Telegram.SendMessage($"[{List[i].Name}] Ошибка: {List[i].Error}!", Message == EMessage.MUTE);
                                        }
                                    }
                                }
                                else
                                {
                                    List[i].Error = null;
                                    List[i].Time = null;

                                    await Task.Delay(Delay(
                                        X,
                                        Average
                                    ));
                                }
                            }
                        }
                    }
                });
            });

            await Task.WhenAll(_);
        }

        private static void DoStorage(bool X = true)
        {
            if (Storage == null) return;

            try
            {
                var Date = Helper.Date();

                foreach (var Pair in Storage.Separate.ToList())
                {
                    if (Pair.Value > Date)
                    {
                        foreach (var Separate in SeparateList)
                        {
                            if (Separate.Car == null) continue;

                            foreach (var Certificate in Separate.Car.List.SelectMany(x => x.Certificate))
                            {
                                if (Certificate.Name == Pair.Key)
                                {
                                    Certificate.DoThread(Pair.Value - Date);
                                }
                            }
                        }
                    }
                    else
                    {
                        Storage.Separate.Remove(Pair.Key);
                    }
                }

                if (X)
                {
                    foreach (var Pair in Storage.Notice.ToList())
                    {
                        if (Pair.Value > Date)
                        {
                            foreach (var Notice in NoticeDictionary)
                            {
                                if (Notice.Key == Pair.Key)
                                {
                                    Notice.Value.Type = INotice.EType.ACTIVE;

                                    while (Notice.Value.Date == null) { }

                                    Notice.Value.Date = Date + (Pair.Value - Date);
                                }
                            }
                        }
                        else
                        {
                            Storage.Notice.Remove(Pair.Key);
                        }
                    }

                    foreach (var Pair in Storage.Compensation.ToList())
                    {
                        if (Date < Pair.Value)
                        {
                            foreach (var Compensation in CompensationList)
                            {
                                if (Compensation.Name == Pair.Key)
                                {
                                    Compensation.Index = Compensation.Count;

                                    Compensation.DoThread(Pair.Value);
                                }
                            }
                        }
                        else
                        {
                            Storage.Compensation.Remove(Pair.Key);
                        }
                    }
                }
            }
            finally
            {
                Storage.Save(StorageFile);
            }
        }

        private static async void DoNotice()
        {
            var _ = NoticeDictionary.Select(X =>
            {
                return Task.Run(async () =>
                {
                    while (true)
                    {
                        if (X.Value.Active)
                        {
                            if (X.Value.Time.HasValue)
                            {
                                X.Value.Date = Helper.Date().Add(X.Value.Time.Value);

                                X.Value.Source = new();

                                if (Storage is not null)
                                {
                                    if (Storage.Notice.TryAdd(X.Key, X.Value.Date.Value))
                                    {
                                        Storage.Save(StorageFile);
                                    }
                                }

                                var Tomorrow = Helper.Tomorrow();

                                if (X.Value.AFTER_RESTART && X.Value.Date > Tomorrow)
                                {
                                    X.Value.Date = Tomorrow;
                                }

                                try
                                {
                                    for (var N = Helper.Date(); N <= X.Value.Date; N = N.AddSeconds(1))
                                    {
                                        if (X.Value.Source.IsCancellationRequested) break;

                                        X.Value.Header = X.Value.Date - N;

                                        await Task.Delay(1000);
                                    }

                                    X.Value.Source.Token.ThrowIfCancellationRequested();

                                    X.Value.Type = INotice.EType.NONE;

                                    switch (X.Value.Notification)
                                    {
                                        case INotice.INotification.BEEP:

                                            await Task.Delay(2500, X.Value.Source.Token);

                                            for (int i = 0; i < 3; i++)
                                            {
                                                if (X.Value.Source.IsCancellationRequested) break;

                                                Console.Beep();

                                                await Task.Delay(1000);
                                            }

                                            break;
                                        case INotice.INotification.MESSAGE when Message > EMessage.DISABLED:

                                            string[] Array = X.Key.Split(" - ");

                                            if (Telegram is not null)
                                            {
                                                await Telegram.SendMessage($"\"{(Regex.IsMatch(Array[0], "Аккаунт #[0-9]+") ? Array.First() + " - " : "")}{Array.Last()}\" доступно!", Message == EMessage.MUTE);
                                            }

                                            break;
                                    }
                                }
                                catch (OperationCanceledException) { }
                                catch (ObjectDisposedException) { }
                                finally
                                {
                                    if (Storage is not null)
                                    {
                                        if (Storage.Notice.ContainsKey(X.Key))
                                        {
                                            Storage.Notice.Remove(X.Key);
                                            Storage.Save(StorageFile);
                                        }
                                    }

                                    X.Value.Source.Dispose();
                                    X.Value.Source = null;
                                }
                            }
                        }
                        else
                        {
                            await Task.Delay(1000);
                        }
                    }
                });
            });

            await Task.WhenAll(_);
        }

        #region Can

        private static ConcurrentDictionary<string, object>? Current;

        private static async Task<bool> Write(string File)
        {
            if (Current == null) return false;

            object T = new();

            Current[File] = T;

            await Task.Delay(1000).ConfigureAwait(false);

            return Current.TryGetValue(File, out var X) && (T == X) && Current.TryRemove(File, out _);
        }

        private async static void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (string.IsNullOrEmpty(e.FullPath) || string.IsNullOrEmpty(e.Name)) return;

            if (await Write(e.Name))
            {
                string? DirectoryName = Path.GetDirectoryName(e.FullPath);

                if (string.IsNullOrEmpty(DirectoryName)) return;

                string? FileName = Path.GetFileName(DirectoryName);

                if (string.IsNullOrEmpty(FileName)) return;

                var Separate = SeparateList
                    .Where(x => x.ID == FileName)
                    .FirstOrDefault(x => x.Car?.Key == e.FullPath);

                if (Separate == null) return;

                var CarList = JsonConvert.DeserializeObject<List<ISeparate.ICar.IList>>(File.ReadAllText(e.FullPath));

                if (CarList == null) return;

                Separate.Car = new(e.FullPath, CarList);

                new Thread(() => DoStorage(false)).Start();
            }
        }


        private async static Task OnCreatedFile(string Name, string FullPath)
        {
            var X = await Write(FullPath);

            if (X)
            {
                if (Path.HasExtension(FullPath))
                {
                    string? DirectoryName = Path.GetDirectoryName(FullPath);

                    if (string.IsNullOrEmpty(DirectoryName)) return;

                    string? FileName = Path.GetFileName(DirectoryName);

                    if (string.IsNullOrEmpty(FileName)) return;

                    var Separate = SeparateList.FirstOrDefault(x => x.ID == FileName);

                    if (Separate == null) return;

                    switch (Path.GetExtension(FullPath).ToUpper())
                    {
                        case ".TXT":

                            Separate.Content = FullPath;

                            break;

                        case ".JSON":

                            var CarList = JsonConvert.DeserializeObject<List<ISeparate.ICar.IList>>(File.ReadAllText(FullPath));

                            if (CarList == null) return;

                            Separate.Car = new(FullPath, CarList);

                            break;

                        default:

                            Separate.Image ??= new();
                            Separate.Image.Add(FullPath);

                            break;
                    }

                    Begin();
                }
                else
                {
                    if (Regex.IsMatch(Name, @"^\d+$"))
                    {
                        var Separate = new ISeparate(Name);

                        SeparateList.Add(Separate);

                        UserList.ForEach(x => x.Security.Add(Separate.ID, true));
                    }
                }
            }
        }

        private async static void OnCreated(object sender, FileSystemEventArgs e)
        {
            if (string.IsNullOrEmpty(e.FullPath) || string.IsNullOrEmpty(e.Name)) return;

            await OnCreatedFile(e.Name, e.FullPath);
        }

        private static async Task OnDeletedFile(string Name, string FullPath)
        {
            if (await Write(FullPath))
            {
                if (Path.HasExtension(FullPath))
                {
                    string? DirectoryName = Path.GetDirectoryName(Name);

                    if (string.IsNullOrEmpty(DirectoryName)) return;

                    var Separate = SeparateList.FirstOrDefault(x => x.ID == DirectoryName);

                    if (Separate == null) return;

                    switch (Path.GetExtension(FullPath).ToUpper())
                    {
                        case ".TXT":

                            if (string.IsNullOrEmpty(Separate.Content)) return;

                            if (Separate.Content == FullPath)
                            {
                                Separate.Content = null;
                            }

                            break;

                        case ".JSON":

                            if (Separate.Car == null) return;

                            if (Separate.Car.Key == FullPath)
                            {
                                Separate.Car = null;
                            }

                            break;

                        default:

                            if (Separate.Image == null || Separate.Image.Count == 0) return;

                            Separate.Image.RemoveAll(Image => Image == FullPath);

                            break;
                    }
                }
                else
                {
                    if (SeparateList.RemoveAll(x => x.ID == Name) > 0)
                    {
                        UserList.ForEach(x => x.Security.Remove(Name));
                    }
                }
            }
        }

        private async static void OnDeleted(object sender, FileSystemEventArgs e)
        {
            if (string.IsNullOrEmpty(e.FullPath) || string.IsNullOrEmpty(e.Name)) return;

            await OnDeletedFile(e.Name, e.FullPath);
        }

        private static async void OnRenamed(object sender, RenamedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.OldName) && !string.IsNullOrEmpty(e.OldFullPath))
            {
                await OnDeletedFile(e.OldName, e.OldFullPath);
            }

            if (!string.IsNullOrEmpty(e.Name) && !string.IsNullOrEmpty(e.FullPath))
            {
                await OnCreatedFile(e.Name, e.FullPath);
            }
        }

        #endregion

        #region Bar

        private static void BAR()
        {
            var Thread = new Thread(() =>
            {
                while (true)
                {
                    if (BAR_DAILY)
                    {
                        var N = CompensationList
                            .Where(x => x.Index == x.Count)
                            .ToList();

                        if (N.Count > 0)
                        {
                            Console.Title = $"$ {N.Sum(x => x.Value)} BP - {N.Sum(x => x.Start + (x.Per * x.Count))}$";
                        }
                    }
                    else
                    {
                        Console.Title = "$ ";
                    }

                    System.Threading.Thread.Sleep(1000);
                }
            });

            Thread.Start();
        }

        #endregion

        private static TimeSpan Delay(ISeparate X, double N)
        {
            var TS = TimeSpan.FromMinutes(N);

            X.Date = Helper.Date() + TS;
            X.Position = ISeparate.EPosition.ACTIVE;

            return TS;
        }
    }
}