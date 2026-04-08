namespace LMS.API.DependencyInjection;

public class LazyResolver<T> : Lazy<T>
    where T : notnull
{
    public LazyResolver(IServiceProvider provider)
        : base(provider.GetRequiredService<T>)
    {
    }
}