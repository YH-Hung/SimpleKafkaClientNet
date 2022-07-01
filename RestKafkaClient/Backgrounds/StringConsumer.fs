namespace RestKafkaClient.Backgrounds

open System
open System.Text.Json
open System.Threading.Tasks
open Confluent.Kafka
open KafkaFacade
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open SharedLibrary

type StringConsumer(logger: ILogger<StringConsumer>) =
    inherit BackgroundService()

    let config = ConsumerConfig(BootstrapServers = "localhost:9092")
    do 
        config.GroupId <- "SimpleConsumer"
        config.AutoOffsetReset <- AutoOffsetReset.Earliest
                                                                
    override this.ExecuteAsync(stoppingToken) =
        try
            task {
                use consumer = new ConsumerFacade<string, ScoreResult, string>(config, "test-serde-string", JsonSerializer.Deserialize<ScoreResult>)
                while not stoppingToken.IsCancellationRequested do
                    let! msg = Task.Run(Func<ScoreResult>(fun () -> consumer.FetchNext stoppingToken))
                    msg.ToString() |> logger.LogInformation
            }            
        with
            | :? OperationCanceledException ->
                "Consumer closed..." |> logger.LogWarning
                Task.CompletedTask
        