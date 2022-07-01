namespace RestKafkaClient
#nowarn "20"
open System
open System.Collections.Generic
open System.IO
open System.Linq
open System.Threading.Tasks
open Confluent.Kafka
open KafkaFacade
open Microsoft.AspNetCore
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.HttpsPolicy
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open RestKafkaClient.Backgrounds
open SharedLibrary

module Program =
    let exitCode = 0

    [<EntryPoint>]
    let main args =

        let builder = WebApplication.CreateBuilder(args)

        let producerConfig =  ProducerConfig(BootstrapServers = "localhost:9092")
        builder.Services.AddSingleton(new ProducerFacade<string, ScoreResult>(producerConfig))
        builder.Services.AddHostedService<StringConsumer>()
        builder.Services.AddHostedService<ByteArrayConsumer>()
        
        builder.Services.AddControllers()

        let app = builder.Build()

        app.UseHttpsRedirection()

        app.UseAuthorization()
        app.MapControllers()

        app.Run()

        exitCode