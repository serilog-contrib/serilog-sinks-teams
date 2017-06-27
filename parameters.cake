#tool "nuget:?package=GitVersion.CommandLine"
#tool "nuget:?package=vswhere"

#addin "Cake.Git"

public class BuildParameters
{
    public string Target { get; private set; }

    public string SolutionBuildConfiguration { get; private set; }

    public string Version { get; private set; }

    public string FullVersion { get; private set; }

    public string AssemblyVersion { get; private set;}

    public FilePath MSBuildPath {get; private set;}

    public static BuildParameters GetParameters(ICakeContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException("context");
        }

        var target = context.Argument("target", "Default");
        var solutionBuildConfiguration = context.Argument("SolutionBuildConfiguration", "Release");

        var vsLatest = context.VSWhereLatest();
        if (vsLatest == null)
        {
            throw new InvalidOperationException("MSBuild not found.");
        }
        var msBuildPath = vsLatest.CombineWithFilePath("./MSBuild/15.0/Bin/amd64/MSBuild.exe");

        var logFilePath = context.MakeAbsolute(context.File("gitVersion.log")).FullPath;
        var gitVersionSettings = new GitVersionSettings
        {
            WorkingDirectory = context.Directory(System.IO.Directory.GetCurrentDirectory()),
            LogFilePath = context.File(logFilePath)
        };
        GitVersion gitVersion;
        try
        {
            gitVersion = context.GitVersion(gitVersionSettings);
        } catch
        {
            var logs = System.IO.File.ReadAllLines(logFilePath);
            foreach (var log in logs)
            {
                context.Information(log);
            }
            throw;
        }

        context.Information("Semantic Version: " + gitVersion.SemVer);
        context.Information("Full version: " + gitVersion.FullSemVer);

        return new BuildParameters
        {
            Target = target,
            SolutionBuildConfiguration = solutionBuildConfiguration,
            MSBuildPath = msBuildPath,
            FullVersion = gitVersion.FullSemVer,
            AssemblyVersion = gitVersion.AssemblySemVer,
            Version = gitVersion.SemVer
        };
    }
}