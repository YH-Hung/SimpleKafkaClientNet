open System
open Confluent.Kafka
open KafkaFacade

let args = Environment.GetCommandLineArgs()
let config = ProducerConfig(BootstrapServers = args[1])
let topicName = args[2]
let mutable counter = 0
let mutable input = ""

using (new ProducerFacade<string, string>(config)) (fun producer ->
    while (input <- Console.ReadLine(); input <> "break") do
        if counter > 2 then counter <- 0
        { TopicName = topicName; KeyForPartition = counter; MessageContent = input }
        |> producer.PostModel (fun d -> Console.WriteLine(d.Message.Value))
        counter <- counter + 1
        )

printfn "Producer terminated."
