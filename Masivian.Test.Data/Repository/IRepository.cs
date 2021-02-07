using StackExchange.Redis;
namespace Masivian.Test.Data.Repository
{
    public interface IRepository
    {
        HashEntry[] Get(string key);
        string GetById(string key, string fieldId);
        bool Post(string hashKey, string fieldKey, string value);

    }
}