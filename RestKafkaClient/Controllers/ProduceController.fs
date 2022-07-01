namespace RestKafkaClient.Controllers

open KafkaFacade
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open SharedLibrary

[<ApiController>]
[<Route("[controller]")>]
type ProduceController (producer: ProducerFacade<string, ScoreResult>, logger: ILogger<ProduceController>) =
    inherit ControllerBase()
    
    [<HttpPost("str")>]
    member _.SendMessageByString(dto: ProduceDto<string, ScoreResult>) =
        dto
        |> producer.PostModelAsString (fun d -> d.Message.Value |> logger.LogInformation)
        
        $"Send message {dto.KeyForPartition} to topic {dto.TopicName} by string"
        
    [<HttpPost("byte")>]        
    member _.SendMessageByByteArray(dto: ProduceDto<string, ScoreResult>) =
        dto
        |> producer.PostModelAsByteArray (fun d -> d.Message.Value.ToString() |> logger.LogInformation)
        
        $"Send message {dto.KeyForPartition} to topic {dto.TopicName} by byte[]"