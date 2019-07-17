using System.IO;
using System.IO.Compression;
using UnityEngine;
using ZXing;
using ZXing.QrCode;

public class MazeTools {
#region QRCode
    public static string textToBase64 (string text, bool compress) {
        string o = "";
        byte[] bytesToEncode = System.Text.Encoding.UTF8.GetBytes (text);
        using (MemoryStream encoded = new MemoryStream (bytesToEncode)) {
            using (MemoryStream stream = new MemoryStream ()) {
                if (compress) {
                    using (GZipStream zip = new GZipStream (stream, CompressionMode.Compress, true)) {
                        encoded.CopyTo (zip);
                        // zip.Write (bytesToEncode, 0, bytesToEncode.Length);
                    }
                } else {
                    encoded.CopyTo (stream);
                }
                stream.Flush ();
                o = System.Convert.ToBase64String (stream.ToArray ());
                stream.Close ();
            }
            encoded.Close ();
        }
        return o;
    }
    public static string base64ToText (string text, bool compress) {
        string o = "";
        byte[] bytesToDecode = System.Convert.FromBase64String (text);
        using (MemoryStream decoded = new MemoryStream (bytesToDecode)) {
            using (MemoryStream stream = new MemoryStream ()) {
                if (compress) {
                    using (GZipStream zip = new GZipStream (decoded, CompressionMode.Decompress, true)) {
                        zip.CopyTo (stream);
                        // zip.Write (bytesToDecode, 0, bytesToDecode.Length);
                    }
                } else {
                    decoded.CopyTo (stream);
                }
                // o = System.Convert.ToBase64String (stream.ToArray ());
                stream.Flush ();
                o = System.Text.Encoding.UTF8.GetString (stream.ToArray ());
                stream.Close();
            }
            decoded.Close();
        }
        return o;
    }
    public static Color32[] EncodeToQRCode (string textForEncoding,
        int width, int height) {
        var writer = new BarcodeWriter {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions {
            Height = height,
            Width = width
            }
        };
        return writer.Write (textForEncoding);
    }
    public static Texture2D Color32ToTexture (Color32[] color32, int width, int height) {
        var encoded = new Texture2D (width, height);
        encoded.SetPixels32 (color32);
        encoded.Apply ();
        return encoded;
    }

    public static Texture2D GenerateQRCode (string text, int width, int height) {
        string encoded = textToBase64 (text, true);
        Color32[] color32 = EncodeToQRCode (encoded, width, height);
        return Color32ToTexture (color32, width, height);
    }

    public static void testConvertion (string t) {
        string b = textToBase64 (t, true);
        string d = base64ToText (b, true);
        Debug.Log (t.Equals (d));

        b = textToBase64 (t, false);
        d = base64ToText (b, false);
        Debug.Log (t.Equals (d));
    }
#endregion
}
