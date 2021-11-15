# [Documentation](./index.md)
## Basic usage

### Load a JSON Key file and create a Wallet instance
Here's a key file JSON for Alice account, do not use the key file in mainnet.
```json
{
  "version": 4,
  "id": "0dc10c02-b59b-4bac-9710-6b2cfa4284ba",
  "address": "0139472eff6886771a982f3083da5d421f24c29181e63888228dc81ca60d69e1",
  "bech32": "erd1qyu5wthldzr8wx5c9ucg8kjagg0jfs53s8nr3zpz3hypefsdd8ssycr6th",
  "crypto": {
    "ciphertext":
      "4c41ef6fdfd52c39b1585a875eb3c86d30a315642d0e35bb8205b6372c1882f135441099b11ff76345a6f3a930b5665aaf9f7325a32c8ccd60081c797aa2d538",
    "cipherparams": {
      "iv": "033182afaa1ebaafcde9ccc68a5eac31"
    },
    "cipher": "aes-128-ctr",
    "kdf": "scrypt",
    "kdfparams": {
      "dklen": 32,
      "salt": "4903bd0e7880baa04fc4f886518ac5c672cdc745a6bd13dcec2b6c12e9bffe8d",
      "n": 4096,
      "r": 8,
      "p": 1
    },
    "mac": "5b4a6f14ab74ba7ca23db6847e28447f0e6a7724ba9664cf425df707a84f5a8b"
  }
}
```
We can create a [`Wallet`](../src/Domain/Wallet.cs) instance by providing this key file and the associated password : 
```csharp
var filePath = "path/to/keyfile.json";
var password = "password";
var wallet = Wallet.DeriveFromKeyFile(KeyFile.FromFilePath(filePath), password);
```

For more usage please, see the [WalletTest.cs](../tests/Erdcsharp.Tests/Domain/WalletTests.cs) file.


### Create an Account, Address and TokenAmount instance
An [`Account`](../src/Domain/Account.cs) instance is build from an [`Address`](../src/Domain/Address.cs)

We have multiple way to build an [`Address`](../src/Domain/Address.cs)
* By providing the bech32 address form 
```csharp 
Address.FromBech32("erd1qyu5wthldzr8wx5c9ucg8kjagg0jfs53s8nr3zpz3hypefsdd8ssycr6th")
```
* By providing the hex address form 
```csharp 
Address.FromHex("0139472eff6886771a982f3083da5d421f24c29181e63888228dc81ca60d69e1")
```
* By providing a valid bytes array
```csharp 
Address.FromBytes(new byte[] {0x00, 0x00, [...]});
```

Full usage example below : 
[C# Snippet from integration tests](../tests/Erdcsharp.IntegrationTests/AccountIntegrationTests.cs) 
```csharp
[Test(Description = "Synchronize an account from the network")]
public async Task Should_Get_Alice_Balance()
{
    var alice = new Account(Address.FromBech32(TestData.AliceBech32));
    await alice.Sync(_provider);

    Assert.That(alice.Balance.Token.Ticker, Is.EqualTo("EGLD"));
    Assert.That(alice.Nonce, Is.Not.Null);                      //5456
    Assert.That(alice.Balance.Value, Is.Not.Null);              //7076251965849781128
    Assert.That(alice.Balance.ToCurrencyString(), Is.Not.Null); //7.076251965849781128 EGLD
}
```
## Send a transaction
### Build and sign a transaction

First, we need to get a valid [`NetworkConfig`](../src/Domain/NetworkConfig.cs) instance.

```csharp
var networkConfig = await NetworkConfig.GetFromNetwork(provider);
```

Then, we can build a [`TransactionRequest`](../src/Domain/TransactionRequest.cs) and send it with the provider.

```csharp
// Ensure that the account in synchronize with the network to have a valid Nonce
await aliceAccount.Sync(_provider);

// Alice send 1 EGLD to bob
var txRequest = TransactionRequest.Create(aliceAccount, networkConfig, bobAddress, TokenAmount.EGLD("1"));
var tx        = await txRequest.Send(_provider, aliceWallet);

// Await the execution of the transaction
await tx.AwaitExecuted(_provider);
```

### Fetch transaction detail from the network

We can the transaction detail from the network with the provider by calling the `GetTransactionDetail` method.
```csharp
var transactionDetail = await _provider.GetTransactionDetail("ca5f97d1542307ae75086c6284ada1ed5db0dcc639e2ac2ad4fa59d3949c5e3a");
```