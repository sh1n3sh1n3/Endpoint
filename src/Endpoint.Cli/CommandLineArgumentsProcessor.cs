using CommandLine;
using Endpoint.Core;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Endpoint.Cli;

public class CommandLineArgumentsProcessor: BackgroundService
{
    private readonly ILogger<CommandLineArgumentsProcessor> _logger;
    private readonly IMediator _mediator;
    private readonly IConfiguration _configuration;

    public CommandLineArgumentsProcessor(
        ILogger<CommandLineArgumentsProcessor> logger,
        IMediator mediator,
        IConfiguration configuration)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    private Parser _createParser() => new Parser(with =>
    {
        with.CaseSensitive = false;
        with.HelpWriter = Console.Out;
        with.IgnoreUnknownArguments = true;
    });

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var args = Environment.GetCommandLineArgs().Skip(1).ToArray();

        if (args.Length == 0 || args[0].StartsWith("-"))
        {
            args = new string[1] { _configuration[Constants.EnvironmentVariables.DefaultCommand] }.Concat(args).ToArray();
        }

        var verbs = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(type => type.GetCustomAttributes(typeof(VerbAttribute), true).Length > 0)
            .ToArray();

        _createParser().ParseArguments(args, verbs)
            .WithParsed(async (dynamic request) => {

                await _mediator.Send(request).ConfigureAwait(false);

                Environment.Exit(0);
            });
    }
}
