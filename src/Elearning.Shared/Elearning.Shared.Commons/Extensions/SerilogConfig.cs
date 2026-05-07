using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System.Diagnostics;
using System.Reflection;

public sealed class CLogsOptions
{
    public string Environment { get; set; } = "Production";
    public string DeploymentUnit { get; set; } = "Unkown";
    public string HostName { get; set; } = "https://es.example.com:9200";
    public string? ApiKey { get; set; }
    public string LogLevelToConsole { get; set; } = "Information";
    public string LogLevelToDB { get; set; } = "Warning";
    public string LogLevelToFile { get; set; } = "Warning";
    public Dictionary<string, string>? LogLevels { get; set; }
    public string TypeIndice { get; set; } = "dotnet";
    public string Dataset { get; set; } = "dotnet";
    public string Namespace { get; set; } = "prod";

    public bool EnableConsole { get; set; } = true;
    public bool EnableElasticsearch { get; set; } = false;
    public bool EnableFile { get; set; } = false;

    public string LogFilePath { get; set; } = "logs/app-.log";
    public long FileSizeLimitBytes { get; set; } = 100 * 1024 * 1024; // 100MB
    public int RetainedFileCountLimit { get; set; } = 31; // giữ 31 file
    public string RollingInterval { get; set; } = "Day"; // Day, Hour, Month
    public bool RollOnFileSizeLimit { get; set; } = true;
    public bool SharedFile { get; set; } = false;
    public string FileOutputTemplate { get; set; } = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}";

    public string? ModeAuth { get; set; }
    public string? BasicKey { get; set; }
    public string? UserName =>
        string.Equals(ModeAuth, "Basic", StringComparison.OrdinalIgnoreCase) &&
        !string.IsNullOrWhiteSpace(BasicKey)
            ? BasicKey.Split(':', 2).FirstOrDefault()
            : null;
    public string? Password =>
        string.Equals(ModeAuth, "Basic", StringComparison.OrdinalIgnoreCase) &&
        !string.IsNullOrWhiteSpace(BasicKey) &&
        BasicKey.Contains(':')
            ? BasicKey.Split(':', 2).Last()
            : null;
}

