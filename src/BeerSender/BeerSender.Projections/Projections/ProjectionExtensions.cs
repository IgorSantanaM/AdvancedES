using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeerSender.Projections.Projections
{
    public static class ProjectionExtensions
    {
        public static void RegisterProjections(this IServiceCollection services)
        {
            services.AddTransient<OpenBoxProjection>();

            services.AddHostedService<ProjectionService<OpenBoxProjection>>();
        }
    }
}
