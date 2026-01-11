using BeerSender.Domain.Boxes;
using JasperFx.Events;
using Marten.Events.Projections;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeerSender.Domain.Projections
{
    public class UnsentBox
    {
        public Guid BoxId { get; set; }
        public string? Status { get; set; }
    }

    public class UnsentBoxProjection : EventProjection
    {
        private const string STATUS_READY_TO_SEND = "Ready to send";

        public UnsentBoxProjection()
        {
            Project<IEvent<BoxCreated>>((evt, operations) =>
            {
                operations.Store(new UnsentBox
                {
                    BoxId = evt.StreamId
                });
            });

            Project<IEvent<BoxSent>>((evt, operations) =>
            {
                operations.Delete<UnsentBox>(evt.StreamId);
            });

            ProjectAsync<IEvent<BoxClosed>>(async (evt, operations, token) =>
            {
                if (token.IsCancellationRequested) return;
                var unsentBox = await operations.LoadAsync<UnsentBox>(evt.StreamId);

                if (unsentBox is null)
                    return;

                unsentBox.Status = STATUS_READY_TO_SEND;

                operations.Store(unsentBox);
            });
        }
    }
}
