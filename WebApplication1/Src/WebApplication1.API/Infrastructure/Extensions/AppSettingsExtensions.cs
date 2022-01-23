﻿using Microsoft.Extensions.Configuration;

namespace WebApplication1.Api.Settings
{
    public static class AppSettingsExtension
    {
        public static T GetObject<T>(this IConfiguration configuration, string sectionKey) where T : class, new()
        {
            var configurationSection = configuration.GetSection(sectionKey);
            var options = new T();
            configurationSection.Bind(options);

            return options;
        }
    }
}
