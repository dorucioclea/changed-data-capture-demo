namespace Syncer.Fa
{
    public interface IFactory<out T>
    {
        T Create(string name);
    }
}