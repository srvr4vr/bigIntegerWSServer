using System.Numerics;

namespace BigBossApp;

public class BigNumberSource : IBigNumberSource {
    public async Task<BigInteger> getNumberAsync() {
        var lenght = Random.Shared.Next(1, 100);
        var bytes = new byte[lenght];    
        Random.Shared.NextBytes(bytes);
        var value = new BigInteger(bytes);
        return await Task.FromResult(value);
    }
}