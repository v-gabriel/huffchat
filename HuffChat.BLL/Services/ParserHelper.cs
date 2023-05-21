using System.Collections;
using System.Numerics;

namespace HuffChat.BLL.Services
{
    public class ParserHelper
    {
        public static string ParseStringFromBitArray(BitArray bits)
        {
            var encodedMessage = String.Empty;
            foreach (var bit in bits)
            {
                if (Boolean.TryParse(bit.ToString(), out bool result))
                {
                    encodedMessage += result == true ? '1' : '0';
                }
            }

            return encodedMessage;
        }

        public static BitArray ParseBitArrayFromString(string value)
        {
            var bitMessage = new BitArray(value.Select(c => c == '1').ToArray());
            return bitMessage;
        }
    }
}
