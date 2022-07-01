namespace KafkaFacade

open System
open System.Text.Json
open System.Threading
open Confluent.Kafka

type ConsumerFacade<'TKey, 'TValue, 'TSerde when 'TValue : not struct>(config : ConsumerConfig, topicName : string, deserializer: 'TSerde -> 'TValue) =
    let consumer = ConsumerBuilder<'TKey, 'TSerde>(config).Build()
    do
        consumer.Subscribe(topicName)

    member this.FetchNext (cts : CancellationToken) =
        let msgContent = consumer.Consume(cts).Message.Value
        deserializer msgContent

    interface IDisposable with
        member this.Dispose() = consumer.Close()
