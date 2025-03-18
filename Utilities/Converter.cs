
namespace Utilities;

using System.Text;
using System.Globalization;
public class Converter
{
	public static byte[] ToBytes(string value)
        => Encoding.UTF8.GetBytes(value ?? string.Empty);

    public static string ToString(byte[] bytes)
        => bytes == null || bytes.Length == 0 ? string.Empty : Encoding.UTF8.GetString(bytes);

    public static byte[] FromBase64(string base64)
    {
        if (string.IsNullOrEmpty(base64)) return System.Array.Empty<byte>();
        return System.Convert.FromBase64String(base64);
    }

    public static string ToBase64(byte[] data)
    {
        if (data == null || data.Length == 0) return string.Empty;
        return System.Convert.ToBase64String(data);
    }

	public static string ToStringU16(ReadOnlySpan<char> data, CultureInfo? culture = null)
    {
        // 필요 시 구현
        return data.ToString();
    }
}