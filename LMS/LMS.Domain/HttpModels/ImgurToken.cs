namespace LMS.Domain.Entities.HttpEntities
{
    public class ImgurToken
    {
        public Guid ImgurTokenId { get; set; }

        public string AccessToken { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;
    }
}