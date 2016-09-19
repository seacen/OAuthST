# OAuthST
#### An OAuth controller that handles ROPC grant type authorization obtaining and auto token refreshing

## NuGet Package
https://www.nuget.org/packages/OAuthST/

## Usage
``` C#
var account = new AuthController(new IOAuthClient());  // you need to create your own implementation of the IOAuthClient interface

// retrieved token is stored in AuthController.Info
account.RetrieveAccessToken(username, password);

// auto refresh the token when it's about to expire
account.RenewTokenBeforeExpiry();

account.TokenRenewed += (object sender, TokenRenewedEventArgs e) =>
{
    // Do something when token is renewed
};
```
