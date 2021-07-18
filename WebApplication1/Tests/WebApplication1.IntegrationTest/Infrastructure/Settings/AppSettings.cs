using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace WebApplication1.IntegrationTest.Infrastructure.Settings
{
    public class AppSettings<T> : IOptions<T>
        where T : class, new()
    {
        public AppSettings(string sectionKey)
        {
            IConfiguration configuration = TestServer.Configuration.GetSection(sectionKey);
            var options = new T();
            configuration.Bind(options);

            this.Value = options;
        }

        public T Value { get; set; }
    }
}
