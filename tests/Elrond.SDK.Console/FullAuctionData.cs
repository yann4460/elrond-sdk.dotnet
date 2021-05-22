
public class FullAuctionData
{
    public Payment_Token payment_token { get; set; }
    public string min_bid { get; set; }
    public string max_bid { get; set; }
    public string deadline { get; set; }
    public string original_owner { get; set; }
    public string current_bid { get; set; }
    public string current_winner { get; set; }
    public string marketplace_cut_percentage { get; set; }
    public string creator_royalties_percentage { get; set; }
}

public class Payment_Token
{
    public string token_type { get; set; }
    public string nonce { get; set; }
}
