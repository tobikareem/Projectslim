namespace Slim.Pages.Extensions
{
    public static class UtilityExtension
    {
        /// <summary>
        ///    Return Image Url
        /// </summary>
        /// <param name="imageItem"></param>
        /// <param name="profileImageByte"></param>
        /// <returns></returns>
        public static string GetImageSrc(this string imageItem, byte[]? profileImageByte)
        {
            var imageData = string.Empty;
            if (profileImageByte != null)
            {
                imageData = Convert.ToBase64String(profileImageByte);
            }

            imageItem = $"data:image/png;base64,{imageData}";

            return string.IsNullOrWhiteSpace(imageData) ? string.Empty : imageItem;
        }
    }
}
