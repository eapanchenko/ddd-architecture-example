namespace DDD.Example.Api.Extensions;

public interface IOptionsConfigurator {
    IOptionsConfigurator Add<TOptions>(string sectionName) where TOptions : class;
}