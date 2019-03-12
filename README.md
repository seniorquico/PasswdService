# Passwd Service

This repository contains an example project consisting of a simple, cross-platform REST API developed using ASP.NET Core.

## Getting Started on Linux and macOS

Download and run the [.NET Core SDK installer](https://www.microsoft.com/net/download) for your platform. A version 2.2.105 or higher release of the .NET Core SDK is required. The latest available version is recommended. (For reference, the minimum version of the .NET Core SDK required may be found by inspecting the `sdk` property in the `global.json` file.)

To clone the example project repository to your home folder, run the following commands:

```bash
cd ~
git clone https://github.com/seniorquico/PasswdService.git PasswdService
```

To build the example project, run the following commands:

```bash
cd ~/PasswdService
dotnet publish ./src/PasswdService/PasswdService.csproj --configuration Release --output ~/PasswdService/app/
```

To configure the example project, open the `~/PasswdService/app/appsettings.json` file in your favorite text editor. Adjust the `GroupFilePath` and `GroupFileName` settings to point to a valid `/etc/group` file. Adjust the `PasswordFilePath` and `PasswordFileName` settings to point to a valid `/etc/group` file. For example, your configuration may end up looking similar to the following:

```json
{
  "AllowedHosts": "*",
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "PasswdService": {
    "GroupFileName": "group",
    "GroupFilePath": "/etc",
    "PasswordFileName": "passwd",
    "PasswordFilePath": "/etc"
  }
}
```

The folder(s) identified by `GroupFilePath` and `PasswordFilePath` must exist before running the example project.

Depending on your system's security settings, the example project may not be able to access the actual `/etc/group` and `/etc/passwd` files when running (for good reason). If this is true for your system, simply copy `/etc/group` and `/etc/passwd` to a different folder, remove any sensitive information from the copied files, and update the `GroupFilePath` and `PasswordFilePath` settings accordingly.

To run the example project, run the following commands:

```bash
cd ~/PasswdService/app
dotnet PasswdService.dll
```

Congratulations, you're now running the Passwd Service! You may exercise the REST API using your favorite HTTP client with the following base URL:

http://localhost:5000/

Alternatively, you may explore and test the REST API using the bundled Swagger UI utility:

http://localhost:5000/swagger/

## Getting Started on Windows

Download and run the [.NET Core SDK installer](https://www.microsoft.com/net/download?initial-os=windows) for 64-bit Windows (this example project is not supported on 32-bit Windows). A version 2.2.105 or higher release of .NET Core SDK is required. The latest available version is recommended. (For reference, the minimum version of the .NET Core SDK required may be found by inspecting the `sdk` property in the `global.json` file.)

To clone the example project repository to your user profile folder, run the following commands:

```bash
cd "%USERPROFILE%"
git clone https://github.com/seniorquico/PasswdService.git PasswdService
```

To build the example project, run the following commands:

```bash
cd "%USERPROFILE%\PasswdService"
dotnet publish .\src\PasswdService\PasswdService.csproj --configuration Release --output "%USERPROFILE%\PasswdService\app"
```

To configure the example project, open the `%USERPROFILE%\PasswdService\app\appsettings.json` file in your favorite text editor. Adjust the `GroupFilePath` and `GroupFileName` settings to point to a valid `/etc/group` file. Adjust the `PasswordFilePath` and `PasswordFileName` settings to point to a valid `/etc/group` file. Be sure to escape any backslashes in the `GroupFilePath` and `PasswordFilePath` values. For example, your configuration may end up looking similar to the following:

```json
{
  "AllowedHosts": "*",
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "PasswdService": {
    "GroupFileName": "group",
    "GroupFilePath": "C:\\Example Configuration",
    "PasswordFileName": "passwd",
    "PasswordFilePath": "C:\\Example Configuration"
  }
}
```

The folder(s) identified by `GroupFilePath` and `PasswordFilePath` must exist before running the example project.

To run the example project, run the following commands:

```bash
cd "%USERPROFILE%\PasswdService\app"
dotnet PasswdService.dll
```

Congratulations, you're now running the Passwd Service! You may exercise the REST API using your favorite HTTP client with the following base URL:

http://localhost:5000/

Alternatively, you may explore and test the REST API using the bundled Swagger UI utility:

http://localhost:5000/swagger/

## API Documentation

The following table lists the available APIs:

| Method | URL Template | Description |
| --- | --- | --- |
| `GET` | `/groups` | Get all groups defined in `/etc/group` |
| `GET` | `/groups/:gid` | Get the group defined in `/etc/group` matching `gid` |
| `GET` | `/groups/query`<br>`[?name=<name>]`<br>`[&gid=<gid>]`<br>`[&member=<username>[&...]]` | Get the groups defined in `/etc/group` matching _all_ of the specified query terms |
| `GET` | `/users` | Get all users defined in `/etc/passwd` |
| `GET` | `/users/:uid` | Get the user defined in `/etc/passwd` matching `uid` |
| `GET` | `/users/:uid/groups` | Get all groups to which the user matching `uid` is a member (including the primary group) |
| `GET` | `/users/query`<br>`[?name=<name>]`<br>`[&uid=<uid>]`<br>`[&gid=<gid>]`<br>`[&comment=<comment>]`<br>`[&home=<home>]`<br>`[&shell=<shell>]` | Get the users defined in `/etc/passwd` matching _all_ of the specified query terms |
