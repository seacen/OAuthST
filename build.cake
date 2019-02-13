var target = Argument("target", "Build");
var configuration = Argument("configuration", "Debug");
var vsVersion = Argument("vsVersion", "2017");
var packageVersion = Argument("packageVersion", "1.0.0");
var nugetApiKey = Argument("nugetApiKey", "");

Task("Build")
  .Does(() =>
{
    var msbuildToolVersion = MSBuildToolVersion.VS2017;
    if(vsVersion == "2019")
        msbuildToolVersion = MSBuildToolVersion.VS2019;

    MSBuild("./OAuthST.sln", configurator =>
        configurator.SetConfiguration(configuration)
            .SetVerbosity(Verbosity.Minimal)
            .UseToolVersion(msbuildToolVersion));
});

Task("NuGetPack")
    .IsDependentOn("Build")
    .Does(() =>
{
	NuGetPack("./deploy/OAuthST.nuspec", new NuGetPackSettings 
	{
		Version = packageVersion, 
		WorkingDirectory = "./deploy"
	});
});

Task("NuGetPack")
    .IsDependentOn("Build")
    .Does(() =>
{
	var package = $"./deploy/OAuthST.{packageVersion}.nupkg";

	NuGetPush(package, new NuGetPushSettings { ApiKey = nugetApiKey });
});


RunTarget(target);