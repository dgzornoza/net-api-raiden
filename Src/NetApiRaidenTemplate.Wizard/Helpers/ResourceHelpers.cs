using System.Reflection;

namespace NetApiRaidenTemplate.Wizard.Helpers
{
    public static class ResourceHelpers
    {
        /// <summary>
        /// Helper to get file bytes from embedded resource.
        /// </summary>
        /// <param name="resourceName">Resource name relative to project ex: '{folders}.{resourceName}'</param>
        /// <returns>Resource file bytes</returns>
        public static byte[] GetEmbeddedResource(string resourceName)
        {
            byte[] bytes;
            using (System.IO.Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"NetApiRaidenTemplate.Wizard.{resourceName}"))
            {
                bytes = new byte[resourceStream.Length];
                resourceStream.Read(bytes, 0, (int)resourceStream.Length);
            }

            return bytes;
        }
    }
}
