using Newtonsoft.Json;
using RestSharp;
using System.Diagnostics;
using System.Net;

namespace GTA_5_RP_Bot
{
    public class Telegram
    {
        private readonly string Token;
        private readonly int ChatID;

        public Telegram(string Token, int ChatID)
        {
            this.Token = Token;
            this.ChatID = ChatID;
        }

        #region Send Message

        public class IMessageResponse
        {
            [JsonProperty("ok", Required = Required.Always)]
            public bool Success { get; private set; }

            [JsonProperty("description", Required = Required.DisallowNull)]
            public string? Description { get; private set; }

            public class IResult
            {
                [JsonProperty("message_id", Required = Required.Always)]
                public int MessageID { get; private set; }
            }

            [JsonProperty("result", Required = Required.DisallowNull)]
            public IResult? Result { get; private set; }
        }

        public async Task<int?> SendMessage(string Text, bool Notification = false)
        {
            var Client = new RestClient(
                new RestClientOptions()
                {
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.159 Safari/537.36",
                    MaxTimeout = 300000
                });

            var Request = new RestRequest($"https://api.telegram.org/bot{Token}/sendMessage");

            Request.AddParameter("chat_id", ChatID);
            Request.AddParameter("text", Text);
            Request.AddParameter("parse_mode", "HTML");
            Request.AddParameter("disable_web_page_preview", true);
            Request.AddParameter("disable_notification", Notification);
            Request.AddParameter("protect_content", true);

            for (byte i = 0; i < 3; i++)
            {
                try
                {
                    var Execute = await Client.ExecutePostAsync(Request);

                    if (string.IsNullOrEmpty(Execute.Content))
                    {
                        if (Execute.StatusCode == 0 || Execute.StatusCode == HttpStatusCode.OK)
                        {
                            Trace.WriteLine("Ответ пуст!");
                        }
                        else
                        {
                            Trace.WriteLine($"Ошибка: {Execute.StatusCode}.");
                        }
                    }
                    else
                    {
                        if (Helper.IsValidJson(Execute.Content))
                        {
                            try
                            {
                                var JSON = JsonConvert.DeserializeObject<IMessageResponse>(Execute.Content);

                                if (JSON == null)
                                {
                                    Trace.WriteLine($"Ошибка: {Execute.Content}.");
                                }
                                else
                                {
                                    if (JSON.Success)
                                    {
                                        if (JSON.Result is not null)
                                        {
                                            return JSON.Result.MessageID;
                                        }
                                    }
                                    else
                                    {
                                        Trace.WriteLine($"Ошибка: {JSON.Description}");
                                    }
                                }

                                break;
                            }
                            catch (Exception e)
                            {
                                Trace.WriteLine(e);
                            }
                        }
                        else
                        {
                            Trace.WriteLine($"Ошибка: {Execute.Content}");
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
