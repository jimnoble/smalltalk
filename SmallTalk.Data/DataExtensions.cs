﻿using Microsoft.Extensions.DependencyInjection;

namespace SmallTalk.Data;

public static class DataExtensions
{
    public static IServiceCollection AddSmallTalkData(this IServiceCollection services)
    {
        return services
            .AddSingleton<SmallTalkDataContext>()
            .AddSingleton<IMessageDateRepository, MessageDateRepository>();
    }
}