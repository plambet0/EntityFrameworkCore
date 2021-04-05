using Newtonsoft.Json;

namespace BookShop.DataProcessor.ImportDto
{
    public class InportJsonBookInputModel
    {
        [JsonProperty("Id")]
        public int? BookId { get; set; }
    }
}