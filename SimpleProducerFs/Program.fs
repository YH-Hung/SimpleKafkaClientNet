open System
open System.Text.Json
open Confluent.Kafka
open KafkaFacade
open SharedLibrary

let args = Environment.GetCommandLineArgs()
let config = ProducerConfig(BootstrapServers = args[1])
let topicName = args[2]
let mutable counter = 0
let mutable input = ""

using (new ProducerFacade<int, ScoreResult>(config)) (fun producer ->
    while (input <- Console.ReadLine(); input <> "break") do
        try
            if counter > 2 then counter <- 0
            { TopicName = topicName; KeyForPartition = counter; MessageContent = JsonSerializer.Deserialize input }
            |> producer.PostModelAsString (fun d -> printfn "Message %s send to topic %s" input topicName)
            counter <- counter + 1
        with
            | :? Exception -> printfn "Deserialization failed"          
        )

printfn "Producer terminated."
