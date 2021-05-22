using System.Text.Json.Serialization;

namespace Elrond.SDK.Console
{
    public class FullAuctionData
    {
        [JsonPropertyName("payment_token")]
        public PaymentToken PaymentToken { get; set; }
        [JsonPropertyName("min_bid")]
        public string MinBid { get; set; }
        [JsonPropertyName("max_bid")]
        public string MaxBid { get; set; }
        [JsonPropertyName("deadline")]
        public string Deadline { get; set; }
        [JsonPropertyName("originalOwner")]
        public string OriginalOwner { get; set; }
        [JsonPropertyName("current_bid")]
        public string CurrentBid { get; set; }
        [JsonPropertyName("current_winner")]
        public string CurrentWinner { get; set; }
        [JsonPropertyName("marketplace_cut_percentage")]
        public string MarketplaceCutPercentage { get; set; }
        [JsonPropertyName("creator_royalties_percentage")]
        public string CreatorRoyaltiesPercentage { get; set; }
    }

    public class PaymentToken
    {
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }
        [JsonPropertyName("nonce")]
        public string Nonce { get; set; }
    }
}