namespace Erdcsharp.Domain
{
    public static class Constants
    {
        public const string ArwenVirtualMachine = "0500";

        /// <summary>
        /// Human-Readable Part
        /// </summary>
        public const string Hrp = "erd";

        /// <summary>
        /// eGold ticker
        /// </summary>
        public const string EGLD = "EGLD";

        public static class SmartContractAddress
        {
            public const string EsdtSmartContract = "erd1qqqqqqqqqqqqqqqpqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqzllls8a5w6u";
        }

        public static class EsdtNftSpecialRoles
        {
            /// <summary>
            /// This role allows one to create a new NFT
            /// </summary>
            public const string EsdtRoleNftCreate = "ESDTRoleNFTCreate";

            /// <summary>
            /// This role allows one to burn quantity of a specific NFT
            /// </summary>
            public const string EsdtRoleNftBurn = "ESDTRoleNFTBurn";
        }

        public static class EsdtSftSpecialRoles
        {
            /// <summary>
            /// This role allows one to create a new SemiFungible
            /// </summary>
            public const string EsdtRoleNftCreate = "ESDTRoleNFTCreate";

            /// <summary>
            /// This role allows one to burn quantity of a specific SemiFungible
            /// </summary>
            public const string EsdtRoleNftBurn = "ESDTRoleNFTBurn";

            /// <summary>
            /// This role allows one to add quantity of a specific SemiFungible
            /// </summary>
            public const string EsdtRoleNftAddQuantity = "ESDTRoleNFTAddQuantity";
        }
    }
}
