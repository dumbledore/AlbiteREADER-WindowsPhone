using System.Security.Cryptography;

namespace Albite.Reader.Core.IO
{
    public interface IHashableContainer : IContainer
    {
        // Hash of all container's data
        byte[] ComputeHash(HashAlgorithm hashAlgorithm);
    }
}
