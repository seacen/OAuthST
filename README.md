# OAuthST
#### An OAuth controller that handles [ROPC](https://tools.ietf.org/html/rfc6749#section-1.3.3) grant type authorization obtaining and auto token refreshing

## NuGet Package
https://www.nuget.org/packages/OAuthST/

## Usage
``` C#
var account = new AuthController(new IOAuthClient());  // you need to create your own implementation of the IOAuthClient interface

// retrieved token is stored inside AuthController.Info
account.RetrieveAccessToken(username, password);

// auto refresh the token when it's about to expire
account.RenewTokenBeforeExpiry();

account.TokenRenewed += (object sender, TokenRenewedEventArgs e) =>
{
    // Do something when token is renewed
};
```
### References
[`AuthController`](OAuthST/AuthController.cs), 
[`IOAuthClient`](OAuthST/Models/IOAuthClient.cs), 
[`AuthController.Info`(`OAuthResponse`)](OAuthST/Models/OAuthResponse.cs), 
[`AuthController.RetrieveAccessToken(username, password)`](OAuthST/AuthController.cs#L55-L71), 
[`AuthController.RenewTokenBeforeExpiry()`](OAuthST/AuthController.cs#L166-L194), 
[`AuthController.TokenRenewed`](OAuthST/AuthController.cs#L263-L270)
