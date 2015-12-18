namespace VkTunes.IoC
{
    public interface IFactory<TInstance>
    {
        TInstance CreateInstance();
    }
}