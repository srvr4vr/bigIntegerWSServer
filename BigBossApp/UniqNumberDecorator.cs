using System.Numerics;
using ConcurrentCollections;

namespace BigBossApp; 

//Декоратор контролит уникальность значений
//В бою можно хранить не in memory, а в каком-то внешнем хранилище, Redis, Database, etc.
//async тут именно для этого "навырост", хотя для данной задачи оверинжинеринг, признаю
public class UniqNumberDecorator: IBigNumberSource 
{
    private readonly IBigNumberSource _source;
    private readonly ConcurrentHashSet<BigInteger> _cache = new();

    public UniqNumberDecorator(IBigNumberSource source) 
    {
        _source = source;
    }

    public async Task<BigInteger> getNumberAsync() {
        BigInteger result;
        
        do 
        {
            result = await _source.getNumberAsync();
        }
        while (_cache.TryGetValue(result, out _));

        _cache.Add(result);

        return result;
    }
}