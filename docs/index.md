# Documentation

## [Basic usage](basic.md)
  * Load a JSON Key file and create a Wallet instance
  * Create an Account, Address and TokenAmount instance
  * Send a basic transaction
    * Build and sign a transaction
    * Fetch transaction detail from the network
## [Advanced usage](advanced.md)
  * Smart contract interactions (Deploy, Call, Query)
    * Compute a smart contract deployment address
  * ESDT (Elrond Standard Digital Token) interraction
    * NFT (Non-fungible tokens)
    * SFT (Semi-fungible tokens)
 
--- 

### Quick start
This is a quick start sample with minimal dependencies.

#### Install .NET
erdcsharp works with .Net Core or .Net Framework (From 4.6.1 upwards). You’ll need to have the .Net SDK installed. For new starters we recommend .Net core. Mac or Linux users will also need .Net Core.

[Download .NET SDK](https://dotnet.microsoft.com/download)


#### Create your application
Create a project using the .Net CLI (below) OR create a project in Visual Studio.
```
dotnet new console -o ElrondConsoleApp
cd ElrondConsoleApp
```

#### Add package reference to 'elrond-sdk-erdcsharp' restore the project packages.
```
dotnet add package elrond-sdk-erdcsharp
dotnet restore
```

#### Open your favorite IDE (VS Code, Visual Studio etc)
Visual Studio Code or Visual Studio are both good choices for .Net development. Other good IDE’s are also available (Jet Brains Rider for instance).

Now, open the Program.cs file in the editor.

#### Code to retreive an account balance
Full sample code for Program.cs. See below for a fuller explanation of each step.

```csharp
public static class Program
{
    public static async Task Main(string[] args)
    {
        await GetAccountBalance();
        System.Console.ReadLine();
    }

    private static async Task GetAccountBalance()
    {
        var provider = new ElrondProvider(new HttpClient(), new ElrondNetworkConfiguration(Network.TestNet));
        var account  = await provider.GetAccount("erd1qyu5wthldzr8wx5c9ucg8kjagg0jfs53s8nr3zpz3hypefsdd8ssycr6th");
            
        System.Console.WriteLine($"Balance : {account.Balance}");

        var amount = TokenAmount.From(account.Balance);
        System.Console.WriteLine($"Balance in EGLD : {amount.ToCurrencyString()}");
    }
}
```

You can directly query the Elrond API with the ElrondProvider by creating an instance of ElrondProvider.
```csharp
var provider = new ElrondProvider(new HttpClient(), new ElrondNetworkConfiguration(Network.TestNet));
```
Using the ElrondProvider we can execute the GetAccount request asynchronously, for our selected account.