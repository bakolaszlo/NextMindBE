using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using MLAPI.Cryptography.KeyExchanges;
using System;
using System.Security.Cryptography;
using System.Text;
using ECDiffieHellman = MLAPI.Cryptography.KeyExchanges.ECDiffieHellman;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NextMindBE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DHController : ControllerBase
    {
        ECDiffieHellman serverDiffie;
        public static byte[] sharedKey;
        private static byte[] salt = { 198, 154, 92, 97, 99, 104, 126, 161, 170, 57, 37, 222, 211, 118, 55, 26, 8, 71, 35, 222, 111, 247, 211, 200, 43, 59, 72, 142, 96, 120, 47, 222 };
        private static byte[] info = new System.Text.UTF8Encoding().GetBytes("Message");

        public DHController()
        {
            serverDiffie = new ECDiffieHellman();
        }

        [HttpPost]
        public IActionResult PostPublicKey([FromBody] string clientPublicBase64)
        {
            byte[] clientPublic = Convert.FromBase64String(clientPublicBase64);
            sharedKey = serverDiffie.GetSharedSecretRaw(clientPublic);
            var b64Key = Convert.ToBase64String(serverDiffie.GetPublicKey());

            var sharedKeyString = Convert.ToBase64String(sharedKey);
            Console.WriteLine(b64Key);
            Console.WriteLine(sharedKeyString);
            return Ok(b64Key);
        }

        [HttpGet]
        public IActionResult TestDerviceKey()
        {
            Console.WriteLine(Convert.ToBase64String(DeriveKey(196)));
            return Ok();
        }

        public static byte[] DeriveKey(int length)
        {
            // Use HMAC-SHA256 to derive a one-time pad encryption key
            using (var hmac = new HMACSHA256(sharedKey))
            {
                byte[] prk = hmac.ComputeHash(salt);
                byte[] key = new byte[length];
                byte[] t = new byte[0];

                for (int i = 0; i < length; i++)
                {
                    if (i % 32 == 0)
                    {
                        t = hmac.ComputeHash(Concat(prk, Concat(t, info, new byte[] { (byte)(i / 32 + 1) })));
                    }
                    key[i] = t[i % 32];
                }

                return key;
            }
        }

        private static byte[] Concat(params byte[][] arrays)
        {
            int length = 0;
            foreach (byte[] array in arrays)
            {
                length += array.Length;
            }

            byte[] result = new byte[length];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                Buffer.BlockCopy(array, 0, result, offset, array.Length);
                offset += array.Length;
            }

            return result;
        }

        public static byte[] Cipher(byte[] ciphertext)
        {
            byte[] key = DeriveKey(ciphertext.Length);
            // Decrypt the ciphertext using the one-time pad
            byte[] plaintext = new byte[ciphertext.Length];
            for (int i = 0; i < ciphertext.Length; i++)
            {
                plaintext[i] = (byte)(ciphertext[i] ^ key[i]);
            }

            return plaintext;
        }
    }
}