public static class SerilogConfig
{
    public static (Logger Logger, LoggingLevelSwitch LevelSwitch) ConfigureLogger(IConfiguration config)
    {
        var opts = config.GetSection("CLogs").Get<CLogsOptions>()
                   ?? new CLogsOptions();

        var lvlConsole = Parse(opts.LogLevelToConsole, LogEventLevel.Information);
        var lvlDb = Parse(opts.LogLevelToDB, LogEventLevel.Warning);
        var lvlFile = Parse(opts.LogLevelToFile, LogEventLevel.Warning);

        // Tính minimum level dựa trên các sink được enabled
        var activeLevels = new List<LogEventLevel>();
        if (opts.EnableConsole) activeLevels.Add(lvlConsole);
        if (opts.EnableElasticsearch) activeLevels.Add(lvlDb);
        if (opts.EnableFile) activeLevels.Add(lvlFile);

        var minLevel = activeLevels.Any()
            ? (LogEventLevel)activeLevels.Min(l => (int)l)
            : LogEventLevel.Information;

        var levelSwitch = new LoggingLevelSwitch(minLevel);

        var appName = Assembly.GetEntryAssembly()?.GetName().Name ?? "app";
        var deploymentUnit = opts.DeploymentUnit ?? "unknown";
        var appVer = Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "unknown";

        var cfg = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(levelSwitch)
            .MinimumLevel.Override("Microsoft", lvlConsole)
            .MinimumLevel.Override("Microsoft.AspNetCore", lvlConsole)
            .Filter.ByExcluding(e => e.Exception?.GetType().Name == "JSDisconnectedException")
            .Enrich.FromLogContext()

            .Enrich.WithProperty("deployment_unit", deploymentUnit)

            .Enrich.WithProperty("service.name", appName)
            .Enrich.WithProperty("service.version", appVer)
            .Enrich.WithProperty("service.environment", opts.Environment)
            .Enrich.With(new ActivityEnricher());

        // ✅ Console Sink (nếu enabled)
        if (opts.EnableConsole)
        {
            cfg = cfg.WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(e => e.Level >= lvlConsole)
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext} [{ServerIP}]{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}",
                    theme: AnsiConsoleTheme.Code));
        }

        // ✅ Elasticsearch Sink (nếu enabled)
        //if (opts.EnableElasticsearch)
        //{
        //    string nameIndex = new DataStreamName(opts.TypeIndice, opts.Dataset, opts.Namespace).GetTemplateName();
        //    cfg = cfg.WriteTo.Logger(lc => lc
        //        .Filter.ByIncludingOnly(e => e.Level >= lvlDb)
        //        .WriteTo.Elasticsearch(
        //            nodes: new[] { new Uri(opts.HostName) },
        //            bootstrapMethod: BootstrapMethod.None,
        //            username: string.Equals(opts.ModeAuth, "Basic", StringComparison.OrdinalIgnoreCase)
        //                ? opts.UserName
        //                : null,
        //            password: string.Equals(opts.ModeAuth, "Basic", StringComparison.OrdinalIgnoreCase)
        //                ? opts.Password
        //                : null,
        //            apiKey: !string.Equals(opts.ModeAuth, "Basic", StringComparison.OrdinalIgnoreCase)
        //                ? opts.ApiKey
        //                : null,
        //            dataStream: nameIndex
        //        ));
        //}

        // ✅ File Sink (nếu enabled)
        if (opts.EnableFile)
        {
            var rollingInterval = Enum.TryParse<RollingInterval>(opts.RollingInterval, true, out var ri)
                ? ri
                : RollingInterval.Day;

            cfg = cfg.WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(e => e.Level >= lvlFile)
                .WriteTo.File(
                    path: opts.LogFilePath,
                    outputTemplate: opts.FileOutputTemplate,
                    rollingInterval: rollingInterval,
                    fileSizeLimitBytes: opts.FileSizeLimitBytes,
                    retainedFileCountLimit: opts.RetainedFileCountLimit,
                    rollOnFileSizeLimit: opts.RollOnFileSizeLimit,
                    shared: opts.SharedFile,
                    flushToDiskInterval: TimeSpan.FromSeconds(1)
                ));
        }

        // Override levels
        if (opts.LogLevels != null)
            foreach (var kv in opts.LogLevels)
                if (Enum.TryParse(kv.Value, true, out LogEventLevel l))
                    cfg = cfg.MinimumLevel.Override(kv.Key, l);
        Logger logger;
        try
        {
            logger = cfg.CreateLogger();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[SERILOG-FATAL] CreateLogger failed: {ex}");
            throw;
        }

        var enabledSinks = new List<string>();
        if (opts.EnableConsole)
            enabledSinks.Add($"Console({lvlConsole})");

        if (opts.EnableElasticsearch)
            enabledSinks.Add($"Elasticsearch({lvlDb})");

        if (opts.EnableFile)
            enabledSinks.Add($"File({lvlFile})");

        logger.Warning(
            "Serilog started. env={Env} min={Min} sinks=[{Sinks}]",
            opts.Environment, minLevel, string.Join(", ", enabledSinks));


        return (logger, levelSwitch);
    }

    private static LogEventLevel Parse(string? s, LogEventLevel def) =>
        Enum.TryParse(s, true, out LogEventLevel lvl) ? lvl : def;

    private sealed class ActivityEnricher : Serilog.Core.ILogEventEnricher
    {
        public void Enrich(Serilog.Events.LogEvent logEvent, Serilog.Core.ILogEventPropertyFactory pf)
        {
            var act = Activity.Current;
            if (act == null) return;
            logEvent.AddPropertyIfAbsent(pf.CreateProperty("trace_id", act.TraceId.ToString()));
            logEvent.AddPropertyIfAbsent(pf.CreateProperty("span_id", act.SpanId.ToString()));
        }
    }
}