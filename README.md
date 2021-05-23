# Elronford SDK for .NET Core 
[![Build status](https://github.com/yann4460/elrond-sdk.dotnet/actions/workflows/dotnet.yml/badge.svg)](httpshttps://github.com/yann4460/elrond-sdk.dotnet/actions/workflows/dotnet.yml)  

## How to install ? 
Elronford SDK for .NET Core is delivered via nuget package, therefore it can be installed as follows:
`Install-Package elrond-sdk.dotnet`

[![Package](https://img.shields.io/nuget/v/elrond-sdk.dotnet)](https://www.nuget.org/packages/elrond-sdk.dotnet/)

# What is Elrond SDK for .NET Core  ?
This is the .Net integration library for Elrond, simplifying the access and smart contract interaction with Elrond blockchain. It is developed targeting net5.0 only for now.

# Features
* Transaction construction, signing, broadcasting and querying.
* Smart Contracts deployment and interaction (execution and querying).
* Wallet creation, derive wallet from mnemonic
* Query values stored within Smart Contracts.

# Quick documentations
## Synchronizing network parameters
```csharp
Task SynchronizingNetworkParameter()
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
Task SynchronizingAnAccountObject(IElrondProvider provider)
{
    var address = AddressValue.FromBech32("erd1spyavw0956vq68xj8y4tenjpq2wd5a9p2c6j8gsz7ztyrnpxrruqzu66jx");
    var account = new Account(address);
    await account.Sync(provider);

    System.Console.WriteLine("Balance {0}", account.Balance);
    System.Console.WriteLine("Nonce {0}", account.Nonce);
}
```

## Creating value-transfer transactions
```csharp
Task CreatingValueTransferTransactions(IElrondProvider provider, Constants constants, Wallet wallet)
{
    var sender = wallet.GetAccount();
    var receiver = AddressValue.FromBech32("erd1qyu5wthldzr8wx5c9ucg8kjagg0jfs53s8nr3zpz3hypefsdd8ssycr6th");
    await sender.Sync(provider);

    var transaction = TransactionRequest.CreateTransaction(sender, constants, receiver, Balance.EGLD("2.15"));
    
    transaction.SetData("Hello world !");
    transaction.SetGasLimit(GasLimit.ForTransfer(constants, transaction));

    var transactionResult = await transaction.Send(wallet, provider);
    System.Console.WriteLine("TxHash {0}", transactionResult.TxHash);
}
```

## Smart Contract transactions
#### Deploy a smart contract
Deploy a smart contract from a WASM file.
```csharp
 Task<AddressValue> DeploySmartContract(IElrondProvider provider, Constants constants, Wallet wallet,
    Account account, string filePath)
{
    await account.Sync(provider);
    
    var wasmFile = await Code.FromFilePath(filePath);
    var deployRequest = SmartContract.CreateDeploySmartContractTransactionRequest(constants, account, wasmFile,
        new CodeMetadata(false, true, false),
        new IBinaryType[]
        {
            NumericValue.BigIntValue(5)
        });

    deployRequest.SetGasLimit(new GasLimit(60000000));

    // Get the deployed smart contract address based on account address and transaction nonce
    var smartContractAddress = SmartContract.ComputeAddress(account.Address, account.Nonce);
    var deployTransaction = await deployRequest.Send(wallet, provider);

    await deployTransaction.WaitForExecution(provider);
    deployTransaction.EnsureTransactionSuccess();

    return smartContractAddress;
}
```
#### Create a query smart contract transaction. 
This allows one to create a smart contract transaction call and get the result.
```csharp
Task QuerySmartContract(IElrondProvider provider, Constants constants, Wallet wallet, AddressValue scAddress)
{
    var account = wallet.GetAccount();
    await account.Sync(provider);

    var queryTransaction = SmartContract.CreateCallSmartContractTransactionRequest(constants, account,
        scAddress,
        "getSum",
        Balance.Zero());

    queryTransaction.SetGasLimit(await GasLimit.ForTransaction(queryTransaction, provider));
    
    var transaction = await queryTransaction.Send(wallet, provider);
    await transaction.WaitForExecution(provider);

    // Set the type value according to the ABI description (BigInt)
    var result = transaction.GetSmartContractResult(new[] {TypeValue.BigIntTypeValue});
    var numericResult = result[0].ValueOf<NumericValue>().Number;
}
```

#### Query a smart contract with ABI Definition.
This allows one to execute - with no side-effects - a pure function of a Smart Contract and retrieve the execution results (the Virtual Machine Output).
```csharp
Task QuerySmartContractWithAbi(IElrondProvider provider, AddressValue scAddress)
{
    var abiDefinition = await AbiDefinition.FromJsonFilePath("SmartContracts/auction/auction.abi.json");
    var getFullAuctionData = await SmartContract.QuerySmartContractWithAbiDefinition(scAddress, "getFullAuctionData",
        new IBinaryType[]
        {
            TokenIdentifierValue.From("TSTKR-209ea0"),
            NumericValue.U64Value(3),
        },
        abiDefinition, provider);

    // Need to use the value define in the ABI file (Here it's a StructValue)
    var optFullAuctionData = getFullAuctionData[0].ValueOf<OptionValue>();
    if (optFullAuctionData.IsSet())
    {
        var fullAuctionData = optFullAuctionData.Value.ValueOf<StructValue>().Fields;
    }

    var getDeadline = await SmartContract.QuerySmartContractWithAbiDefinition(scAddress, "getDeadline",
        new IBinaryType[]
        {
            TokenIdentifierValue.From("TSTKR-209ea0"),
            NumericValue.U64Value(3),
        },
        abiDefinition, provider);

    // Need to use the value define in the ABI file (Here it's a StructValue)
    var optDeadline = getDeadline[0].ValueOf<OptionValue>();
    if (optDeadline.IsSet())
    {
        var deadline = optDeadline.Value.ValueOf<NumericValue>().Number;
    }
}
```

#### Query a smart contract without ABI Definition
This allows one to execute - with no side-effects - a pure function of a Smart Contract and retrieve the execution results (the Virtual Machine Output).
You need to manually define the TypeValue definition that will be use by the codec.
```csharp
Task QuerySmartContractWithoutAbi(IElrondProvider provider, AddressValue scAddress)
{
    var esdtToken = TypeValue.StructValue("EsdtToken", new[]
    {
        new FieldDefinition("token_type", "", TypeValue.TokenIdentifierValue),
        new FieldDefinition("nonce", "", TypeValue.U64TypeValue)
    });

    var auction = TypeValue.StructValue("Auction", new[]
    {
        new FieldDefinition("payment_token", "", esdtToken),
        new FieldDefinition("min_bid", "", TypeValue.BigUintTypeValue),
        new FieldDefinition("max_bid", "", TypeValue.BigUintTypeValue),
        new FieldDefinition("deadline", "", TypeValue.U64TypeValue),
        new FieldDefinition("original_owner", "", TypeValue.AddressValue),
        new FieldDefinition("current_bid", "", TypeValue.BigUintTypeValue),
        new FieldDefinition("current_winner", "", TypeValue.AddressValue),
        new FieldDefinition("marketplace_cut_percentage", "", TypeValue.BigUintTypeValue),
        new FieldDefinition("creator_royalties_percentage", "", TypeValue.BigUintTypeValue)
    });
    var option = TypeValue.OptionValue(auction);
    
    var results = await SmartContract.QuerySmartContract(scAddress, "getFullAuctionData",
        new IBinaryType[]
        {
            TokenIdentifierValue.From("TSTKR-209ea0"),
            NumericValue.U64Value(3),
        },
        new []{ option }, provider);

    var optFullAuctionData = results[0].ValueOf<OptionValue>();
    if (optFullAuctionData.IsSet())
    {
        var fullAuctionData = optFullAuctionData.Value.ValueOf<StructValue>().Fields;
    }
}
```

#### Query a smart contract and get result in c# class or as a JSON string
```csharp
var results = await SmartContract.QuerySmartContract(scAddress, "getFullAuctionData",
    new IBinaryType[]
    {
        TokenIdentifierValue.From("TSTKR-209ea0"),
        NumericValue.U64Value(3),
    },
    outputTypeValue: new[] {option}, provider);

//FullAuctionData is a standard C# class.
var fullAuctionDataJSON = results[0].ToJSON();
var fullAuctionData = results[0].ToObject<FullAuctionData>();
System.Console.WriteLine("payment_token.token_type {0}", fullAuctionData.payment_token.token_type);
System.Console.WriteLine("payment_token.nonce {0}", fullAuctionData.payment_token.nonce);
System.Console.WriteLine("min_bid {0}", fullAuctionData.min_bid);
}
```
The result of `.ToJSON()` is a plain JSON object: 
```json
{
  "payment_token": {
    "token_type": "EGLD",
    "nonce": "0"
  },
  "min_bid": "100000000000000000",
  "max_bid": "10000000000000000000",
  "deadline": "1621378452",
  "original_owner": "erd1lkeja8knfjhkqzvrf3d9hmefxzt75wtf3vlg9m7ccugc8jmnrdpqy7yjeq",
  "current_bid": "0",
  "current_winner": "erd1qqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqq6gq4hu",
  "marketplace_cut_percentage": "500",
  "creator_royalties_percentage": "0"
}
```

#### ESDTTokenManager
```csharp
var manager = new ESDTTokenManager(provider, constants, wallet);

var tokenIdentifier = await manager.IssueNonFungibleToken("MyToken1", "MTKN");
System.Console.WriteLine($"Issue token : {tokenIdentifier}");

await manager.SetSpecialRole(tokenIdentifier, ESDTTokenTransactionRequest.NFTRoles.ESDTRoleNFTCreate);

var tokenId = await manager.CreateNFT(tokenIdentifier, "My token name can be a longer one");
System.Console.WriteLine($"Create token  '{tokenIdentifier}:{tokenId}'");
```
# Change Log
All notable changes will be documented in this file.

## [1.0.13] - 23.05.2021
-   Add ESDTNFT Token operations
    - IssueESDTTransactionRequest
    - SetSpecialRoleTransactionRequest
    - IssueNonFungibleTokenTransactionRequest
    - IssueSemiFungibleTokenTransactionRequest
    - TransferESDTNFTTransactionRequest
    - TransferESDTTransactionRequest
    - CreateESDTNFTTokenTransactionRequest
-   Update console app to add utilisation example of `ESDTTokenManager`.

## [1.0.12] - 23.05.2021
-   [Add support for Multi<T> result in smartContract query](https://github.com/yann4460/elrond-sdk.dotnet/pull/12)

## [1.0.11] - 22.05.2021
-   [Directly map smartContract result to JSON Object with `T ToObject<T>` method](https://github.com/yann4460/elrond-sdk.dotnet/pull/11)

## [1.0.10] - 21.05.2021
-   Allow to create a Option value with a null inner Type

## [1.0.9] - 21.05.2021
-   Allow to query smart contract values with or without ABI definition.
-   Update documentation to add call examples.

## [1.0.8] - 21.05.2021
-   [Add smart contract query option.](https://github.com/yann4460/elrond-sdk.dotnet/pull/9) 

## [1.0.7] - 20.05.2021
-   [Add  GetSmartContractResult method.](https://github.com/yann4460/elrond-sdk.dotnet/pull/8) 
    - `var getSumResult = getSumTransaction.GetSmartContractResult("getSum", abi);`

### [1.0.6] - 20.05.2021
-   Remove Argument class. Prefer the use of IBinaryType
-   Ex : `Argument.CreateArgumentFromInt64(12)` is now obsolete. Use : `NumericValue.BigIntValue(12)` instead.
    - Build a balance argument : `NumericValue.BigUintValue(Balance.EGLD("10").Value)`
    - Build from a byte array : `BytesValue.FromBuffer(new byte[] {0x00});`

### [1.0.5] - 20.05.2021
-   [Add mutliples codec](https://github.com/yann4460/elrond-sdk.dotnet/pull/5)

### [1.0.4] - 18.05.2021
-   [Compute GasLimit for transfer #4](https://github.com/yann4460/elrond-sdk.dotnet/pull/4)