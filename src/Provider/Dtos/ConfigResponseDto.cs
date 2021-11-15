namespace Erdcsharp.Provider.Dtos
{
    public class ConfigDataDto
    {
        public ConfigDto Config { get; set; }
    }

    public class ConfigDto
    {
        public string erd_chain_id                      { get; set; }
        public int    erd_denomination                  { get; set; }
        public int    erd_gas_per_data_byte             { get; set; }
        public string erd_gas_price_modifier            { get; set; }
        public string erd_latest_tag_software_version   { get; set; }
        public int    erd_meta_consensus_group_size     { get; set; }
        public int    erd_min_gas_limit                 { get; set; }
        public int    erd_min_gas_price                 { get; set; }
        public int    erd_min_transaction_version       { get; set; }
        public int    erd_num_metachain_nodes           { get; set; }
        public int    erd_num_nodes_in_shard            { get; set; }
        public int    erd_num_shards_without_meta       { get; set; }
        public string erd_rewards_top_up_gradient_point { get; set; }
        public int    erd_round_duration                { get; set; }
        public int    erd_rounds_per_epoch              { get; set; }
        public int    erd_shard_consensus_group_size    { get; set; }
        public int    erd_start_time                    { get; set; }
        public string erd_top_up_factor                 { get; set; }
    }
}
