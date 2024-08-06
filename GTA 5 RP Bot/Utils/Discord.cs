using Newtonsoft.Json;
using RestSharp;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;

namespace GTA_5_RP_Bot
{
    public class Discord
    {
        private const string GUILD_ID = "608610149569134619";

#pragma warning disable CA2211 // Non-constant fields should not be visible

        public static Dictionary<string, List<string>> GTA_5_RP = new()
        {
            {
                "🌆 DOWNTOWN", new List<string>
                {
                  "652635581104259093",
                  "652635912437760003",
                  "652636987563573249",
                  "655093042457477130",
                  "655095105748860970",
                  "668563512649580572",
                  "668563838869700646",
                  "720436413870374922",
                  "1001780477591556237"
                }
            },
            {
                "🍓 STRAWBERRY", new List<string>
                {
                  "652634813576118335",
                  "652635708481339447",
                  "652635762033950732",
                  "652636170722869258",
                  "652636664598102016",
                  "668563725980008468",
                  "668564034152300559",
                  "720689643003314316",
                  "1001780836565254195"
                }
            },
            {
                "🏰 VINEWOOD", new List<string>
                {
                  "670497246646763568",
                  "670499660644745231",
                  "670499688520351759",
                  "670499751921451008",
                  "670500728665669632",
                  "670500753646944267",
                  "670500780536627210",
                  "720689672682209364",
                  "1001781120662245396"
                }
            },
            {
                "🍇 BLACKBERRY", new List<string>
                {
                  "675341237368258610",
                  "675341282456895538",
                  "675341313192886294",
                  "675341353374449714",
                  "675341398236594187",
                  "675341437336027166",
                  "675341467400536092",
                  "720689698498019389",
                  "1001781745261232178"
                }
            },
            {
                "🎭 INSQUAD", new List<string>
                {
                  "680543558704693318",
                  "680543604904689684",
                  "680543636374421579",
                  "680543676509978707",
                  "680543703034626093",
                  "680543750006898718",
                  "680543779786457196",
                  "720689726272831498",
                  "1001782007761748078"
                }
            },
            {
                "🌅 SUNRISE", new List<string>
                {
                  "688097779746340918",
                  "688097801799729250",
                  "688097821831987223",
                  "688097839427092595",
                  "688097863108001859",
                  "688097916988031094",
                  "688097958465372177",
                  "720689748565295224",
                  "1001782329552928858"
                }
            },
            {
                "🌈 RAINBOW", new List<string>
                {
                  "698165014627877038",
                  "698165035687477259",
                  "698165056617316362",
                  "698165079417290793",
                  "698165120475594783",
                  "698165138057986128",
                  "698165157242601574",
                  "720689794526740620",
                  "1001782390642987018"
                }
            },
            {
                "🤵 RICHMAN", new List<string>
                {
                  "702546541650378802",
                  "702546560747307038",
                  "702546599200686080",
                  "702546631882571776",
                  "702546659984408586",
                  "702546678691135620",
                  "702546728284455044",
                  "720689816550899723",
                  "1001782519928205353"
                }
            },
            {
                "🌘 ECLIPSE", new List<string>
                {
                  "713466163585351791",
                  "713466197458550866",
                  "713466225220386846",
                  "713466268560392222",
                  "713466300860465192",
                  "713466334343725106",
                  "713466362202292285",
                  "720689842593464400",
                  "1001782792121757706"
                }
            },
            {
                "🌵 LAMESA", new List<string>
                {
                  "715236162855501824",
                  "715236228953538580",
                  "715236296699936840",
                  "715236361593946113",
                  "715236430472937513",
                  "715236482209546340",
                  "715236539420114975",
                  "720689869256654859",
                  "1001782857775198279"
                }
            },
            {
                "🏛 BURTON", new List<string>
                {
                  "901059232806367243",
                  "901059441246482442",
                  "901059502873378856",
                  "901059541783965716",
                  "901059577783648296",
                  "901059635501465640",
                  "901059676559511572",
                  "901059715696574465",
                  "1001783126223228958"
                }
            },
            {
                "💎 ROCKFORD", new List<string>
                {
                  "908945969326997524",
                  "908946123442520115",
                  "908946445380513792",
                  "908946483263467520",
                  "908946541564272670",
                  "908946592105656340",
                  "908946644949663866",
                  "908946725379657748",
                  "1001783710674325544"
                }
            },
            {
                "🍀 ALTA", new List<string>
                {
                  "941038896932876299",
                  "941039018831933500",
                  "941039254023323668",
                  "941039291302285363",
                  "941039327725649930",
                  "941039364731981834",
                  "941039406494670858",
                  "941039435737354321",
                  "1001784263546507304"
                }
            },
            {
                "🎡 DEL PERRO", new List<string>
                {
                  "1043074463576637491",
                  "1043074522158477312",
                  "1043074559672328254",
                  "1043074598591270943",
                  "1043074634586783744",
                  "1043074668644532224",
                  "1043074709853577247",
                  "1043074744196534373",
                  "1043074779063799808"
                }
            },
            {
                "🏀 DAVIS", new List<string>
                {
                  "1055790485748334643",
                  "1055790595534229506",
                  "1055790643328319519",
                  "1055790679047012362",
                  "1055790730574037023",
                  "1055790775058837514",
                  "1055790819455545385",
                  "1055790850094944296",
                  "1055790878414880879"
                }
            },
            {
                "🌸 HARMONY", new List<string>
                {
                  "1088041853573681283",
                  "1088041966144598028",
                  "1088042007697567785",
                  "1088042041096810526",
                  "1088042117928071199",
                  "1088042160215031868",
                  "1088042194721579048",
                  "1088042229198766110",
                  "1088042270751731712"
                }
            },
            {
                "🌲 REDWOOD", new List<string>
                {
                  "1121379089354330142",
                  "1121650742009266176",
                  "1121650890189836289",
                  "1121650995886305371",
                  "1121651071429910568",
                  "1121651300426317944",
                  "1121651525140353224",
                  "1121651612570636298",
                  "1121651692660858880"
                }
            }
        };

#pragma warning restore CA2211 // Non-constant fields should not be visible

