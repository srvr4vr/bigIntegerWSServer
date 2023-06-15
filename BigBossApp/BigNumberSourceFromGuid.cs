using System.Numerics;

namespace BigBossApp;

//Вариант через GUID, он не гарантирует что гуид будет уникальный
//НО ВЕРОЯТНОСТЬ ˜˜РЕЗИСТА˜˜КОЛЛИЗИИ КРАЙНЕ МАЛА =)
//оставлю не заюзанным, в качестве прикола, но зато память не нужно забивать
//Еще из минусов числа получаются довольно большие, а по заданию не понятно ок или не ок
public class BigNumberSourceFromGuid : IBigNumberSource 
{
    public async Task<BigInteger> GetNumberAsync() 
    {
        var guid = new Guid();
        var value = new BigInteger(guid.ToByteArray());
        return await Task.FromResult(value);
    }
}