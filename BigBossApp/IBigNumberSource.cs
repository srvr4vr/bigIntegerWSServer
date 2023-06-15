using System.Numerics;

namespace BigBossApp;

public interface IBigNumberSource 
{
    Task<BigInteger> GetNumberAsync();
}