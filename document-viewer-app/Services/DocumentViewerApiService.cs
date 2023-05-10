using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using static document_viewer_app.Services.DocumentViewer;

namespace document_viewer_app.Services
{
    public class DocumentViewerApiService
    {
        public async Task<AuthResponse> Auth(AuthRequest data)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var apiEndPoint = "https://bconnectapitest.azurewebsites.net/Auth/DocumentViewer";
                    httpClient.BaseAddress = new Uri(apiEndPoint);
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var jsonData = JsonConvert.SerializeObject(data);
                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    var responseMessage = await httpClient.PostAsync(apiEndPoint, content);
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        var jsonContent = await responseMessage.Content.ReadAsStringAsync();
                        var response = JsonConvert.DeserializeObject<AuthResponse>(jsonContent);

                        return response;
                    }
                    else
                    {
                        throw new Exception(responseMessage.ReasonPhrase);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
