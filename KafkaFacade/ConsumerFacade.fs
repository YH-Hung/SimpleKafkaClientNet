namespace KafkaFacade

open System
open System.Text.Json
open System.Threading
open Confluent.Kafka

type ConsumerFacade<'TValue when 'TValue : not struct>(config : ConsumerConfig, topicName : string) =
    let consumer = ConsumerBuilder<string, string>(config).Build()
    do
        consumer.Subscribe(topicName)

    member this.FetchNext (cts : CancellationToken) =
        let msgContent = consumer.Consume(cts).Message.Value
        JsonSerializer.Deserialize<'TValue> msgContent

    interface IDisposable with
        member this.Dispose() = consumer.Close()
