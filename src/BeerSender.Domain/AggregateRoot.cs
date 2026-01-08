using System;
using System.Collections.Generic;
using System.Text;

namespace BeerSender.Domain;

public abstract class AggregateRoot
{
    public void Apply(object @event) { }
}