        #region User

        public class IMeResponse
        {
            [JsonProperty("id", Required = Required.DisallowNull)]
            public string? ID { get; private set; }
            
            [JsonProperty("username", Required = Required.DisallowNull)]
            public string? Login { get; private set; }

            [JsonProperty("global_name", Required = Required.DisallowNull)]
            public string? Name { get; private set; }
        }

        public static async Task<IMeResponse?> Me(Program.IUser User)
        {
            var Client = new RestClient(
                new RestClientOptions()
                {
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.159 Safari/537.36",
                    MaxTimeout = 300000
                });

            var Request = new RestRequest("https://discord.com/api/v9/users/@me");

            Request.AddHeader("Authorization", User.Token);

            for (byte i = 0; i < 3; i++)
            {
                try
                {
                    var Execute = await Client.ExecuteGetAsync(Request);

                    if (string.IsNullOrEmpty(Execute.Content))
                    {
                        if (Execute.StatusCode == 0 || Execute.StatusCode == HttpStatusCode.OK)
                        {
                            Trace.WriteLine("USER | Ответ пуст!");
                        }
                        else
                        {
                            Trace.WriteLine($"USER | Ошибка: {Execute.StatusCode}.");
                        }
                    }
                    else
                    {
                        if (Helper.IsValidJson(Execute.Content))
                        {
                            try
                            {
                                var JSON = JsonConvert.DeserializeObject<IMeResponse>(Execute.Content);

                                if (JSON == null)
                                {
                                    Trace.WriteLine($"USER | Ошибка: {Execute.Content}.");
                                }

                                return JSON;
                            }
                            catch (Exception e)
                            {
                                Trace.WriteLine(e);
                            }
                        }
                        else
                        {
                            Trace.WriteLine($"USER | Ошибка: {Execute.Content}");
                        }
                    }

                    await Task.Delay(2500);
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                }
            }

            return null;
        }

        public class IBannerResponse
        {
            [JsonProperty("guild_member", Required = Required.Always)]
            public IMember Member { get; private set; } = new();

            public class IMember
            {
                [JsonProperty("roles", Required = Required.Always)]
                public List<string> Role { get; private set; } = new();
            }
        }

        public static async Task<IBannerResponse?> Banner(Program.IUser User)
        {
            var Client = new RestClient(
                new RestClientOptions()
                {
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.159 Safari/537.36",
                    MaxTimeout = 300000
                });

            var Request = new RestRequest($"https://discord.com/api/v9/users/{User.ID}/profile?with_mutual_guilds=true&with_mutual_friends=true&with_mutual_friends_count=false&guild_id={GUILD_ID}");

            Request.AddHeader("Authorization", User.Token);

            for (byte i = 0; i < 3; i++)
            {
                try
                {
                    var Execute = await Client.ExecuteGetAsync(Request);

                    if (string.IsNullOrEmpty(Execute.Content))
                    {
                        if (Execute.StatusCode == 0 || Execute.StatusCode == HttpStatusCode.OK)
                        {
                            Trace.WriteLine("BANNER | Ответ пуст!");
                        }
                        else
                        {
                            Trace.WriteLine($"BANNER | Ошибка: {Execute.StatusCode}.");
                        }
                    }
                    else
                    {
                        if (Helper.IsValidJson(Execute.Content))
                        {
                            try
                            {
                                var JSON = JsonConvert.DeserializeObject<IBannerResponse>(Execute.Content);

                                if (JSON == null)
                                {
                                    Trace.WriteLine($"BANNER | Ошибка: {Execute.Content}.");
                                }

                                return JSON;
                            }
                            catch (Exception e)
                            {
                                Trace.WriteLine(e);
                            }
                        }
                        else
                        {
                            Trace.WriteLine($"BANNER | Ошибка: {Execute.Content}");
                        }
                    }

                    await Task.Delay(2500);
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                }
            }

            return null;
        }

