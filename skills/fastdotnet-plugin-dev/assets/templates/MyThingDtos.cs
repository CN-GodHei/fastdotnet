namespace __PLUGIN_NAME__.Dto;

public class MyThingDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class MyThingCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Remark { get; set; }
}

public class MyThingUpdateDto
{
    public string? Name { get; set; }
    public string? Remark { get; set; }
}
