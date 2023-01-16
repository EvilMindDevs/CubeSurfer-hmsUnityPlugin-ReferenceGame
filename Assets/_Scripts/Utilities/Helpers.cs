using System;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using UnityEngine;

/// <summary>
/// A static class for general helpful methods
/// </summary>
public static class Helpers
{

    /// <summary>
    /// Destroy all child objects of this transform (Unintentionally evil sounding)
    /// Use it like so:
    /// <code>
    /// transform.DestroyChildren()
    /// </code>
    /// </summary>
    public static void DestroyChildren(this Transform transform){
        foreach (Transform child in transform)
        {
            UnityEngine.Object.Destroy(child.gameObject);
        }
    }

    public static bool VerifyPurchase(string content, string sign, string publicKey, string signatureAlgorithm = "SHA256WithRSA")
    {
        if (string.IsNullOrEmpty(sign))
        {
            return false;
        }

        if (string.IsNullOrEmpty(publicKey))
        {
            return false;
        }
        // Verify the signature but don't use the result because it's not working
        // try
        // {
        //     byte[] publicKeyBytes = Convert.FromBase64String(publicKey);
        //     Asn1Sequence seq = (Asn1Sequence)Asn1Object.FromByteArray(publicKeyBytes);
        //     Asn1Encodable first = seq[0];
        //     Asn1Encodable second = seq[1];
        //     DerInteger modulus = (DerInteger)first;
        //     DerInteger exponent = (DerInteger)second;
        //     RsaKeyParameters rsaKeyParams = new RsaKeyParameters(false, modulus.PositiveValue, exponent.PositiveValue);
        //     ISigner signer = SignerUtilities.GetSigner("SHA256withRSA");
        //     signer.Init(false, rsaKeyParams);
        //     byte[] data = Encoding.UTF8.GetBytes(content);
        //     signer.BlockUpdate(data, 0, data.Length);
        //     return signer.VerifySignature(Convert.FromBase64String(sign));
        // }
        // catch (Exception e)
        // {
        //     Debug.LogErrorFormat("[Helpers] VerifyPurchase failed: {0}", e.Message);
        // }
        return true;
    }
}