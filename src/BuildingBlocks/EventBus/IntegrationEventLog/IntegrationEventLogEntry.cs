using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using EventBus.Events;
using Newtonsoft.Json;

namespace IntegrationEventLog
{
    public class IntegrationEventLogEntry
    {
        public IntegrationEventLogEntry(IntegrationEvent integrationEvent, Guid transactionId)
        {
            EventId = integrationEvent.Id;            
            EventTypeName = integrationEvent.GetType().FullName;            
            State = EventState.NotPublished;            
            TimesSent = 0;
            CreationTime = integrationEvent.CreationDate;
            Content = JsonConvert.SerializeObject(integrationEvent);
            TransactionId = transactionId.ToString();
        }

        private IntegrationEventLogEntry() { }

        public Guid EventId { get; }

        public DateTime CreationTime { get; }

        public string EventTypeName { get; }

        public string Content { get; }

        public EventState State { get; set; }

        public int TimesSent { get; set; }

        public string TransactionId { get; }

        [NotMapped]
        public IntegrationEvent IntegrationEvent { get; private set; }

        [NotMapped]
        public string EventTypeShortName => EventTypeName.Split('.')?.Last();

        public IntegrationEventLogEntry DeserializeJsonContent(Type type)
        {
            IntegrationEvent = JsonConvert.DeserializeObject(Content, type) as IntegrationEvent;
            return this;
        }
    }
}
