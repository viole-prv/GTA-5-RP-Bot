using Newtonsoft.Json;

namespace GTA_5_RP_Bot
{
    public class IStorage
    {
        [JsonIgnore]
        private static string? File { get; set; }

        private static readonly SemaphoreSlim Semaphore = new(1, 1);

        [JsonProperty("Список")]
        public Dictionary<string, DateTimeOffset> Separate { get; set; } = new();

        [JsonProperty("Уведомления")]
        public Dictionary<string, DateTimeOffset> Notice { get; set; } = new();

        public static bool ShouldSerializeNotice() => Program.Unique;

        [JsonProperty("Вознаграждение")]
        public Dictionary<string, DateTimeOffset> Compensation { get; set; } = new();

        public static bool ShouldSerializeCompensation() => Program.Unique;

        public static (string? ErrorMessage, IStorage? Config) Load(string _File)
        {
            File = _File;

            if (!string.IsNullOrEmpty(File) && !System.IO.File.Exists(File))
            {
                System.IO.File.WriteAllText(File, JsonConvert.SerializeObject(new IStorage(), Formatting.Indented));
            }

            string Json;

            try
            {
                Json = System.IO.File.ReadAllText(File);
            }
            catch (Exception e)
            {
                return (e.Message, null);
            }

            if (string.IsNullOrEmpty(Json) || Json.Length == 0)
            {
                return ("Данные равны нулю!", null);
            }

            IStorage Storage;

            try
            {
                Storage = JsonConvert.DeserializeObject<IStorage>(Json)!;
            }
            catch (Exception e)
            {
                return (e.Message, null);
            }

            if (Storage == null)
            {
                return ("Место хранения равен нулю!", null);
            }

            return (null, Storage);
        }

        public async void Save(string File)
        {
            if (string.IsNullOrEmpty(File) || (this == null)) return;

            string JSON = JsonConvert.SerializeObject(this, Formatting.Indented);
            string _ = File + ".new";

            await Semaphore.WaitAsync();

            try
            {
                System.IO.File.WriteAllText(_, JSON);

                if (System.IO.File.Exists(File))
                {
                    System.IO.File.Replace(_, File, null);
                }
                else
                {
                    System.IO.File.Move(_, File);
                }
            }
            finally
            {
                Semaphore.Release();
            }
        }
    }
}
