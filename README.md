# HuffChat

Simple TCP chat using Sockets (.NET).

Encoded using Huffman coding.

<br>

## Info

Sockets are initialized through a single Server and multiple Client executables.

Huffman tree generator input is generated during Server setup (check *AppConstants.cs* in *HuffChat.BLL* project).

First Server response to the Client is the Huffman generator input needed for sending encoded and decoding incoming messages.

There are some stability issues, try to wait a few seconds before sending each message.

### Start options

#### 1. Using terminal and dotnet CLI

First, start the server from the solution root directory (*\HuffChat*):

```terminal
dotnet run --project HuffChat.Server
```

After the server is initialized, you can start one or multiple clients:

**1.1 Default - host machine, automatically linked to the above Server**

```terminal
dotnet run --project HuffChat.Client
```

**1.2 Define Client connection using params**

 - *{y...}* values represent the port
 - *{x...}* values represent the IP address 
 - *{displayName}* is the name shown before each message (random if not provided)

```terminal
dotnet run --project HuffChat.Client {xxxx::xxxx:xxxx:xxxx:xxxxxx} {yyyyy} {displayName}
```

<br>

#### 2. Using executables

Found in:

- *\HuffChat\HuffChat.Server\bin\Release\net6.0* and *\HuffChat\HuffChat.Client\bin\Release\net6.0* or
- *\HuffChat\HuffChat.Server\bin\Debug\net6.0* and *\HuffChat\HuffChat.Client\bin\Debug\net6.0* 

<br>

#### 3. Using Visual Studio 

Runs 2 instances - 1 Server and 1 Client. Both will be connected to host machine.

<br>

## Demo

### Using executables

//TODO

<br>

## Tech

**IDEs:** [Visual Studio 2022](https://visualstudio.microsoft.com/vs/)

**Frameworks:** [ASP.NET Core](https://learn.microsoft.com/en-us/dotnet/fundamentals/)

[![My Skills](https://skillicons.dev/icons?i=dotnet,visualstudio)](https://skillicons.dev)

<br>

## References

- [.NET Sockets](https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/sockets/socket-services)

<br>

## Attributions

- [Huffman coding](https://www.csharpstar.com/csharp-huffman-coding-using-dictionary/)

<br>

## Authors

- [@v-gabriel](https://github.com/v-gabriel)
