namespace KafkaFacade

type ProduceDto<'TKey, 'TValue> = {
    TopicName : string
    KeyForPartition: 'TKey
    MessageContent: 'TValue
}
