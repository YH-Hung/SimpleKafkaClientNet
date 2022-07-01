namespace KafkaFacade

open System
open System.Text.Json
open Confluent.Kafka

/// Use generic wrap string producer, handle serialization manually.
type ProducerFacade<'TKey, 'TValue when 'TValue : not struct>(config : ProducerConfig) =
    let producerString = ProducerBuilder<'TKey, string>(config).Build()
    let producerByteArray = ProducerBuilder<'TKey, byte[]>(config).Build()
    
    let PostModel (producer: IProducer<'TKey, 'T>) (serializer: 'TValue -> 'T) deliveryHandler model =
        let key = model.KeyForPartition
        let msgContent = serializer model.MessageContent
        let message = Message<'TKey, 'T>(Key = key, Value = msgContent)
        
        let handler = System.Action<DeliveryReport<'TKey, 'T>>(deliveryHandler)
        
        producer.Produce(model.TopicName, message, handler)

    member this.PostModelAsString = PostModel producerString JsonSerializer.Serialize
        
    member this.PostModelAsByteArray = PostModel producerByteArray JsonSerializer.SerializeToUtf8Bytes

    interface IDisposable with
        member this.Dispose() =
            producerString.Flush()
            producerByteArray.Flush()
            producerString.Dispose()
            producerByteArray.Dispose()
