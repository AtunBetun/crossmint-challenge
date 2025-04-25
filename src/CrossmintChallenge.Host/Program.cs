using CrossmintChallenge.Clients;
using Flurl;
using Serilog;

Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Console().CreateLogger();

Log.Information("starting");
const string candidateId = "9b5772ff-aa4e-4870-956c-9e0332789869";
Url url = new Url("https://challenge.crossmint.io/api");

await MegaverseMapClient.GetMap(url, candidateId);
Log.Information("end");
