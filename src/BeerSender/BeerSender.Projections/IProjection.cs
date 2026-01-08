using BeerSender.Projections.Database.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeerSender.Projections
{
    public interface IProjection
    {
        public List<Type> RelevantEventTypes { get; }
        public int BatchSize { get; }   
        public int WaitTime { get; }
        public void Project(IEnumerable<StoredEventWithVersion> events);
    }
}
