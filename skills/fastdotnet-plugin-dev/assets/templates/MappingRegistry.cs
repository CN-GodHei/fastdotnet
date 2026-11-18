namespace __PLUGIN_NAME__.Mappings;

/// <summary>
/// Mapster 映射。插件程序集内的 IRegister 会在启用时被宿主自动扫描注册。
/// </summary>
public class MyMappingRegistry : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<MyThing, MyThingDto>().MaskSensitiveData();
        config.NewConfig<MyThingCreateDto, MyThing>();
        config.NewConfig<MyThingUpdateDto, MyThing>().IgnoreNullValues(true);
    }
}
