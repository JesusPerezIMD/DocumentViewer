namespace document_viewer_app.Services
{
    public class DocumentViewer
    {
        public class AuthRequest
        {
            public string secretKey { get; set; }
        }

        public class AuthResponse
        {
            public string status { get; set; }
            public string fileUrl { get; set; }
            public string message { get; set; }
        }
    }
}
