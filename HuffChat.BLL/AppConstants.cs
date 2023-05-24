using HuffChat.BLL.Enums;

namespace HuffChat.BLL
{
    public static class AppConstants
    {
        private static readonly string alphabet = "abcdefghijklmnopqrstuvwxyzšđčćž";
        private static readonly Random random = new Random();
        public readonly static string HUFFMAN_DEFAULT_BUILD_INPUT =
                new string(
                    $"{alphabet}{alphabet.ToUpper()}{SocketResponse.END_OF_MESSAGE}0123456789 !'#$%&/=?*-.:,<>@"
                    .ToCharArray()
                    .OrderBy(s => (random.Next(2) % 2) == 0).ToArray());

    }
}