        #endregion

        #region Message

        public class IMessageResponse
        {
            [JsonProperty("id", Required = Required.Always)]
            public string ID { get; private set; } = "";

            [JsonProperty("type", Required = Required.Always)]
            public int Type { get; private set; }

            [JsonProperty("last_message_id", Required = Required.Always)]
            public long LastMessageID { get; private set; }

            [JsonProperty("recipients", Required = Required.Always)]
            public List<IRecipient> Recipient { get; private set; } = new();

            public class IRecipient
            {
                [JsonProperty("id", Required = Required.Always)]
                public string ID { get; private set; } = "";
            }
        }

        public static async Task<List<IMessageResponse>?> Message(Program.IUser User)
        {
            var Client = new RestClient(
                new RestClientOptions()
                {
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.159 Safari/537.36",
                    MaxTimeout = 300000
                });

            var Request = new RestRequest("https://discord.com/api/v9/users/@me/channels");

            Request.AddHeader("Authorization", User.Token);

            for (byte i = 0; i < 3; i++)
            {
                try
                {
                    var Execute = await Client.ExecuteGetAsync(Request);

                    if (string.IsNullOrEmpty(Execute.Content))
                    {
                        if (Execute.StatusCode == 0 || Execute.StatusCode == HttpStatusCode.OK)
                        {
                            Trace.WriteLine("MESSAGE | Ответ пуст!");
                        }
                        else
                        {
                            Trace.WriteLine($"DIRECT | Ошибка: {Execute.StatusCode}.");
                        }
                    }
                    else
                    {
                        if (Helper.IsValidJson(Execute.Content))
                        {
                            try
                            {
                                var JSON = JsonConvert.DeserializeObject<List<IMessageResponse>>(Execute.Content);

                                if (JSON == null)
                                {
                                    Trace.WriteLine($"MESSAGE | Ошибка: {Execute.Content}.");
                                }

                                return JSON;
                            }
                            catch (Exception e)
                            {
                                Trace.WriteLine(e);
                            }
                        }
                        else
                        {
                            Trace.WriteLine($"MESSAGE | Ошибка: {Execute.Content}");
                        }
                    }

                    await Task.Delay(2500);
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                }
            }

            return null;
        }

        #endregion

        #region Seek

        public class ISeekResponse
        {
            [JsonProperty("messages", Required = Required.Always)]
            public List<List<IMessage>> Message { get; private set; } = new();

            public class IMessage
            {
                [JsonProperty("channel_id", Required = Required.Always)]
                public string ID { get; private set; } = "";

                [JsonProperty("timestamp", Required = Required.Always)]
                public DateTime Date { get; private set; }

                [JsonIgnore]
                public int Count { get; private set; }

                public IMessage(string ID, DateTime Date, int Count)
                {
                    this.ID = ID;
                    this.Date = Date;
                    this.Count = Count;
                }
            }
        }

        public static async Task<ISeekResponse?> Seek(string ID, Program.IUser User)
        {
            var Client = new RestClient(
                new RestClientOptions()
                {
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.159 Safari/537.36",
                    MaxTimeout = 300000
                });

            var Request = new RestRequest($"https://discord.com/api/v9/guilds/{GUILD_ID}/messages/search?author_id={ID}");

            Request.AddHeader("Authorization", User.Token);

            for (byte i = 0; i < 3; i++)
            {
                try
                {
                    var Execute = await Client.ExecuteGetAsync(Request);

                    if (string.IsNullOrEmpty(Execute.Content))
                    {
                        if (Execute.StatusCode == 0 || Execute.StatusCode == HttpStatusCode.OK)
                        {
                            Trace.WriteLine("GUILD | Ответ пуст!");
                        }
                        else
                        {
                            Trace.WriteLine($"GUILD | Ошибка: {Execute.StatusCode}.");
                        }
                    }
                    else
                    {
                        if (Helper.IsValidJson(Execute.Content))
                        {
                            try
                            {
                                var JSON = JsonConvert.DeserializeObject<ISeekResponse>(Execute.Content);

                                if (JSON == null)
                                {
                                    Trace.WriteLine($"GUILD | Ошибка: {Execute.Content}.");
                                }

                                return JSON;
                            }
                            catch (Exception e)
                            {
                                Trace.WriteLine(e);
                            }
                        }
                        else
                        {
                            Trace.WriteLine($"GUILD | Ошибка: {Execute.Content}");
                        }
                    }

                    await Task.Delay(2500);
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                }
            }

            return null;
        }

