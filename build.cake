var src = Directory("./src");
var dst = Directory("./artifacts");
var reports = dst + Directory("./reports");
var configuration = Argument("Configuration", "Release");

// The build number to use in the version number of the built NuGet packages.
// There are multiple ways this value can be passed, this is a common pattern.
// 1. If command line parameter parameter passed, use that.
// 2. Otherwise if running on AppVeyor, get it's build number.
// 3. Otherwise if running on Travis CI, get it's build number.
// 4. Otherwise if an Environment variable exists, use that.
// 5. Otherwise default the build number to 0.
var buildNumber =
    HasArgument("BuildNumber") ? Argument<int>("BuildNumber") :
    AppVeyor.IsRunningOnAppVeyor ? AppVeyor.Environment.Build.Number :
    TravisCI.IsRunningOnTravisCI ? TravisCI.Environment.Build.BuildNumber :
    EnvironmentVariable("BuildNumber") != null ? int.Parse(EnvironmentVariable("BuildNumber")) : 1;
 
// Assume we're building on appveyor for publishing NuGets
// So always add a beta prefix if not doing a tag
var isTag = EnvironmentVariable("APPVEYOR_REPO_TAG") != null && EnvironmentVariable("APPVEYOR_REPO_TAG") == "true" ;
var revision = isTag ? null : "beta-" +buildNumber.ToString("D4");

Task("Clean").Does(() => {
    CleanDirectories(dst);
    CleanDirectories(src.Path + "/**/bin");
    CleanDirectories(src.Path + "/**/obj");
    CleanDirectories(src.Path + "/**/pkg");
    DotNetCoreClean(src);
});

Task("Restore").Does(() => {
    EnsureDirectoryExists(dst);
    DotNetCoreRestore(src);
});


Task("Test").Does(() => {
    var settings = new DotNetCoreTestSettings {
        NoBuild = true,
        Configuration = "Release"
    };
    foreach(var file in GetFiles(src.Path + "/commercetools.Core.Tests/commercetools.Core.Tests.csproj")) {
        DotNetCoreTest(file.FullPath, settings);
    }
});

Task("Build").Does(() => {
    var settingsStd = new DotNetCoreBuildSettings {
        Configuration = "Release",
        Framework = "netstandard1.3",
    };
    var settingsCore = new DotNetCoreBuildSettings {
        Configuration = "Release",
        Framework = "netcoreapp1.1"
    };
    DotNetCoreBuild(src + Directory("commercetools.Core"), settingsStd);
    DotNetCoreBuild(src + Directory("commercetools.Core.Tests"), settingsCore);
});

Task("Publish")
    .WithCriteria(AppVeyor.IsRunningOnAppVeyor)
    .Does(() => {
        var settings = new DotNetCorePackSettings {
        Configuration = "Release",
        OutputDirectory = dst,
        VersionSuffix = revision
        };
       Information("VersionSuffix: {0}", revision);
        DotNetCorePack(
            src + Directory("commercetools.Core"), 
            settings);
});


Task("Default")
  .IsDependentOn("Clean")
  .IsDependentOn("Restore")
  .IsDependentOn("Build")
  .IsDependentOn("Publish");

RunTarget(Argument("target", "Default"));