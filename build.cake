#load "parameters.cake"

var parameters = BuildParameters.GetParameters(Context);

Task("Restore-NuGet-Packages")
    .Does(() =>
{
    MSBuild("src/Serilog.Sinks.MicrosoftTeams.sln",
        new MSBuildSettings
        {
            ToolPath = parameters.MSBuildPath
        }
        .SetConfiguration(parameters.SolutionBuildConfiguration)
        .WithTarget("restore"));
});

Task("Create-Version-Info")
    .Does(() =>
{
    CreateAssemblyInfo(File("AssemblyVersionInfo.cs"), new AssemblyInfoSettings
    {
        Version = parameters.AssemblyVersion,
        FileVersion = parameters.Version,
        InformationalVersion = parameters.FullVersion
    });
});

Task("Build")
    .IsDependentOn("Create-Version-Info")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    MSBuild("src/Serilog.Sinks.MicrosoftTeams.sln", new MSBuildSettings
        {
            ToolPath = parameters.MSBuildPath
        }
        .SetConfiguration(parameters.SolutionBuildConfiguration)
        .WithTarget("build")
    );
});

Task("Pack")
    .IsDependentOn("Build")
    .Does(() =>
{
    var outputPath = MakeAbsolute(Directory("build")).FullPath;
    MSBuild("src/Serilog.Sinks.MicrosoftTeams.sln", new MSBuildSettings
        {
            ToolPath = parameters.MSBuildPath
        }
        .SetConfiguration(parameters.SolutionBuildConfiguration)
        .WithProperty("PackageVersion", parameters.Version)
        .WithProperty("PackageOutputPath", outputPath)
        .WithTarget("pack")
    );
});

Task("Restore-Version-Info")
    .Does(() =>
{
    CreateAssemblyInfo(File("AssemblyVersionInfo.cs"), new AssemblyInfoSettings
    {
        Version = "0.0.0",
        FileVersion = "0.0.0",
        InformationalVersion = "0.0.0"
    });
});

Task("Default")
    .IsDependentOn("Pack")
    .IsDependentOn("Restore-Version-Info")
    .Does(() =>
{
});

RunTarget(parameters.Target);