        #endregion

        #region Thread

        public class IThreadWriteResponse
        {
            [JsonProperty("code", Required = Required.DisallowNull)]
            public uint? Code { get; private set; }

            [JsonProperty("retry_after", Required = Required.DisallowNull)]
            public float Retry { get; private set; }
        }

        public static async Task<IThreadWriteResponse?> ThreadWrite(Program.ISeparate Thread, Program.IUser User)
        {
            var Client = new RestClient(
                new RestClientOptions()
                {
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.159 Safari/537.36",
                    MaxTimeout = 300000
                });

            var Request = new RestRequest($"https://discord.com/api/v9/channels/{Thread.ID}/messages");

            var C = Thread.GatherContent();

            if (C is not null)
            {
                Request.AddParameter("content", C);
            }

            var I = Thread.GatherImage();

            if (I is not null)
            {
                for (int i = 0; i < I.Count; i++)
                {
                    Request.AddFile($"image[{i}]", I[i]);
                }
            }

            Request.AddHeader("Authorization", User.Token);

            for (byte i = 0; i < 3; i++)
            {
                try
                {
                    var Execute = await Client.ExecutePostAsync(Request);

                    if (string.IsNullOrEmpty(Execute.Content))
                    {
                        if (Execute.StatusCode == 0 || Execute.StatusCode == HttpStatusCode.OK)
                        {
                            Trace.WriteLine("WRITE | Ответ пуст!");
                        }
                        else
                        {
                            Trace.WriteLine($"WRITE | Ошибка: {Execute.StatusCode}.");
                        }
                    }
                    else
                    {
                        if (Helper.IsValidJson(Execute.Content))
                        {
                            try
                            {
                                var JSON = JsonConvert.DeserializeObject<IThreadWriteResponse>(Execute.Content);

                                if (JSON == null)
                                {
                                    Trace.WriteLine($"WRITE | Ошибка: {Execute.Content}.");
                                }

                                return JSON;
                            }
                            catch (Exception e)
                            {
                                Trace.WriteLine(e);
                            }
                        }
                        else
                        {
                            Trace.WriteLine($"WRITE | Ошибка: {Execute.Content}");
                        }
                    }

                    await Task.Delay(2500);
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                }
            }

            return null;
        }

        public class IThreadReadResponse
        {
            public class IAuthor
            {
                [JsonProperty("username", Required = Required.Always)]
                public string Login { get; private set; } = "";
            }

            [JsonProperty("author", Required = Required.Always)]
            public IAuthor Author { get; private set; } = new();

            [JsonProperty("timestamp", Required = Required.Always)]
            public DateTime Date { get; private set; }
        }

        public static async Task<List<IThreadReadResponse>?> ThreadRead(Program.ISeparate Thread, Program.IUser User)
        {
            var Client = new RestClient(
                new RestClientOptions()
                {
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.159 Safari/537.36",
                    MaxTimeout = 300000
                });

            var Request = new RestRequest($"https://discord.com/api/v9/channels/{Thread.ID}/messages");

            Request.AddHeader("Authorization", User.Token);

            for (byte i = 0; i < 3; i++)
            {
                try
                {
                    var Execute = await Client.ExecuteGetAsync(Request);

                    if (string.IsNullOrEmpty(Execute.Content))
                    {
                        if (Execute.StatusCode == 0 || Execute.StatusCode == HttpStatusCode.OK)
                        {
                            Trace.WriteLine("READ | Ответ пуст!");
                        }
                        else
                        {
                            Trace.WriteLine($"READ | Ошибка: {Execute.StatusCode}.");
                        }
                    }
                    else
                    {
                        if (Helper.IsValidJson(Execute.Content))
                        {
                            try
                            {
                                var JSON = JsonConvert.DeserializeObject<List<IThreadReadResponse>>(Execute.Content);

                                if (JSON == null)
                                {
                                    Trace.WriteLine($"READ | Ошибка: {Execute.Content}.");
                                }

                                return JSON;
                            }
                            catch (Exception e)
                            {
                                Trace.WriteLine(e);
                            }
                        }
                        else
                        {
                            Trace.WriteLine($"READ | Ошибка: {Execute.Content}");
                        }
                    }

                    await Task.Delay(2500);
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                }
            }

            return null;
        }

        #endregion
    }
}
