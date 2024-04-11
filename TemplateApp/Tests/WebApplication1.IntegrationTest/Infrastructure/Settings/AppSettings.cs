using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace WebApplication1.IntegrationTest.Infrastructure.Settings
{
    public class AppSettings<T> : IOptions<T>
        where T : class, new()
    {
        public AppSettings(string sectionKey)
        {
            var configurationSection = TestServer.Configuration.GetSection(sectionKey);
            var options = new T();
            configurationSection.Bind(options);

            this.Value = options;
        }

        public T Value { get; set; }
    }
}
