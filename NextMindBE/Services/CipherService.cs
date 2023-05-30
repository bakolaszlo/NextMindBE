using NextMindBE.Interfaces.Service;
using System.Security.Cryptography;

namespace NextMindBE.Services
{
    public class CipherService : ICipher
    {
        private byte[] salt = { 198, 154, 92, 97, 99, 104, 126, 161, 170, 57, 37, 222, 211, 118, 55, 26, 8, 71, 35, 222, 111, 247, 211, 200, 43, 59, 72, 142, 96, 120, 47, 222 };
        private byte[] info = new System.Text.UTF8Encoding().GetBytes("Message");
        public byte[] Cipher(byte[] ciphertext, byte[] sharedKey)
        {
            byte[] key = DeriveKey(ciphertext.Length, sharedKey);
            // Decrypt the ciphertext using the one-time pad
            byte[] plaintext = new byte[ciphertext.Length];
            for (int i = 0; i < ciphertext.Length; i++)
            {
                plaintext[i] = (byte)(ciphertext[i] ^ key[i]);
            }

            return plaintext;
        }

        public byte[] DeriveKey(int length, byte[] sharedKey)
        {
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

        private byte[] Concat(params byte[][] arrays)
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
    }
}
