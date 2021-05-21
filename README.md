# elrond-sdk.dotnet
Elronford SDK for .NET Core.

# What is Elrond SDK for .NET Core  ?
This is the .Net integration library for Elrond, simplifying the access and smart contract interaction with Elrond blockchain. It is developed targeting net5.0 only for now.

### Under development, stay tuned!

# Features
* Transaction construction, signing, broadcasting and querying.
* Smart Contracts deployment and interaction (execution and querying).

# Quick documentations
## Synchronizing network parameters
```csharp
        private static async Task SynchronizingNetworkParameter()
        {
            var client = new HttpClient { BaseAddress = new Uri("https://testnet-gateway.elrond.com") };
            var provider = new ElrondProvider(client);
            var constants = await Constants.GetFromNetwork(provider);
            System.Console.WriteLine("MinGasPrice {0}", constants.MinGasPrice);
            System.Console.WriteLine("ChainId {0}", constants.ChainId);
            System.Console.WriteLine("GasPerDataByte {0}", constants.GasPerDataByte);
        }
```
## Synchronizing an account object
```csharp
        private static async Task SynchronizingAnAccountObject(IElrondProvider provider)
        {
            var address = AddressValue.FromBech32("erd1spyavw0956vq68xj8y4tenjpq2wd5a9p2c6j8gsz7ztyrnpxrruqzu66jx");
            var account = new Account(address);
            await account.Sync(provider);

            System.Console.WriteLine("Balance {0}", account.Balance);
            System.Console.WriteLine("Nonce {0}", account.Nonce);
        }
```
# Change Log
All notable changes will be documented in this file.

## [1.0.8] - 21.05.2021
-   [Add smart contract query option.](https://github.com/yann4460/elrond-sdk.dotnet/pull/9) 

## [1.0.7] - 20.05.2021
-   [Add  GetSmartContractResult method.](https://github.com/yann4460/elrond-sdk.dotnet/pull/8) 
    - `var getSumResult = getSumTransaction.GetSmartContractResult("getSum", abi);`

### [1.0.6] - 20.05.2021
-   Remove Argument class. Prefer the use of IBinaryType
-   Ex : `Argument.CreateArgumentFromInt64(12)` is ow obsolete. Use : `NumericValue.BigIntValue(12)` instead.
    - Build a balance argument : `NumericValue.BigUintValue(Balance.EGLD("10").Value)`
    - Build from a byte array : `BytesValue.FromBuffer(new byte[] {0x00});`

### [1.0.5] - 20.05.2021
-   [Add mutliples codec](https://github.com/yann4460/elrond-sdk.dotnet/pull/5)

### [1.0.4] - 18.05.2021
-   [Compute GasLimit for transfer #4](https://github.com/yann4460/elrond-sdk.dotnet/pull/4)
