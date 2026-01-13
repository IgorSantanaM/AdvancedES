using BeerSender.Domain.Boxes;
using JasperFx.Events;
using Marten.Events.Aggregation;
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

    public class UnsentBoxProjection : SingleStreamProjection<UnsentBox, Guid>
    {
        private const string STATUS_READY_TO_SEND = "Ready to send";

        public UnsentBoxProjection()
        {
            DeleteEvent<BoxSent>();
        }

        public static UnsentBox Create(BoxCreated boxCreated)
        {
            return new UnsentBox();
        }

        public void Apply(BoxClosed _, UnsentBox box)
        {
            box.Status = STATUS_READY_TO_SEND;
        }
    }
}
