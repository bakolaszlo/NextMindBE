namespace NextMindBE.Interfaces.Service
{
    public interface ICipher
    {
        byte[] Cipher(byte[] ciphertext, byte[] sharedKey);
    }
}
