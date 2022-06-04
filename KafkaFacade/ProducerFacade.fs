namespace KafkaFacade

open System
open System.Text.Json
open Confluent.Kafka

/// Use generic wrap string producer, handle serialization manually.
type ProducerFacade<'TKey, 'TValue when 'TValue : not struct>(config : ProducerConfig) =
    let producer = ProducerBuilder<string, string>(config).Build()

    member this.PostModel deliveryHandler model =
        let key = JsonSerializer.Serialize model.KeyForPartition
        let msgContent = JsonSerializer.Serialize model.MessageContent
        let message = Message<string, string>(Key = key, Value = msgContent)

        let handler = System.Action<DeliveryReport<string, string>>(deliveryHandler)

        producer.Produce(model.TopicName, message, handler)

    interface IDisposable with
        member this.Dispose() =
            producer.Flush()
            producer.Dispose()
