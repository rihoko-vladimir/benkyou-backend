# Benkyou!

Benkyou! is a ASP.NET backend for Benkyou! application. This application will help you learn Japanese kanji

## Installation

To install and run backend on your computer you need to have [.NET SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) installed.

It won't work without working database, so you need to have one.

You also need to have your own SMTP server and Firebase cloud storage, where it will store user avatars.

Just start database instance (for example in docker) and change Connection String in appsettings.Development.json.

Also change JWT secrets to your own

```Json
{
  "ConnectionStrings": {
    "SqlServerConnectionString": "Server={your server host}; Database={database name}; User Id = {user login}; Password = {user password}; TrustServerCertificate=True;"
  },
  "JWT": {
    "Audience": "https://localhost:5001",
    "Issuer": "https://localhost:5001",
    "AccessSecret": "{access token secret key}",
    "RefreshSecret": "{refresh token secret key}",
    "AccessTokenExpirationTimeMinutes": 10,
    "RefreshTokenExpirationTimeMinutes": 1440
  },
  "Firebase": {
    "Key": "{firebase api key}",
    "Email": "{account login}",
    "Password": "{account password}"
  },
  "Email": {
    "Server": "{smtp server}",
    "ServerPort": "{smtp port}",
    "Login": "{smtp login}",
    "Password": "{smtp password}"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  }
}

```
After you set everything up you can simply clone this repo and run
```bash
dotnet run --project Benkyou.Presentation
```

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

## License
License is absent