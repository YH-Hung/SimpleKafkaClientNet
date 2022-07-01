open System
open System.Threading
open Confluent.Kafka
open KafkaFacade
open SharedLibrary

let args = Environment.GetCommandLineArgs()
let config = ConsumerConfig(BootstrapServers = args[1])
config.GroupId <- "SimpleConsumer"
config.AutoOffsetReset <- AutoOffsetReset.Earliest

let topicName = args[2]

let cts = new CancellationTokenSource()
Console.CancelKeyPress.Add (fun e ->
    e.Cancel <- true
    cts.Cancel())

using (new ConsumerFacade<ScoreResult>(config, topicName)) (fun consumer ->
    try
        while not cts.IsCancellationRequested do
            consumer.FetchNext cts.Token
            |> Console.WriteLine
    with
        | :? OperationCanceledException -> "Consumer closed..." |> Console.WriteLine
    )

printfn "Consumer terminated."
