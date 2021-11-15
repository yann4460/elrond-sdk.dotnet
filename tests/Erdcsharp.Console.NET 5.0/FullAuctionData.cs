using Newtonsoft.Json;

namespace Elrond.SDK.Console
{
    public class FullAuctionData
    {
        [JsonProperty("payment_token")]  public PaymentToken PaymentToken  { get; set; }
        [JsonProperty("min_bid")]        public string       MinBid        { get; set; }
        [JsonProperty("max_bid")]        public string       MaxBid        { get; set; }
        [JsonProperty("deadline")]       public string       Deadline      { get; set; }
        [JsonProperty("original_owner")] public string       OriginalOwner { get; set; }
        [JsonProperty("current_bid")]    public string       CurrentBid    { get; set; }
        [JsonProperty("current_winner")] public string       CurrentWinner { get; set; }

        [JsonProperty("marketplace_cut_percentage")]
        public string MarketplaceCutPercentage { get; set; }

        [JsonProperty("creator_royalties_percentage")]
        public string CreatorRoyaltiesPercentage { get; set; }
    }

    public class PaymentToken
    {
        [JsonProperty("token_type")] public string TokenType { get; set; }
        [JsonProperty("nonce")]      public string Nonce     { get; set; }
    }
}
