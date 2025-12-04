namespace WorkflowApi.Infrastructure.Extensions
{
    public static class HttpClientExtensions
    {
        public static string BuildUrl(this HttpClient client, string relativePath)
        {
            if (client.BaseAddress == null)
                return relativePath;

            var baseUrl = client.BaseAddress.ToString().TrimEnd('/');
            var path = relativePath.TrimStart('/');
            return $"{baseUrl}/{path}";
        }
    }
}
