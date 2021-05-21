# elrond-sdk.dotnet

Elrond SDK for .NET Core.

## Under development, stay tuned!

# Change Log

All notable changes will be documented in this file.

## [1.0.8] - 22.05.2021

-   Add `BytesValue.FromUtf8()` helper method


## [1.0.7] - 21.05.2021

-   [Add  GetSmartContractResult method.](https://github.com/yann4460/elrond-sdk.dotnet/pull/8) 
    - `var getSumResult = getSumTransaction.GetSmartContractResult("getSum", abi);`

## [1.0.6] - 21.05.2021

-   [Remove Argument class. Prefer the use of IBinaryType](https://github.com/yann4460/elrond-sdk.dotnet/pull/7)
-   Ex : `Argument.CreateArgumentFromInt64(12)` is ow obsolete. Use : `NumericValue.BigIntValue(12)` instead.
    - Build a balance argument : `NumericValue.BigUintValue(Balance.EGLD("10").Value)`
    - Build from a byte array : `BytesValue.FromBuffer(new byte[] {0x00});`

## [1.0.5] - 21.05.2021

-   [Add mutliples codec](https://github.com/yann4460/elrond-sdk.dotnet/pull/5)

## [1.0.4] - 18.05.2021

-   [Compute GasLimit for transfer #4](https://github.com/yann4460/elrond-sdk.dotnet/pull/4)
