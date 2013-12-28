using System.Security.Cryptography;

namespace SvetlinAnkov.Albite.Core.IO
{
    public interface IAlbiteHashableContainer : IAlbiteContainer
    {
        // Hash of all container's data
        byte[] ComputeHash(HashAlgorithm hashAlgorithm);
    }
}
